using SDTS.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.Xaml;

namespace SDTS.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GlobalViewPage : ContentPage
    {
        GlobalViewViewModel _viewModel;
        public GlobalViewPage()
        {
            InitializeComponent();

            
            MapSpan mapSpan = MapSpan.FromCenterAndRadius(new Position(22.3254973, 114.1671742), Distance.FromKilometers(0));
            map.MoveToRegion(mapSpan);
            BindingContext = _viewModel = new GlobalViewViewModel();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
           
            _viewModel.LoadWardsCommand.Execute(null);
            var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            if (status == PermissionStatus.Granted)
            {
                return;
            }

            await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }
    }
}