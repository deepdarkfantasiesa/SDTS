using Microsoft.AspNetCore.SignalR;
using Models;
using SDTS.BackEnd.Hubs;
using SDTS.DataAccess.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace SDTS.BackEnd
{
    public class EmergencyTimers: IEmergencyTimers
    {
        private IMockData mock;
        private IHubContext<DataHub> hubContext;
        private readonly IEmergencyHelpersRepository _emergencyHelpers;
        private readonly IUserRepository _user;
        private readonly IConnectedUsersRepository _connectedUsers;
        private readonly IUserDataRepository _userData;

        public EmergencyTimers(IMockData data, 
            IHubContext<DataHub> hub, 
            IEmergencyHelpersRepository emergencyHelpers,
            IUserRepository user,
            IConnectedUsersRepository connectedUsers,
            IUserDataRepository userData)
        {
            mock = data;
            hubContext = hub;
            _emergencyHelpers = emergencyHelpers;
            _user = user;
            _connectedUsers = connectedUsers;
            _userData = userData;
        }
        

        private Timer Emergencytimer;
        public void Init()
        {
            if (Emergencytimer != null)
                return;
            Emergencytimer = new Timer();
            Emergencytimer.Interval = 5000;
            Emergencytimer.Elapsed += EmergencyEvent;
            Emergencytimer.AutoReset = true;
            Emergencytimer.Enabled = true;
        }
    

        public async void EmergencyEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            var helpers = mock.AllEmergencyHelpers();//获取所有求救事件
            foreach(var helper in helpers)
            {
                var guardians = mock.getguardians(helper.Account);//获取求救者的监护人

                List<string> guarvolun_connedctid = new List<string>();
                foreach (var guardian in guardians)//获取已登录的监护人
                {
                    if (mock.ReflashGuardians(guardian.Account) != null 
                        && helper.isPublished.Exists(p=>p== mock.ReflashGuardians(guardian.Account)) ==false)//若在isPublished中存在，则表示已经推送给该用户，则不再推送给ta
                    {
                        guarvolun_connedctid.Add(mock.ReflashGuardians(guardian.Account));
                    }

                }
                var vounteers = mock.FindConnectUserWithType("志愿者");
                foreach (var volunteer in vounteers)
                {   //获取符合条件的志愿者
                    if (Math.Abs(volunteer.Value.Latitude - helper.Latitude) <= 0.005 && Math.Abs(volunteer.Value.Longitude - helper.Longitude) <= 0.005
                        && helper.isPublished.Exists(p=>p==volunteer.Key)==false)//若在isPublished中存在，则表示已经推送给该用户，则不再推送给ta
                    {
                        guarvolun_connedctid.Add(volunteer.Key);
                    }
                }
                foreach (var connectionid in guarvolun_connedctid)//依次给符合条件的人推送求救信息
                {
                    await hubContext.Clients.Client(connectionid).SendAsync("loadhelpers", helper);
                    //Debug.WriteLine("success");
                    helper.isPublished.Add(connectionid);
                }

                //foreach(var ispublished in helper.isPublished)
                //{
                //    Debug.WriteLine(ispublished);
                //}
            }
        }

        public void InitTimer()
        {
            if (Emergencytimer != null)
                return;
            Emergencytimer = new Timer();
            Emergencytimer.Interval = 5000;
            Emergencytimer.Elapsed += EmergencyEvents;
            Emergencytimer.AutoReset = true;
            Emergencytimer.Enabled = true;
        }

        public async void EmergencyEvents(Object source, System.Timers.ElapsedEventArgs e)
        {
            var helpers =await _emergencyHelpers.GetAllEmergencyHelper();
            foreach(var helper in helpers)
            {
                var guardians = _user.GetGuardians(helper.Account);
                List<string> guardiansconnnectionids = new List<string>();
                foreach(var guardian in guardians)
                {
                    guardiansconnnectionids.Add(await _connectedUsers.QueryConnectUserAsync(guardian.Account));
                }
                var volunteers = _user.GetVolunteers().Result;
                List<string> volunteersconnnectionids = new List<string>();
                foreach(var volunteer in volunteers)
                {
                    volunteersconnnectionids.Add(await _connectedUsers.QueryConnectUserAsync(volunteer.Account));
                }
                foreach(var volunteersconnnectionid in volunteersconnnectionids)
                {
                    var volunteerdata = await _userData.QueryUserDatasAsync(volunteersconnnectionid);
                    if (Math.Abs(volunteerdata.Latitude-helper.Latitude) <= 0.005 && Math.Abs(volunteerdata.Longitude-helper.Longitude) <= 0.005)
                    {

                    }

                }

            }
        }

    }
}
