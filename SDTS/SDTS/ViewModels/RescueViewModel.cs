using Models;
using SDTS.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Microsoft.AspNetCore.SignalR.Client;
using Xamarin.Essentials;

namespace SDTS.ViewModels
{
    public class RescueViewModel:BasesViewModel
    {
        public Command LoadRescueGroup { get; set; }
        public RescueViewModel()
        {
            LoadRescueGroup = new Command(async () => await ExecuteLoadRescueGroupCommand());

            Pins = new ObservableCollection<Pin>();

            Users = new List<SensorData>();
            Others = new List<SensorsData>();
        }

        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        public ObservableCollection<Pin> Pins
        {
            get;
            set;
        }

        public Command FinishRescue => new Command(async () => {
            var result = await Application.Current.MainPage.DisplayAlert("警告", "请确认结束此次救援", "确定", "取消");
            if (result.Equals(true))
            {
                HubServices hubServices = DependencyService.Get<HubServices>();

                hubServices.hubConnection.On<bool,SensorData>("FinishRescueResult", async (message,data) =>
                {

                    if (message.Equals(true))
                    {
                        await Application.Current.MainPage.DisplayAlert("通知", "关闭成功", "完成");
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("通知", $"关闭失败 \nlat:{data.Latitude}\nlon:{data.Longitude}\nbar{data.dataBar[0]}", "返回");
                    }
                });

                //await hubServices.hubConnection.InvokeAsync("VolunteerFinishRescue", GlobalVariables.user, helper);

                await hubServices.hubConnection.InvokeAsync("SomeBodyFinishRescue", GlobalVariables.user, Ehelper);
            }

        });


        public Helpers helper { get; set; }

        public EmergencyHelper Ehelper { get; set; }

        List<SensorData> Users { get; set; }

        List<SensorsData> Others { get; set; }

        async Task ExecuteLoadRescueGroupCommand()
        {
            IsBusy = true;

            try
            {
                HubServices hubServices = DependencyService.Get<HubServices>();

                //await hubServices.hubConnection.InvokeAsync("JoinRescueGroup", GlobalVariables.user, helper);
                await hubServices.hubConnection.InvokeAsync("UserJoinRescueGroup", GlobalVariables.user.Account, Ehelper.Account);

                hubServices.hubConnection.On<SensorsData,string>("ReceiveOthersData", (data,type) =>
                {
                    if (Others.Exists(p => p.Account == data.Account).Equals(false))
                    {
                        Others.Add(data);

                        Pin Pin = new Pin
                        {
                            Label = type + ":" + data.Name,
                            Tag = data,
                            Position = new Position(data.Latitude, data.Longitude)
                        };
                        if (data.Account == GlobalVariables.user.Account)
                        {
                            Pin.Icon = BitmapDescriptorFactory.DefaultMarker(Color.Green);
                        }
                        else if (type.Equals("被监护人"))
                        {
                            Pin.Icon = BitmapDescriptorFactory.DefaultMarker(Color.Red);
                        }
                        else if (type.Equals("监护人"))
                        {
                            Pin.Icon = BitmapDescriptorFactory.DefaultMarker(Color.Blue);
                        }
                        else if (type.Equals("志愿者"))
                        {
                            Pin.Icon = BitmapDescriptorFactory.DefaultMarker(Color.Yellow);
                        }
                        Pins?.Add(Pin);

                    }

                    foreach (var pin in Pins)
                    {
                        MainThread.BeginInvokeOnMainThread(() => {
                            if (((SensorsData)pin.Tag).Account == data.Account)
                            {
                                Pins[Pins.IndexOf(pin)].Position = new Position(data.Latitude, data.Longitude);
                                Debug.WriteLine(data.Name + "\n" + data.dateTime);
                            }
                        });
                    }
                });

                hubServices.hubConnection.On<SensorData>("ReceiveRescuerData", (data) =>
                {
                    if(Users.Exists(p=>p.user.Account==data.user.Account).Equals(false))
                    {
                        Users.Add(data);

                        Pin Pin = new Pin
                        {
                            Label =data.user.Type+":"+ data.user.Name,
                            Tag = data,
                            Position = new Position(data.Latitude, data.Longitude)
                        };
                        if (data.user.Account==GlobalVariables.user.Account)
                        {
                            Pin.Icon = BitmapDescriptorFactory.DefaultMarker(Color.Green);
                        }
                        else if(data.user.Type.Equals("被监护人"))
                        {
                            Pin.Icon = BitmapDescriptorFactory.DefaultMarker(Color.Red);
                        }
                        else if (data.user.Type.Equals("监护人"))
                        {
                            Pin.Icon = BitmapDescriptorFactory.DefaultMarker(Color.Blue);
                        }
                        else if (data.user.Type.Equals("志愿者"))
                        {
                            Pin.Icon = BitmapDescriptorFactory.DefaultMarker(Color.Yellow);
                        }
                        Pins?.Add(Pin);

                    }

                    foreach (var pin in Pins)
                    {
                        if (((Helpers)pin.Tag).Account == data.user.Account)
                        {
                            Pins[Pins.IndexOf(pin)].Position = new Position(data.Latitude, data.Longitude);
                            Debug.WriteLine(data.user.Name + "\n" + data.dateTime);
                        }
                    }
                });

                hubServices.hubConnection.On<string>("SomeBodyFinishRescue", (message) =>
                {
                    Application.Current.MainPage.DisplayAlert("通知", message, "返回");
                });



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

    }
}
