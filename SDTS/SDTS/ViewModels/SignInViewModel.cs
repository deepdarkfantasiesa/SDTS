using Models;
using SDTS.Services;
using SDTS.Views;
using SDTS.VolunteerViews;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xamarin.Forms;


namespace SDTS.ViewModels
{
    class SignInViewModel:INotifyPropertyChanged
    {
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public Command signup => new Command(async () => 
        {
            //await Shell.Current.GoToAsync($"SignUpPage");
            await Application.Current.MainPage.Navigation.PushAsync(new SignUpPage());
        });

        public Command forgetpwd => new Command(async () => 
        {

        });

        public Command signin => new Command(async () => 
        {

            //Debug.WriteLine("username:"+username);
            //Debug.WriteLine("password:" + password);

            //Sign sign = new Sign();//321
            CommunicateWithBackEnd sign = new CommunicateWithBackEnd();
            /*
            GlobalVariables.token = null;
            GlobalVariables.user = null;
            */
            await sign.Signin(Account, PassWord);
            await sign.GetUserInfo();
            //监护人暂时无法登录
            if (GlobalVariables.token != null && GlobalVariables.user != null)
            {
                //await Shell.Current.GoToAsync($"TestPage");


                //判断用户类型，返回到指定的页面
                if(GlobalVariables.user.Type.Equals("监护人"))
                {

                    Application.Current.MainPage = new AppShell();
                }
                else if(GlobalVariables.user.Type.Equals("志愿者"))
                {
                    Application.Current.MainPage = new VolunteerFlyoutPageFlyout();
                }
                else if (GlobalVariables.user.Type.Equals("被监护人"))
                {

                }
            }
            else
            {
                //弹窗提示登陆失败
            }
            //Debug.WriteLine(TokenString.token);
        });
    }
}
