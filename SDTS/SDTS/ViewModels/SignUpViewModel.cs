using Models;
using SDTS.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;
using static Models.User;

namespace SDTS.ViewModels
{
    public class SignUpViewModel : INotifyPropertyChanged
    {
        string username;
        public string UserName
        {
            get
            {
                return username;
            }
            set
            {
                username = value;
                OnPropertyChanged("username");
            }
        }

        string account;
        public string Account
        {
            get
            {
                return account;
            }
            set
            {
                account = value;
                OnPropertyChanged("account");
            }
        }

        string password;
        public string PassWord
        {
            get
            {
                return password;
            }
            set
            {
                password = value;
                OnPropertyChanged("password");
            }
        }

        string information;
        public string Information
        {
            get
            {
                return information;
            }
            set
            {
                information = value;
                OnPropertyChanged("information");
            }
        }

        string gender;
        public string Gender
        {
            get
            {
                return gender;
            }
            set
            {
                gender = value;
                OnPropertyChanged("gender");
            }
        }

        string phonenumber;
        public string PhoneNumber
        {
            get
            {
                return phonenumber;
            }
            set
            {
                phonenumber = value;
                OnPropertyChanged("phonenumber");
            }
        }

        string type;
        public IList<string> Types
        {
            get
            {
                return UserTypes.types;
            }
        }
        public string SelectedType
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
                OnPropertyChanged("type");
            }
        }

        DateTime birthday;
        public DateTime Birthday
        {
            get
            {
                return birthday;
            }
            set
            {
                birthday = value;
                OnPropertyChanged("birthday");
            }
        }

        public Command Register => new Command(async () => 
        {
            //Sign sign = new Sign();//321
            CommunicateWithBackEnd sign = new CommunicateWithBackEnd();
            User user = new User();
            user.Name = UserName;
            user.Account = Account;
            user.PassWord = PassWord;
            user.Information = Information;
            user.PhoneNumber = PhoneNumber;
            user.Type = SelectedType;
            user.Birthday = Birthday;
            user.Gender = Gender;
            var result=await sign.Signup(user);
            if (result.Equals("注册成功"))
            {
                //待添加，弹窗提示注册成功
                //await Shell.Current.GoToAsync("../");
                await Application.Current.MainPage.Navigation.PopToRootAsync();
            }
            //待添加，弹窗提示注册失败
        });

        //没有get,OnPropertyChanged("emergencycontacts")可能有问题
        //List<string> emergencycontacts;
        //public string EmergencyContacts 
        //{ 
        //    //get
        //    //{
        //    //    return emergencycontacts;
        //    //}
        //    set
        //    {
        //        emergencycontacts.Add(value);
        //        OnPropertyChanged("emergencycontacts");
        //    }
        //}

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
