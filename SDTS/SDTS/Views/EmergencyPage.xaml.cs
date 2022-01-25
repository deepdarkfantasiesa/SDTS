using Models;
using SDTS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SDTS.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EmergencyPage : ContentPage
    {
        EmergencyViewModel _viewModel;

        public EmergencyPage()
        {
            InitializeComponent();


            BindingContext = _viewModel = new EmergencyViewModel();

        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            _viewModel.LoadHelpersCommand.Execute(null);
            var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            if (status == PermissionStatus.Granted)
            {
                return;
            }

            await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }
    }
}