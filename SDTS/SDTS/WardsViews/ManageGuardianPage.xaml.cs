using SDTS.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SDTS.WardsViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ManageGuardianPage : ContentPage
    {
        ManageGuardianViewModel _viewModel;
        public ManageGuardianPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new ManageGuardianViewModel();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
            var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            if (status == PermissionStatus.Granted)
            {
                return;
            }

            await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }
    }
}