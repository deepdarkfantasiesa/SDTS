using SDTS.ENotification;
using SDTS.GuardianViews;
using SDTS.Services;
using SDTS.Views;
using SDTS.WardsViews;
using System;
using Xamarin.Forms;

namespace SDTS
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        INotificationManager notificationManager;

        public AppShell()
        {
            InitializeComponent();
     
            Routing.RegisterRoute(nameof(PersonalInformationPage), typeof(PersonalInformationPage));

            if (GlobalVariables.user.Type.Equals("监护人"))
            {
                Routing.RegisterRoute(nameof(WardsDetailPage), typeof(WardsDetailPage));
                Routing.RegisterRoute(nameof(ManageWardsPage), typeof(ManageWardsPage));
                Routing.RegisterRoute(nameof(GlobalViewPage), typeof(GlobalViewPage)); 
                Routing.RegisterRoute(nameof(EmergencyPage), typeof(EmergencyPage));
                globalview.FlyoutItemIsVisible = true;
                managewards.FlyoutItemIsVisible = true;
                emergencyinformation.FlyoutItemIsVisible = true;
            }
            else if (GlobalVariables.user.Type.Equals("志愿者"))
            {
                Routing.RegisterRoute(nameof(EmergencyPage), typeof(EmergencyPage));
                emergencyinformation.FlyoutItemIsVisible = true;
            }
            else if(GlobalVariables.user.Type.Equals("被监护人"))
            {
                Routing.RegisterRoute(nameof(ManageGuardianPage), typeof(ManageGuardianPage));
                manageguardian.FlyoutItemIsVisible = true;
            }


            notificationManager = DependencyService.Get<INotificationManager>();
            notificationManager.NotificationReceived += async (sender, eventArgs) =>
            {
                //监听接收通知
                var evtData = (NotificationEventArgs)eventArgs;
                //跳转到救援页面
                await Shell.Current.GoToAsync("//EmergencyPage");
            };

        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            HubServices hubServices = DependencyService.Get<HubServices>();
            if(hubServices.IssConnected==true)
            {
                await hubServices.DisConnectAsync();
            }
           
            GlobalVariables.token = null;
            GlobalVariables.user = null;
            Application.Current.MainPage =new NavigationPage(new SignInPage());
        }
    }
}
