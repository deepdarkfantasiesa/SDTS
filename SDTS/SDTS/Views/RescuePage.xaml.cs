using Models;
using SDTS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.Xaml;

namespace SDTS.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RescuePage : ContentPage
    {
        RescueViewModel _viewModel;

        public RescuePage(Helpers helper)
        {
            InitializeComponent();

            BindingContext = _viewModel = new RescueViewModel();
            _viewModel.helper = helper;

            MapSpan mapSpan = MapSpan.FromCenterAndRadius(new Position(helper.Latitude, helper.Longitude), Distance.FromKilometers(0));
            map.MoveToRegion(mapSpan);
        }

        //public RescuePage(SensorsData data)
        //{
        //    InitializeComponent();

        //    BindingContext = _viewModel = new RescueViewModel();
        //    //_viewModel.helper = helper;

        //    MapSpan mapSpan = MapSpan.FromCenterAndRadius(new Position(data.Latitude, data.Longitude), Distance.FromKilometers(0));
        //    map.MoveToRegion(mapSpan);
        //}

        public RescuePage(EmergencyHelper helper)
        {
            InitializeComponent();

            BindingContext = _viewModel = new RescueViewModel();
            _viewModel.Ehelper = helper;
            _viewModel.rescuePage = this;
            MapSpan mapSpan = MapSpan.FromCenterAndRadius(new Position(helper.Latitude, helper.Longitude), Distance.FromKilometers(0));
            map.MoveToRegion(mapSpan);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            _viewModel.LoadRescueGroup.Execute(null);
            var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            if (status == PermissionStatus.Granted)
            {
                return;
            }

            await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }

    }
}