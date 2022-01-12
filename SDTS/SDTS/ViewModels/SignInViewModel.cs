using Models;
using SDTS.Services;
using SDTS.Views;
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public Command signup => new Command(async () => 
        {
            await Shell.Current.GoToAsync($"SignUpPage");
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
            TokenString.token = null;
            await sign.Signin(UserName, PassWord);
            if(TokenString.token!=null)
            {
                await Shell.Current.GoToAsync($"TestPage");
            }
            else
            {
                //弹窗提示登陆失败
            }
            //Debug.WriteLine(TokenString.token);
        });
    }
}
