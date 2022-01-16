using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
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

        

        public async Task SendMessageToGuardian(string message)
        {
            var wardaccount = Context.User.Claims.First(p => p.Type.Equals("Account")).Value;

            var guardians = mock.getguardians(wardaccount);

            foreach(var guardian in guardians)
            {

                await Clients.All.SendAsync("GuardianReceive", message);
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
