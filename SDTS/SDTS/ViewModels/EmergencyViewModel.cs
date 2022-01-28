using Models;
using SDTS.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Microsoft.AspNetCore.SignalR.Client;
using Xamarin.Essentials;
using Xamarin.Forms.GoogleMaps;
//using Xamarin.Forms.Maps;

namespace SDTS.ViewModels
{
    public class EmergencyViewModel: BasesViewModel
    {
        private string userId;
        private string name;
        private string information;
        private string gender;
        private string age;

        //public User helper;

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



                if (GlobalVariables.helpers==null)
                {
                    return;
                }
                var wards = GlobalVariables.helpers;

                

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
                    Pin.Icon = BitmapDescriptorFactory.DefaultMarker(_colors[0].Item2);
                    Pins?.Add(Pin);
                    var helper = (Helpers)Pin.Tag;
                    Debug.WriteLine(helper.Name + "\n" + helper.Information + "\n" + helper.Birthday + "\n" + helper.Gender);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
                //HubServices hubServices = DependencyService.Get<HubServices>();
                //if (hubServices.IsFirstOnpenGlobalView)
                //{
                //    hubServices.hubConnection.On<SensorData>("ReceiveEmergencyData", (data) =>
                //    {
                //        int index;
                //        if (PinsIndex.TryGetValue(data.user.Account, out index))
                //        {
                //            Position newlocation = new Position(data.Latitude, data.Longitude);
                //            MainThread.BeginInvokeOnMainThread(() =>
                //            {
                //                Pins[index].Position = newlocation;
                //                //Debug.WriteLine("\nLatitude:" + Pins[index].Position.Latitude+ "\nLongitude:" + Pins[index].Position .Longitude);
                //            });

                //            Request.MoveToRegion(
                //                MapSpan.FromCenterAndRadius(new Position(newlocation.Latitude,newlocation.Longitude),
                //                Distance.FromKilometers(0)),
                //true);
                //        }
                //        else
                //        {
                //            Debug.WriteLine("没有找到对应用户的索引");
                //        }

                //    });

                //    hubServices.IsFirstOnpenGlobalView = false;
                //}

            }
        }

        Helpers helper;

        public Command StartRescue => new Command(async () => {
            var result = await Application.Current.MainPage.DisplayAlert("警告", "您确定要开始救助此人吗", "确定", "取消");
            if (result.Equals(true))
            {
                HubServices hubServices = DependencyService.Get<HubServices>();

                await hubServices.hubConnection.InvokeAsync("JoinRescueGroup", GlobalVariables.user, helper);

                hubServices.hubConnection.On<SensorData>("ReceiveRescuerData", (data) =>
                {
                    foreach(var pin in Pins)
                    {
                        if(((Helpers)pin.Tag).Account==data.user.Account)
                        {
                            Pins[Pins.IndexOf(pin)].Position = new Position(data.Latitude, data.Longitude);
                        }
                    }
                });

                
            }
        });

        public Command GiveUpRescue => new Command(async () => {
            var result = await Application.Current.MainPage.DisplayAlert("警告", "您确定要放弃救助此人吗", "确定", "取消");
            if(result.Equals(true))
            {
                foreach(var pin in Pins)
                {
                    if(((Helpers)pin.Tag).Account.Equals(helper.Account))
                    {
                        Pins.Remove(pin);
                        GlobalVariables.helpers.Remove(helper);
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
            helper = (Helpers)((Pin)sender).Tag;
            Name = helper.Name;
            Information = helper.Information;
            Gender = helper.Gender;
            Age = (DateTime.Now.Year- helper.Birthday.Year).ToString();
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
            helper = new Helpers();
            Name = "";
            Information = "";
            Gender = "";
            Age = "";
            RescueButton.IsEnabled = false;
            GiveUpButton.IsEnabled = false;
        }

        //向服务器请求被选中的被监护人的详细信息
        public async void LoadWardId(int userid)
        {
            try
            {
                //这里需要向服务器请求选中的用户数据
                CommunicateWithBackEnd getwards = new CommunicateWithBackEnd();
                var ward = await getwards.GetWardDetail(userid);

                Id = ward.UserID;
                Name = ward.Name;
                Information = ward.Information;
                Gender = ward.Gender;
                Age = (DateTime.Now.Year - ward.Birthday.Year).ToString();
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Ward");
            }
        }

        

    }
}
