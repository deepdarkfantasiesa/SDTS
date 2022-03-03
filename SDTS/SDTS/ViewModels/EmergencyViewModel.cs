using Models;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using SDTS.Views;

namespace SDTS.ViewModels
{
    public class EmergencyViewModel: BasesViewModel
    {
        private string userId;
        private string name;
        private string information;
        private string gender;
        private string age;


        private Pin _pin;
        public Pin Pin//仅仅用于页面数据绑定，不用于Pin的移动
        {
            get => _pin;
            set => SetProperty(ref _pin, value);
        }
        public ObservableCollection<Pin> Pins
        {
            get;
            set;
        }

        public Button RescueButton { get; set; }
        public Button GiveUpButton { get; set; }

        public int Id { get; set; }

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        public string Information
        {
            get => information;
            set => SetProperty(ref information, value);
        }

        public string UserId
        {
            get
            {
                return userId;
            }
            set => SetProperty(ref userId, value);
        }

        public string Gender
        {
            get => gender;
            set => SetProperty(ref gender, value);
        }

        public string Age
        {
            get { return age; }
            set => SetProperty(ref age, value);
        }

        public Command LoadHelpersCommand { get; }

     

        public EmergencyViewModel()
        {
            LoadHelpersCommand = new Command(async () => await ExecuteLoadWardsCommand());
            Pins = new ObservableCollection<Pin>();
        }

        readonly Tuple<string, Color>[] _colors =
        {
            new Tuple<string, Color>("Green", Color.Green),
            new Tuple<string, Color>("Pink", Color.Pink),
            new Tuple<string, Color>("Aqua", Color.Aqua)
        };

        async Task ExecuteLoadWardsCommand()
        {
            IsBusy = true;

            try
            {
                Pins.Clear();

                if (GlobalVariables.Ehelpers == null)
                {
                    return;
                }
                var wards = GlobalVariables.Ehelpers;



                Pin Pin;
                foreach (var ward in wards)
                {
                    Pin = new Pin
                    {
                        Label = ward.Name,
                        Tag = ward,
                        Position = new Position(ward.Latitude, ward.Longitude)
                    };
                    Pin.Clicked += PinClickedAsync;
                    Pin.Icon = BitmapDescriptorFactory.DefaultMarker(Color.Red);
                    Pins?.Add(Pin);
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        EmergencyHelper Ehelper;

        public Command StartRescue => new Command(async () => {
            var result = await Application.Current.MainPage.DisplayAlert("警告", "您确定要开始救助此人吗", "确定", "取消");
            if (result.Equals(true))
            {
                await Application.Current.MainPage.Navigation.PushAsync(new RescuePage(Ehelper));
            }
        });

        public Command GiveUpRescue => new Command(async () => {
            var result = await Application.Current.MainPage.DisplayAlert("警告", "您确定要放弃救助此人吗", "确定", "取消");
            if(result.Equals(true))
            {
                foreach(var pin in Pins)
                {
                    if (((EmergencyHelper)pin.Tag).Account.Equals(Ehelper.Account))
                    {
                        Pins.Remove(pin);
                        GlobalVariables.Ehelpers.Remove(Ehelper);
                        ResetView();
                        return;
                    }
                }
            }

        });

        public Command MapClicked => new Command(async () => {
            ResetView();
        });
        async void PinClickedAsync(object sender, EventArgs e)
        {
            Ehelper = (EmergencyHelper)((Pin)sender).Tag;
            Name = Ehelper.Name;
            Information = Ehelper.Information;
            Gender = Ehelper.Gender;
            Age = (DateTime.Now.Year - Ehelper.Birthday.Year).ToString();
            try
            {
                RescueButton.Text = "开始救援";
                GiveUpButton.IsEnabled = true;
                RescueButton.IsEnabled = true;
                RescueButton.BackgroundColor = Color.Green;
                GiveUpButton.BackgroundColor = Color.Red;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void ResetView()
        {
            RescueButton.Text = "请选择目标";
            Ehelper = new EmergencyHelper();
            Name = "";
            Information = "";
            Gender = "";
            Age = "";
            RescueButton.IsEnabled = false;
            GiveUpButton.IsEnabled = false;
        }
    }
}
