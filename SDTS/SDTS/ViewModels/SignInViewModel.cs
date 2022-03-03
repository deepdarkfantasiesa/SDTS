using SDTS.Services;
using SDTS.Views;
using System.ComponentModel;
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
            await Application.Current.MainPage.Navigation.PushAsync(new SignUpPage());
        });

        public Command forgetpwd => new Command(async () => 
        {

        });

        public Command signin => new Command(async () => 
        {
            CommunicateWithBackEnd sign = new CommunicateWithBackEnd();
        
            await sign.Signin(Account, PassWord);
            if(GlobalVariables.token!=null)
            {
                await sign.GetUserInfo();
            }
    
            if (GlobalVariables.user != null)
            {

                HubServices hubServices = DependencyService.Get<HubServices>();
                hubServices.Init(Constants.host);
                hubServices.ConnectCommand.Execute(null);

                Application.Current.MainPage = new AppShell();

            }
            else
            {
                //弹窗提示登陆失败
            }

        });
    }
}
