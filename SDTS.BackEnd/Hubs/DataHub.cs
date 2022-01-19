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
            //var wardaccount = Context.User.Claims.First(p => p.Type.Equals("Account")).Value;

            //var guardians = mock.getguardians(wardaccount);//获取该被监护人的监护人账号

            //List<string> connectguardianids = new List<string>();

            //foreach (var guardian in guardians)
            //{
            //    var connectguardianid = mock.ReflashGuardians(guardian.Account);
            //    if (connectguardianid != null)
            //        connectguardianids.Add(connectguardianid);//将已连接的监护人连接id存起来
            //}

            //foreach (var connectguardianid in connectguardianids)
            //{
            //    await Clients.Client(connectguardianid).SendAsync("ReceiveData", data);//向已连接的监护人发送被监护人的数据
            //}

            //Debug.WriteLine($"Acc {data.dataAcc.Count} X:{data.dataAcc[0].Item1} Y:{data.dataAcc[0].Item2} Z:{data.dataAcc[0].Item3}");
            //Debug.WriteLine($"Bar {data.dataBar.Count} X:{data.dataBar[0]}");
            //Debug.WriteLine($"Gyr {data.dataGyr.Count} X:{data.dataGyr[0].Item1} Y:{data.dataGyr[0].Item2} Z:{data.dataGyr[0].Item3}");
            //Debug.WriteLine($"Mag {data.dataMag.Count} X:{data.dataMag[0].Item1} Y:{data.dataMag[0].Item2} Z:{data.dataMag[0].Item3}");
            //Debug.WriteLine($"Ori {data.dataOri.Count} X:{data.dataOri[0].Item1} Y:{data.dataOri[0].Item2} Z:{data.dataOri[0].Item3}");

            //Debug.WriteLine($"Acc {data.dataAcc.Count}");
            //Debug.WriteLine($"Bar {data.dataBar.Count}");
            //Debug.WriteLine($"Gyr {data.dataGyr.Count}");
            //Debug.WriteLine($"Mag {data.dataMag.Count}");
            //Debug.WriteLine($"Ori {data.dataOri.Count}");
            //Debug.WriteLine($"Time {data.dateTime}");
        }

        public async Task SendDataToGuardian(SensorData data)
        {
            var wardaccount = Context.User.Claims.First(p => p.Type.Equals("Account")).Value;

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
                await Clients.Client(connectguardianid).SendAsync("ReceiveData", data);//向已连接的监护人发送被监护人的数据
            }

            //Debug.WriteLine($"Latitude: {data.Latitude} Longitude:{data.Longitude}");
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
    }
}
