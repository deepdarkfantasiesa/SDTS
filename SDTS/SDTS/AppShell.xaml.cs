using SDTS.GuardianViews;
using SDTS.Services;
using SDTS.ViewModels;
using SDTS.Views;
using SDTS.WardsViews;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace SDTS
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        HubServices hubServices = new HubServices();
        public AppShell()
        {
            InitializeComponent();
            //Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            //Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
            //Routing.RegisterRoute(nameof(SignInPage), typeof(SignInPage));
            //Routing.RegisterRoute("SignUpPage", typeof(SignUpPage));
            //Routing.RegisterRoute("TestPage", typeof(TestPage));


            Routing.RegisterRoute(nameof(PersonalInformationPage), typeof(PersonalInformationPage));
            //Routing.RegisterRoute(nameof(WardsDetailPage), typeof(WardsDetailPage));
            //Routing.RegisterRoute(nameof(ManageWardsPage), typeof(ManageWardsPage));
            //Routing.RegisterRoute(nameof(AddWardPage), typeof(AddWardPage));
            //Routing.RegisterRoute(nameof(ManageGuardianPage), typeof(ManageGuardianPage));

            if(GlobalVariables.user.Type.Equals("监护人"))
            {
                Routing.RegisterRoute(nameof(AddWardPage), typeof(AddWardPage));

                Routing.RegisterRoute(nameof(WardsDetailPage), typeof(WardsDetailPage));
                Routing.RegisterRoute(nameof(ManageWardsPage), typeof(ManageWardsPage));
                managewards.FlyoutItemIsVisible = true;
            }
            else if (GlobalVariables.user.Type.Equals("志愿者"))
            {
                //managewards.FlyoutItemIsVisible = false;
            }
            else if(GlobalVariables.user.Type.Equals("被监护人"))
            {
                Routing.RegisterRoute(nameof(ManageGuardianPage), typeof(ManageGuardianPage));
                //managewards.FlyoutItemIsVisible = false;
                manageguardian.FlyoutItemIsVisible = true;
            }

            
            hubServices.Init(Constants.host);
            hubServices.ConnectCommand.Execute(null);
            //hubServices.SendMessage();
        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            //await Shell.Current.GoToAsync("//LoginPage");
            //await Application.Current.MainPage.Navigation.PushAsync(new SignInPage());
            GlobalVariables.token = null;
            GlobalVariables.user = null;
            await hubServices.DisConnectAsync();
            Application.Current.MainPage =new NavigationPage(new SignInPage());
        }
    }
}
