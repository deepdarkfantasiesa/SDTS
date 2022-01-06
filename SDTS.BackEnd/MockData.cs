using Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SDTS.BackEnd
{
    public class MockData
    {
        ObservableCollection<User> wards = new ObservableCollection<User>()
            {
                new User{UserID = 1,Name="cqf",Information="1",Birthday=DateTime.Now,Gender="male",PhoneNumber="2"},
                new User{UserID = 2,Name="ccc",Information="2",Birthday=DateTime.Now,Gender="female",PhoneNumber="2"},
                new User{UserID = 3,Name="qqq",Information="3",Birthday=DateTime.Now,Gender="unknow",PhoneNumber="2"},
                new User{UserID = 4,Name="fff",Information="4",Birthday=DateTime.Now,Gender="male",PhoneNumber="2"}
            };

        public User getdetail(int userid)
        {
            return wards.FirstOrDefault(p => p.UserID == userid);
        }

        public ObservableCollection<User> getwards(int userid)
        {
            return wards;
        }

        public User newuser()
        {
            User user = new User()
            {
                UserID = 1,
                Birthday = DateTime.Now,
                Gender = "male",
                Type = "监护人",
                Information = "no",
                Name = "cqf"
            };
            return user;
        }
    }
}
