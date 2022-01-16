﻿using Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SDTS.BackEnd
{
    public class MockData:IMockData
    {
        //模拟用户表，将来三种角色可能要分成三个表
        ObservableCollection<User> users = new ObservableCollection<User>()
            {
                new User{UserID = 0,Name="cqf",Information="1",Account="0",PassWord="0",Birthday=DateTime.Now,Gender="male",PhoneNumber="13112627289",GuardianID=new List<int>(),Type="监护人" },
                new User{UserID = 1,Name="hjy",Information="1",Account="1",PassWord="1",Birthday=DateTime.Now,Gender="male",PhoneNumber="1",GuardianID=new List<int>(){ 0,5},Type="被监护人" },
                new User{UserID = 2,Name="ccc",Information="2",Account="2",PassWord="2",Birthday=DateTime.Now,Gender="female",PhoneNumber="2",GuardianID=new List<int>(),Type="志愿者" },
                new User{UserID = 3,Name="qqq",Information="3",Account="3",PassWord="3",Birthday=DateTime.Now,Gender="unknow",PhoneNumber="3",GuardianID=new List<int>(){ 0},Type="被监护人" },
                new User{UserID = 4,Name="fff",Information="4",Account="4",PassWord="4",Birthday=DateTime.Now,Gender="male",PhoneNumber="4",GuardianID=new List<int>(){ 5,0},Type="被监护人" },
                new User{UserID = 5,Name="cyq",Information="1234",Account="1234",PassWord="1234",Birthday=DateTime.Now,Gender="female",PhoneNumber="180003080888",GuardianID=new List<int>(),Type="监护人" }
            };

        Dictionary<string, int> Inviter = new Dictionary<string, int>();

        Dictionary<string, string> ConnectedUser = new Dictionary<string, string>();

        int i = 0;

        ObservableCollection<SecureArea> secureAreas = new ObservableCollection<SecureArea>();

        public bool AddConnectUser(string account,string connectid)
        {
            if (ConnectedUser.Where(p => p.Key == account).Count()!=0)
                return false;//已存在
            ConnectedUser.Add(account, connectid);
            if(ConnectedUser.Where(p => p.Key == account).FirstOrDefault().Value.Equals(connectid))
                return true;//添加成功
            else
                return false;
        }

        public bool RemoveConnectUser(string account,string connectid)
        {
            ConnectedUser.Remove(account);
            if (ConnectedUser.Where(p => p.Value == connectid).Count() == 0)
                return true;
            else
                return false;
        }

        public bool addward(int guardianid,int code)
        {

            var wardaccount= Inviter.Where(p => p.Value == code).FirstOrDefault().Key;
            if(wardaccount==null)
            {
                return false;//邀请码不正确
            }

            if(users.Where(p => p.Account == wardaccount).FirstOrDefault().GuardianID.Where(i => i == guardianid).Count()!=0)
            {
                return false;//已存在此被监护人
            }

            users.Where(p => p.Account == wardaccount).FirstOrDefault().GuardianID.Add(guardianid);
            var result= users.Where(p => p.Account == wardaccount).FirstOrDefault().GuardianID.Where(i => i == guardianid).Count();
            if(result!=0)
            {
                Inviter.Remove(wardaccount);
                return true;//添加成功
            }
            return false;//未知错误
        }

        public int invite(string account,int code)
        {
            if(Inviter.Where(p=>p.Key==account).Count()!=0)
            {
                Inviter[account] = code;
            }
            else
            {
                Inviter.Add(account, code);
            }
            var num= Inviter.Where(p => p.Key == account).FirstOrDefault().Value;
            return num;
        }
        public List<User> getguardians(string account)
        {
            var ward = users.Where(p => p.Account == account).FirstOrDefault();
            List<User> guars = new List<User>();
            foreach(var id in ward.GuardianID)
            {
                guars.Add(users.Where(p => p.UserID == id).FirstOrDefault());
            }
            return guars;
        }


        public List<SecureArea> getareas(int wardid)
        {
            return secureAreas.ToList().FindAll(p=>p.wardid==wardid.ToString());
        }

        public SecureArea getarea(int areaid)
        {
            return secureAreas.ToList().Find(p => p.id == areaid);
        }

        public bool addarea(SecureArea secureArea)
        {
            secureAreas.Add(secureArea);
            return true;
        }

        public bool deletearea(SecureArea secureArea)
        {
            //var res = secureAreas.ToList().Remove(secureArea);
            var res = secureAreas.Remove(secureAreas.Where(p=>p.id==secureArea.id).FirstOrDefault());
            return res;
        }

        public bool alterarea(SecureArea newarea)
        {
            var oldarea=secureAreas.ToList().Find(p => p.id == newarea.id);
            alter(oldarea, newarea);
            //这里使用equals和==无法返回正确的结果，所以默认修改必成功
            return true;
        }
        private void alter(SecureArea oldarea,SecureArea newarea)
        {
            oldarea.information = newarea.information;
            oldarea.status = newarea.status;
            oldarea.createtime = newarea.createtime;
            oldarea.creatername = newarea.creatername;
            oldarea.createrid = newarea.createrid;
            oldarea.area = newarea.area;
        }

        public User getuser(string useraccount)
        {
            return users.FirstOrDefault(p => p.Account == useraccount);
        }

        public User getdetail(int userid)
        {
            return users.FirstOrDefault(p => p.UserID == userid);
        }

        //public ObservableCollection<User> getwards(int userid)
        public List<User> getwards(int userid)
        {
            i++;
            //var wardd= wards.ToList().FindAll(p => p.GuardianID.Find(userid)==userid);
            //var wardd = users.ToList().FindAll(p=>p.GuardianID.Find(i=>i==userid)== userid);
            var wardd = users.ToList().FindAll(p=>p.Type== "被监护人");
            var wards = wardd.FindAll(p=>p.GuardianID.Find(i=>i==userid)==userid);
            return wards;
        }

        public User FindUser(string account,string password)
        {
            return users.Where(p => p.Account == account && p.PassWord == password).FirstOrDefault();
        }

        public string signup(User user)
        {
            //账号不能重复
            if(users.Where(p => p.Account == user.Account).FirstOrDefault()==null)
            {
                user.UserID = users.Count + 1;
                users.Add(user);
            }
            else
            {
                return "账号与其他用户的冲突";
            }
            if(users.Where(p=>p==user).FirstOrDefault()!=null)
            {
                return "注册成功";
            }
            return "注册失败";
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