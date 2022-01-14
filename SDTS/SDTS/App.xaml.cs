using SDTS.Services;
using SDTS.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SDTS
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            //MainPage = new AppShell();

            MainPage =new NavigationPage(new SignInPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
