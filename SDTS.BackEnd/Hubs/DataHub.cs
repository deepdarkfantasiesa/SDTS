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
        IEmergencyTimers timers;
        private readonly IUserRepository _user;
        private readonly IConnectedUsersRepository _connectedUsers;
        private readonly IUserDataRepository _userData;
        private readonly IEmergencyHelpersRepository _emergencyHelpers;
        private readonly IRescureGroupRepository _rescureGroups;
        private readonly IIsPublishedsRepository _isPublisheds;
        private readonly ISecureAreaRepository _secureArea;
        public DataHub(IEmergencyTimers emergency, 
            IUserRepository user,
            IConnectedUsersRepository connectedUsers, 
            IUserDataRepository userData,
            IEmergencyHelpersRepository emergencyHelpers,
            IRescureGroupRepository rescureGroups,
            IIsPublishedsRepository isPublisheds,
            ISecureAreaRepository secureArea)
        {
            timers = emergency;
            _user = user;
            _connectedUsers = connectedUsers;
            _userData = userData;
            _emergencyHelpers = emergencyHelpers;
            _rescureGroups = rescureGroups;
            _isPublisheds = isPublisheds;
            _secureArea = secureArea;
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
            }

            if (type.Equals("志愿者") || type.Equals("监护人"))
            {
                var rescurer = await _rescureGroups.QueryRescurer(computeddata.Account);
                if(rescurer!=null)
                {
                    await Clients.Group(rescurer.GroupName).SendAsync("ReceiveOthersData", computeddata,type);
                }
            }
            await _userData.AlterUserDatasAsync(computeddata);
        }

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
            computedata.BarometerData = data.BarometerData;
            return computedata;
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
                }
                else if (bardata == 0)
                {
                    await Clients.Caller.SendAsync("FinishResult", false, new SensorsData() { Latitude = lat, Longitude = lon, dataBar = new List<double>() { 999 } }, helperaccount);
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
                    await Clients.Caller.SendAsync("FinishResult", true, rescuer, helperaccount);
                }
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

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();

            var connectaccount = Context.User.Claims.First(p => p.Type.Equals("Account")).Value;

            var connectid = Context.ConnectionId;

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
