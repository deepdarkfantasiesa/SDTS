using Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SDTS.BackEnd
{
    public interface IMockData
    {
        public User getdetail(int userid);
        public List<User> getwards(int userid);

        public List<SecureArea> getareas(int wardid);

        public bool addarea(SecureArea secureArea);
        public bool deletearea(SecureArea secureArea);
        public bool alterarea(SecureArea secureArea);
        public SecureArea getarea(int areaid);
        public User FindUser(string account, string password);
        public string signup(User user);
        public User getuser(string useraccount);
        public int invite(string account, int code);
        public List<User> getguardians(string account);
        public bool addward(int guardianid, int code);
        public bool AddConnectUser(string account, string connectid);
        public bool RemoveConnectUser(string account, string connectid);
        public string ReflashGuardians(string guardianaccount);
        public bool removeward(int guardianid, int code,string account);
        public bool AddEmergencyHelpers(string account, double Latitude, double Longitude,string ConnectionId,string Problem);
        public bool RemoveEmergencyHelpers(string account);
        public Helpers FindEmergencyHelpers(string account);
        public int EmergencyHelpersCount();
        public List<Helpers> AllEmergencyHelpers();
        public bool AddConnectUserData(string connectid, SensorData data);
        public bool AlterConnectUserData(string connectid, SensorData data);
        public bool RemoveConnectUserData(string connectid);
        public List<KeyValuePair<string, SensorData>> FindConnectUserWithType(string type);

        public string FindConnectedUser(string account);

        public bool CreateRescuerGroup(string groupname, string helperaccount);

        public bool AddRescuer(string rescueraccount, string helperaccount);
        public string AddRescuerInGroup(string rescueraccount, string helperaccount);
        public string FindRescuerGroup(string helperaccount);
    }
}
