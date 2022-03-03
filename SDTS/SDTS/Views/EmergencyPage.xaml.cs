using SDTS.ViewModels;
using System.Diagnostics;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
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

            MapSpan mapSpan = MapSpan.FromCenterAndRadius(new Position(22.325494, 114.167338), Distance.FromKilometers(0));
            map.MoveToRegion(mapSpan);

            RescueButton.IsEnabled = false;
            GiveUpButton.IsEnabled = false;

            _viewModel.RescueButton = RescueButton;
            _viewModel.GiveUpButton = GiveUpButton;

        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            Debug.WriteLine("EmergencyPage OnAppearing!!");
            _viewModel.LoadHelpersCommand.Execute(null);
            _viewModel.MapClicked.Execute(null);
            var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            if (status == PermissionStatus.Granted)
            {
                return;
            }

            await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }
    }
}