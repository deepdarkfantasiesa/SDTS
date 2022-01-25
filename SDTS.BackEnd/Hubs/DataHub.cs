using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Models;
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
        public DataHub(IMockData data)
        {
            mock = data;
            //var id = Context.User.Claims.First(p => p.Type.Equals("UserID")).Value;
        }
        public async Task SendSensorsDataToBackEnd(SensorData data)
        {
            var type = Context.User.Claims.First(p => p.Type.Equals("Type")).Value;

            //推送求救信息给志愿者
            if (mock.EmergencyHelpersCount()!=0&&type.Equals("志愿者"))
            {
                var helpers = mock.AllEmergencyHelpers();//返回还没有发布给志愿者求助信息的被监护人
                foreach(var helper in helpers)
                {
                    //若此时的非被监护人在求救者（存储在EmergencyHelpers）的方圆五百米之内，则向此非被监护人发布求救信息,注意：该后端已经向此被求助者的监护人发布求助信息（在PublishEmergencyInformationToGuardians中）
                    if (Math.Abs(helper.Latitude - data.Latitude)<=0.005&& Math.Abs(helper.Longitude - data.Longitude) <= 0.005)
                    {
                        await Clients.Client(Context.ConnectionId).SendAsync("loadhelpers", helper);
                    }


                }
            }

            if(type.Equals("被监护人"))
            {
                foreach (var acc in data.dataAcc)
                {
                    if (acc.Item1 * acc.Item1 + acc.Item2 * acc.Item2 + acc.Item3 * acc.Item3 > 3)
                    {
                        await PublishEmergencyInformationToGuardians(data.Latitude, data.Longitude);
                    }
                }
            }
            mock.AlterConnectUserData(Context.ConnectionId, data);
            //Debug.WriteLine(data.user.Name);
            //Debug.WriteLine(data.dataAcc.Count);
            //Debug.WriteLine(data.dataBar.Count);
            //Debug.WriteLine(data.dataGyr.Count);
            //Debug.WriteLine(data.dataMag.Count);
            //Debug.WriteLine(data.dataOri.Count);
            //Debug.WriteLine(data.dateTime);
            //Debug.WriteLine(data.Latitude);
            //Debug.WriteLine(data.Longitude);
        }

        //推送求救信息给监护人
        public async Task PublishEmergencyInformationTimer(SensorData data)
        {
            var wardaccount = Context.User.Claims.First(p => p.Type.Equals("Account")).Value;
            
            if (mock.FindEmergencyHelpers(wardaccount) == null)
            {
                mock.AddEmergencyHelpers(wardaccount, data.Latitude, data.Longitude, Context.ConnectionId, "滑倒");
            }
            else
            {
                return;
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

            //await Clients.All.SendAsync("wardreceive", data);
            //Debug.WriteLine($"Latitude: {data.Latitude} Longitude:{data.Longitude}");
            //Debug.WriteLine($"2");
        }

        public async Task SendMessageToGuardian(string message)
        {
            var wardaccount = Context.User.Claims.First(p => p.Type.Equals("Account")).Value;

            var guardians = mock.getguardians(wardaccount);//获取该被监护人的监护人账号

            List<string> connectguardianids = new List<string>();

            foreach(var guardian in guardians)
            {
                var connectguardianid = mock.ReflashGuardians(guardian.Account);
                if (connectguardianid != null)
                    connectguardianids.Add(connectguardianid);//将已连接的监护人连接id存起来
            }

            foreach(var connectguardianid in connectguardianids)
            {
                await Clients.Client(connectguardianid).SendAsync("GuardianReceive", message);//向已连接的监护人发送被监护人的数据
            }
        }

        public async Task ConnectToHub()
        {
            var connectaccount = Context.User.Claims.First(p => p.Type.Equals("Account")).Value;

            var connectid = Context.ConnectionId;

            if(mock.AddConnectUser(connectaccount, connectid))
                await Clients.Client(connectid).SendAsync("Entered", "connect success!");
            else
                await Clients.Client(connectid).SendAsync("Entered", "connect success but fail to add from Dictionary，may be you are already connect!");
        }
        
        public async Task DisConnectToHub()
        {
            var connectaccount = Context.User.Claims.First(p => p.Type.Equals("Account")).Value;

            var connectid = Context.ConnectionId;

            if(mock.RemoveConnectUser(connectaccount,connectid))
                await Clients.Client(connectid).SendAsync("Lefted", "disconnect success!");
            else
                await Clients.Client(connectid).SendAsync("Lefted", "disconnect success but fail to remove from Dictionary!!");
        }

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
            await base.OnDisconnectedAsync(exception);

        }

        public Task ThrowException()
        {
            throw new HubException("This error will be sent to the client!");
        }

    }
}
