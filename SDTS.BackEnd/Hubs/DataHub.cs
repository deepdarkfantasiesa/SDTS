using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Models;
using SDTS.DataAccess.Interface;
using SDTS.DataAccess.Migrations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using System.Threading.Tasks;

namespace SDTS.BackEnd.Hubs
{
    [Authorize(Policy = "datahub")]
    public class DataHub:Hub
    {
        IMockData mock;
        IEmergencyTimers timers;
        private readonly IUserRepository _user;
        private readonly IConnectedUsersRepository _connectedUsers;
        private readonly IUserDataRepository _userData;
        private readonly IEmergencyHelpersRepository _emergencyHelpers;
        private readonly IRescureGroupRepository _rescureGroups;
        private readonly IIsPublishedsRepository _isPublisheds;
        private readonly ISecureAreaRepository _secureArea;
        public DataHub(IMockData data,IEmergencyTimers emergency, 
            IUserRepository user,
            IConnectedUsersRepository connectedUsers, 
            IUserDataRepository userData,
            IEmergencyHelpersRepository emergencyHelpers,
            IRescureGroupRepository rescureGroups,
            IIsPublishedsRepository isPublisheds,
            ISecureAreaRepository secureArea)
        {
            mock = data;
            //var id = Context.User.Claims.First(p => p.Type.Equals("UserID")).Value;

            timers = emergency;

            _user = user;
            _connectedUsers = connectedUsers;
            _userData = userData;
            _emergencyHelpers = emergencyHelpers;
            _rescureGroups = rescureGroups;
            _isPublisheds = isPublisheds;
            _secureArea = secureArea;
        }
        //要，手机端调用发送数据
        public async Task SendSensorsDataToBackEnd(SensorData data)
        {
            var type = Context.User.Claims.First(p => p.Type.Equals("Type")).Value;

            if(type.Equals("被监护人"))
            {
                foreach (var acc in data.dataAcc)
                {
                    if (acc != null && acc.Item1 * acc.Item1 + acc.Item2 * acc.Item2 + acc.Item3 * acc.Item3 > 3) 
                    {
                        await PublishEmergencyInformationTimer(data);
                    }
                }
                await SendDataToGuardian(data);
                //Debug.WriteLine(data.Latitude+"\n"+data.Longitude);
            }

            if (type.Equals("志愿者")|| type.Equals("监护人"))
            {
                var groupname = mock.UserInRescuerGroup(data.user.Account);
                if (groupname!=null)
                {
                    await Clients.Group(groupname).SendAsync("ReceiveRescuerData",data);
                }
            }



            mock.AlterConnectUserData(Context.ConnectionId, data);





            Debug.WriteLine($"Name:{data.user.Name} Acc:{data.dataAcc.Count} Lat:{data.Latitude} Long:{data.Longitude}");

            //Debug.WriteLine(data.dataAcc.Count);
            //Debug.WriteLine(data.dataBar.Count);
            //Debug.WriteLine(data.dataGyr.Count);
            //Debug.WriteLine(data.dataMag.Count);
            //Debug.WriteLine(data.dataOri.Count);
            //Debug.WriteLine(data.Latitude);
            //Debug.WriteLine(data.Longitude);
            //Debug.WriteLine(data.user.Name);
            //Debug.WriteLine(data.dateTime);
        }

        public async Task BackEndReceiveData(SensorsData data)
        {
            var type = Context.User.Claims.First(p => p.Type.Equals("Type")).Value;
            data.ConnectionId = Context.ConnectionId;
            /*计算传感器数据*/
            var computeddata =await ComputeData(data,type);
            /*计算传感器数据*/

            if (type.Equals("被监护人"))
            {
                await SendDataToOthers(computeddata,type);
                //await _userData.AlterUserDatasAsync(computeddata);
            }

            if (type.Equals("志愿者") || type.Equals("监护人"))
            {
                //var groupname = mock.UserInRescuerGroup(data.user.Account);
                //if (groupname != null)
                //{
                //    await Clients.Group(groupname).SendAsync("ReceiveRescuerData", data);
                //}

                //await _userData.AlterUserDatasAsync(computeddata);
                var rescurer = await _rescureGroups.QueryRescurer(computeddata.Account);
                if(rescurer!=null)
                {
                    await Clients.Group(rescurer.GroupName).SendAsync("ReceiveOthersData", computeddata,type);
                    //Debug.WriteLine("Share Data Successfully");
                }
            }
            await _userData.AlterUserDatasAsync(computeddata);





            //Debug.WriteLine($"Name:{data.Account} Acc:{data.dataAcc.Count} Lat:{data.Latitude} Long:{data.Longitude}");

            //Debug.WriteLine(data.dataAcc.Count);
            //Debug.WriteLine(data.dataBar.Count);
            //Debug.WriteLine(data.dataGyr.Count);
            //Debug.WriteLine(data.dataMag.Count);
            //Debug.WriteLine(data.dataOri.Count);
            //Debug.WriteLine(data.Latitude);
            //Debug.WriteLine(data.Longitude);
            //Debug.WriteLine(data.user.Name);
            //Debug.WriteLine(data.dateTime);
        }

        //Timer publishtimer = null;
        //private SensorsData ComputeData(SensorsData data, string type)
        private async Task<SensorsData> ComputeData(SensorsData data,string type)
        {
            /*模拟gps数据*/
            //if(data.Account=="1")
            //{
            //    var result = await _emergencyHelpers.QueryEmergencyHelper(data.Account);
            //    if(result==null)
            //    {
            //        data.Latitude = 22.3254973 + mock.mockdata;
            //        data.Longitude = 114.1671742 + mock.mockdata;
            //        mock.mockdata += 0.0000003;

            //    }
            //    else
            //    {
            //        var helperdata = await _userData.QueryUserDatasAsync(Context.ConnectionId);
            //        data.Latitude = helperdata.Latitude - mock.mockdata;
            //        data.Longitude = helperdata.Longitude - mock.mockdata;
            //        mock.mockdata += 0.0000003;
            //    }
                
            //}
            /*模拟gps数据*/


            if (data.dataBar.Count != 0)
            {
                data.BarometerData = data.dataBar.Average();
            }
            else
            {
                data.BarometerData = 0;
            }
            //用加速度计和陀螺仪计算移动距离后再校准gps数据


            if (type.Equals("被监护人"))
            {
                foreach (var acc in data.dataAcc)
                {
                    //if (acc != null && acc.Item1 * acc.Item1 + acc.Item2 * acc.Item2 + acc.Item3 * acc.Item3 > 3)
                    if (await _emergencyHelpers.QueryEmergencyHelper(data.Account) == null && acc != null && acc.Item1 * acc.Item1 + acc.Item2 * acc.Item2 + acc.Item3 * acc.Item3 > 3) 
                    {
                        var helper = _user.GetUser(data.Account);
                        var groupname = DateTime.Now.ToString("yyyyMMddHHmmss") + helper.Account;
                        var result=await _emergencyHelpers.CreateEmergencyHelper(new EmergencyHelper() {
                           Name=helper.Name,
                           Information=helper.Information,
                           Birthday=helper.Birthday,
                           Account=helper.Account,
                           Gender=helper.Gender,
                            Latitude=data.Latitude,
                            Longitude=data.Longitude,
                            dateTime=data.dateTime,
                            Problem="滑倒",
                            PhoneNumber=helper.PhoneNumber,
                            Altitude=data.BarometerData,
                            ConnectionId=Context.ConnectionId,
                            GroupName= groupname
                        });
                        timers.InitTimer();
                        //Debug.WriteLine(result.Result);
                        await Groups.AddToGroupAsync(Context.ConnectionId, groupname);

                    }
                }
            }



            SensorsData computedata = new SensorsData();

            computedata.Name = data.Name;
            computedata.Latitude = data.Latitude;
            computedata.Longitude = data.Longitude;
            computedata.ConnectionId = data.ConnectionId;
            computedata.dateTime = data.dateTime;
            computedata.Account = data.Account;
            //if (data.dataBar != null)
            //{
            //    computedata.BarometerData = data.dataBar.Average();
            //}
            computedata.BarometerData = data.BarometerData;
            return computedata;
        }


        //推送求救信息给监护人
        public async Task PublishEmergencyInformationTimer(SensorData data)
        {
            var wardaccount = Context.User.Claims.First(p => p.Type.Equals("Account")).Value;
            
            if (mock.FindEmergencyHelpers(wardaccount) == null)
            {
                mock.AddEmergencyHelpers(wardaccount, data.Latitude, data.Longitude, Context.ConnectionId, "滑倒");
                timers.Init();
            }
            else
            {
                return;
            }
        }
        //要，加入救援小组
        public async Task JoinRescueGroup(User Rescuer,Helpers helper)
        {
            var groupname = mock.AddRescuerInGroup(Rescuer.Account, helper.Account);
            var rescuerconid = mock.FindConnectedUser(Rescuer.Account);
            await Groups.AddToGroupAsync(rescuerconid, groupname);

            
        }

        public async Task UserJoinRescueGroup(string rescueraccount, string helperaccount)
        {
            var helper = await _emergencyHelpers.QueryEmergencyHelper(helperaccount);
            await Groups.AddToGroupAsync(Context.ConnectionId, helper.GroupName);
            await _rescureGroups.AddToRescureGroup(new RescureGroup()
            {
                Account = rescueraccount,
                ConnectionId = Context.ConnectionId,
                GroupName = helper.GroupName
            });
        }
        public async Task UserFinishRescue(string rescueraccount, string helperaccount)
        {
            var rescuerdata = await _userData.QueryUserDatasAsync(Context.ConnectionId);
            var helperconnectionid =await _connectedUsers.QueryConnectUserAsync(helperaccount);
            var helperdata = await _userData.QueryUserDatasAsync(helperconnectionid);

            var lat = Math.Abs(rescuerdata.Latitude - helperdata.Latitude);
            var lon = Math.Abs(rescuerdata.Longitude - helperdata.Longitude);
            double bardata = 0;
            if (rescuerdata.BarometerData != 0 && helperdata.BarometerData != 0)
            {
                bardata = Math.Abs(rescuerdata.BarometerData - helperdata.BarometerData);
            }
            if (lat > 0.003 || lon > 0.003)
            {
                if (bardata != 0)
                {
                    await Clients.Caller.SendAsync("FinishResult", false, new SensorsData() { Latitude = lat, Longitude = lon, dataBar = new List<double>() { bardata } }, helperaccount);
                    //Debug.WriteLine("\nFinishResult1\n");
                }
                else if (bardata == 0)
                {
                    await Clients.Caller.SendAsync("FinishResult", false, new SensorsData() { Latitude = lat, Longitude = lon, dataBar = new List<double>() { 999 } }, helperaccount);
                    //Debug.WriteLine("\nFinishResult2\n");
                }
                return;
            }

            var helper = await _emergencyHelpers.QueryEmergencyHelper(helperaccount);
            var groupname = helper.GroupName;
            var Rescuer = _user.GetUser(rescueraccount);
            if (groupname != null)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupname);
                await Clients.Group(groupname).SendAsync("OthersFinishRescue", Rescuer.Type + ":" + Rescuer.Name + " 完成了救援");
                var deleterescurerresult = await _rescureGroups.DeleteRescurerAsync(rescueraccount);
                if (deleterescurerresult.Equals(false))
                {
                    return;
                }
                var otherrescuers = await _rescureGroups.DeleteRescurersAsync(groupname);
                foreach(var otherrescuer in otherrescuers)
                {
                    await Groups.RemoveFromGroupAsync(otherrescuer.ConnectionId, groupname);
                }
                await Groups.RemoveFromGroupAsync(helper.ConnectionId, groupname);
                var deleteEHelper = await _emergencyHelpers.DeleteEmergencyHelper(helperaccount);
                await _isPublisheds.DeleteUsers(helperaccount);
                if (deleteEHelper.Equals(true))
                {
                    var rescuer = await _userData.QueryUserDatasAsync(Context.ConnectionId);
                    //await Clients.Caller.SendAsync("FinishResult", true, null);
                    await Clients.Caller.SendAsync("FinishResult", true, rescuer, helperaccount);
                    Debug.WriteLine("\nFinishResult3\n");
                }
            }
        }

        //要，救援者（加入救援队的志愿者和监护人）端调用
        public async Task VolunteerFinishRescue(User Rescuer, Helpers helper)
        {
            var rescuerdata = mock.FindConnectUserData(Context.ConnectionId);
            var helperdata = mock.FindConnectUserData(helper.ConnectionId);

            var lat = Math.Abs(rescuerdata.Latitude - helperdata.Latitude);
            var lon = Math.Abs(rescuerdata.Longitude - helperdata.Longitude);

            double bar=0;
            if(rescuerdata.dataBar.Count!=0&& helperdata.dataBar.Count!=0)
            {
                bar = Math.Abs(rescuerdata.dataBar.Average() - helperdata.dataBar.Average());
            }
            

            if (lat > 0.003||lon > 0.003)
            {
                if(bar!=0)
                {
                    await Clients.Caller.SendAsync("FinishRescueResult", false, new SensorData() { Latitude = lat, Longitude = lon, dataBar = new List<double>() { bar } });
                }
                else if (bar == 0)
                {
                    await Clients.Caller.SendAsync("FinishRescueResult", false, new SensorData() { Latitude = lat, Longitude = lon, dataBar = new List<double>() { 999 } });
                }
                return;
            }

            var groupname = mock.FindRescuerGroup(helper.Account);
            
            if(groupname!=null)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupname);
                await Clients.Group(groupname).SendAsync("SomeBodyFinishRescue", Rescuer.Type+":"+Rescuer.Name+" 完成了救援");
                if(mock.RemoveRescuer(helper.Account,Rescuer.Account).Equals(false))
                {
                    return;
                }
                var rescuers = mock.FindAllRescuer(helper.Account);
                if (rescuers!=null)
                {
                    foreach(var rescuer in rescuers)
                    {
                        await Groups.RemoveFromGroupAsync(mock.FindConnectedUser(rescuer), groupname);
                    }
                }

                if(mock.RemoveEmergencyHelpers(helper.Account))
                {
                    await Clients.Caller.SendAsync("FinishRescueResult", true,null);
                }


            }


        }



        public async Task PublishEmergencyInformationToGuardians(double Latitude,double Longitude)
        {
            var wardaccount = Context.User.Claims.First(p => p.Type.Equals("Account")).Value;
            if(mock.FindEmergencyHelpers(wardaccount)==null)
            {
                mock.AddEmergencyHelpers(wardaccount, Latitude, Longitude,Context.ConnectionId,"滑倒");
            }
            else
            {
                return;
            }

            var guardians = mock.getguardians(wardaccount);//获取该被监护人的监护人账号

            List<string> connectguardianids = new List<string>();

            foreach (var guardian in guardians)
            {
                var connectguardianid = mock.ReflashGuardians(guardian.Account);
                if (connectguardianid != null)
                    connectguardianids.Add(connectguardianid);//将已连接的监护人连接id存起来
            }

            foreach (var connectguardianid in connectguardianids)
            {
                var helper = mock.FindEmergencyHelpers(wardaccount);
                await Clients.Client(connectguardianid).SendAsync("loadhelpers", helper);//向已连接的监护人发送被监护人的求救信息
            }

            
        }

        public async Task SendDataToOthers(SensorsData data,string type)
        {
            var wardaccount = Context.User.Claims.First(p => p.Type.Equals("Account")).Value;
            var guardians = await _user.GetGuardiansAsync(wardaccount);
            List<string> connectguardianids = new List<string>();
            foreach (var guardian in guardians)
            {
                var connectionid =await _connectedUsers.QueryConnectUserAsync(guardian.Account);
                if(connectionid!=null)
                {
                    connectguardianids.Add(connectionid);
                }
            }
            foreach(var connectguardianid in connectguardianids)
            {
                await Clients.Client(connectguardianid).SendAsync("ReceiveDataFromOthers", data);//向已连接的监护人发送被监护人的数据
            }

            var helper =await _emergencyHelpers.QueryEmergencyHelper(data.Account);
            if(helper!=null)
            {
                await Clients.Group(helper.GroupName).SendAsync("ReceiveOthersData", data,type);
            }
        }

        //要，被监护人把数据发送给监护人，如果此被监护人有救援事件，则同时将数据发送给救援小组
        public async Task SendDataToGuardian(SensorData data)
        {
            var wardaccount = Context.User.Claims.First(p => p.Type.Equals("Account")).Value;

            var guardians = mock.getguardians(wardaccount);//获取该被监护人的监护人列表

            List<string> connectguardianids = new List<string>();

            foreach (var guardian in guardians)
            {
                var connectguardianid = mock.ReflashGuardians(guardian.Account);//获取已连接的监护人connectionid
                if (connectguardianid != null)
                    connectguardianids.Add(connectguardianid);//将已连接的监护人连接id存起来
            }

            foreach (var connectguardianid in connectguardianids)
            {
                await Clients.Client(connectguardianid).SendAsync("ReceiveData", data);//向已连接的监护人发送被监护人的数据

                

            }

            var groupname = mock.FindRescuerGroup(wardaccount);
            if(groupname!=null)
            {
                await Clients.Group(groupname).SendAsync("ReceiveRescuerData",data);
            }

            //await Clients.All.SendAsync("wardreceive", data);
            //Debug.WriteLine($"Latitude: {data.Latitude} Longitude:{data.Longitude}");
            //Debug.WriteLine($"2");
        }

        //public async Task SendMessageToGuardian(string message)
        //{
        //    var wardaccount = Context.User.Claims.First(p => p.Type.Equals("Account")).Value;

        //    var guardians = mock.getguardians(wardaccount);//获取该被监护人的监护人账号

        //    List<string> connectguardianids = new List<string>();

        //    foreach(var guardian in guardians)
        //    {
        //        var connectguardianid = mock.ReflashGuardians(guardian.Account);
        //        if (connectguardianid != null)
        //            connectguardianids.Add(connectguardianid);//将已连接的监护人连接id存起来
        //    }

        //    foreach(var connectguardianid in connectguardianids)
        //    {
        //        await Clients.Client(connectguardianid).SendAsync("GuardianReceive", message);//向已连接的监护人发送被监护人的数据
        //    }
        //}

        //public async Task ConnectToHub()
        //{
        //    var connectaccount = Context.User.Claims.First(p => p.Type.Equals("Account")).Value;

        //    var connectid = Context.ConnectionId;

        //    if(mock.AddConnectUser(connectaccount, connectid))
        //        await Clients.Client(connectid).SendAsync("Entered", "connect success!");
        //    else
        //        await Clients.Client(connectid).SendAsync("Entered", "connect success but fail to add from Dictionary，may be you are already connect!");
        //}
        
        //public async Task DisConnectToHub()
        //{
        //    var connectaccount = Context.User.Claims.First(p => p.Type.Equals("Account")).Value;

        //    var connectid = Context.ConnectionId;

        //    if(mock.RemoveConnectUser(connectaccount,connectid))
        //        await Clients.Client(connectid).SendAsync("Lefted", "disconnect success!");
        //    else
        //        await Clients.Client(connectid).SendAsync("Lefted", "disconnect success but fail to remove from Dictionary!!");
        //}

        public override async Task OnConnectedAsync()
        {
            //await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");
            await base.OnConnectedAsync();

            var connectaccount = Context.User.Claims.First(p => p.Type.Equals("Account")).Value;

            var connectid = Context.ConnectionId;

            //if (mock.AddConnectUser(connectaccount, connectid) && mock.AddConnectUserData(connectid, null))
            //    await Clients.Client(connectid).SendAsync("Entered", "connect success!");
            //else
            //    await Clients.Client(connectid).SendAsync("Entered", "connect success but fail to add from Dictionary，may be you are already connect!");

            await _connectedUsers.AddConnectUser(connectaccount, connectid);
            SensorsData AddData = new SensorsData();
            AddData.Account = connectaccount;
            AddData.ConnectionId = connectid;
            await _userData.AddUserDatasAsync(AddData);

        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var connectaccount = Context.User.Claims.First(p => p.Type.Equals("Account")).Value;

            var connectid = Context.ConnectionId;

            //if (mock.RemoveConnectUser(connectaccount, connectid)&&mock.RemoveConnectUserData(connectid))
            //{
            //    //await Clients.Client(connectid).SendAsync("Lefted", "disconnect success!");

            //} 
            //else
            //{
            //    //await Clients.Client(connectid).SendAsync("Lefted", "disconnect success but fail to remove from Dictionary!!");
            //}


            //Debug.WriteLine(DateTime.Now);
            //await ThrowException();
            //await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR Users");

            await _user.SignOut(connectaccount);
            await _connectedUsers.RemoveConnectUser(connectaccount, connectid);
    
            await _userData.DeleteUserDatasAsync(connectaccount);

            await base.OnDisconnectedAsync(exception);
            var username = Context.User.Claims.First(p => p.Type.Equals("Name")).Value;
            Debug.WriteLine($"{username} disconnected!!!!");
        }

        public Task ThrowException()
        {
            throw new HubException("This error will be sent to the client!");
        }

    }
}
