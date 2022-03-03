using SDTS.ENotification;
using SDTS.Services;
using SDTS.Views;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SDTS
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            //DependencyService.Register<MockDataStore>();
            //MainPage = new AppShell();
            DependencyService.Get<INotificationManager>().Initialize();

            MainPage =new NavigationPage(new SignInPage());
        }



        protected async override void OnStart()
        {
            base.OnStart();
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status == PermissionStatus.Granted)
            {
                return;
            }

            await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

            var sensorsstatus = await Permissions.CheckStatusAsync<Permissions.Sensors>();
            if (sensorsstatus == PermissionStatus.Granted)
            {
                return;
            }

            await Permissions.RequestAsync<Permissions.Sensors>();
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
