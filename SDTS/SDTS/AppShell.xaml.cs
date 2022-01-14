using SDTS.GuardianViews;
using SDTS.ViewModels;
using SDTS.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace SDTS
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            //Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            //Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
            //Routing.RegisterRoute(nameof(SignInPage), typeof(SignInPage));
            //Routing.RegisterRoute("SignUpPage", typeof(SignUpPage));
            //Routing.RegisterRoute("TestPage", typeof(TestPage));
            Routing.RegisterRoute(nameof(PersonalInformationPage), typeof(PersonalInformationPage));
            Routing.RegisterRoute(nameof(WardsDetailPage), typeof(WardsDetailPage));
            Routing.RegisterRoute(nameof(ManageWardsPage), typeof(ManageWardsPage));
            Routing.RegisterRoute(nameof(AddWardPage), typeof(AddWardPage));
            
        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            //await Shell.Current.GoToAsync("//LoginPage");
            //await Application.Current.MainPage.Navigation.PushAsync(new SignInPage());
            GlobalVariables.token = null;
            GlobalVariables.user = null;
            Application.Current.MainPage =new NavigationPage(new SignInPage());
        }
    }
}
