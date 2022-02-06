using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Models;
using SDTS.DataAccess.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        public DataHub(IMockData data,IEmergencyTimers emergency, IUserRepository user,IConnectedUsersRepository connectedUsers)
        {
            mock = data;
            //var id = Context.User.Claims.First(p => p.Type.Equals("UserID")).Value;

            timers = emergency;

            _user = user;
            _connectedUsers = connectedUsers;
        }
        //要，手机端调用发送数据
        public async Task SendSensorsDataToBackEnd(SensorData data)
        {
            var type = Context.User.Claims.First(p => p.Type.Equals("Type")).Value;

            //data.Latitude = data.Latitude + 1;
            //data.Longitude = data.Longitude + 2;

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

            if (mock.AddConnectUser(connectaccount, connectid) && mock.AddConnectUserData(connectid, null))
                await Clients.Client(connectid).SendAsync("Entered", "connect success!");
            else
                await Clients.Client(connectid).SendAsync("Entered", "connect success but fail to add from Dictionary，may be you are already connect!");

            _connectedUsers.AddConnectUser(connectaccount, connectid);


        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var connectaccount = Context.User.Claims.First(p => p.Type.Equals("Account")).Value;

            var connectid = Context.ConnectionId;

            if (mock.RemoveConnectUser(connectaccount, connectid)&&mock.RemoveConnectUserData(connectid))
            {
                //await Clients.Client(connectid).SendAsync("Lefted", "disconnect success!");

            } 
            else
            {
                //await Clients.Client(connectid).SendAsync("Lefted", "disconnect success but fail to remove from Dictionary!!");
            }


            //Debug.WriteLine(DateTime.Now);
            //await ThrowException();
            //await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR Users");

            _user.SignOut(connectaccount);
            _connectedUsers.RemoveConnectUser(connectaccount, connectid);

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
