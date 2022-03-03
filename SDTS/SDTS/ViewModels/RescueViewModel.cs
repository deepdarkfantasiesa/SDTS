using Models;
using SDTS.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Microsoft.AspNetCore.SignalR.Client;
using SDTS.Views;

namespace SDTS.ViewModels
{
    public class RescueViewModel:BasesViewModel
    {
        public Command LoadRescueGroup { get; set; }
        public RescueViewModel()
        {
            LoadRescueGroup = new Command(async () => await ExecuteLoadRescueGroupCommand());

            Pins = new ObservableCollection<Pin>();

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

        public RescuePage rescuePage;

        public Command FinishRescue => new Command(async () => {
            var result = await Application.Current.MainPage.DisplayAlert("警告", "请确认结束此次救援", "确定", "取消");
            if (result.Equals(true))
            {
                HubServices hubServices = DependencyService.Get<HubServices>();

                hubServices.hubConnection.On<bool, SensorsData, string>("FinishResult", async (message, data, helperaccount) =>
                {

                    if (message.Equals(true))
                    {
                        var deletehelper = GlobalVariables.Ehelpers.Find(p => p.Account == helperaccount);
                        GlobalVariables.Ehelpers.Remove(deletehelper);
                        await Application.Current.MainPage.DisplayAlert("通知", "关闭成功", "完成");
                        await Application.Current.MainPage.Navigation.PopAsync();

                        hubServices.hubConnection.Remove("OthersFinishRescue");
                        hubServices.hubConnection.Remove("ReceiveOthersData");
                        hubServices.hubConnection.Remove("FinishResult");
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("通知", $"关闭失败 \nlat:{data.Latitude}\nlon:{data.Longitude}\nbar{data.BarometerData}", "返回");
                    }
                });

                await hubServices.hubConnection.InvokeAsync("UserFinishRescue", GlobalVariables.user.Account, Ehelper.Account);

                Debug.WriteLine($"\n\n{hubServices.IsFirstTimeFinishRescuerView}\n\n");

            }

        });

        public EmergencyHelper Ehelper { get; set; }

        List<SensorsData> Others { get; set; }

        async Task ExecuteLoadRescueGroupCommand()
        {
            IsBusy = true;

            try
            {
                HubServices hubServices = DependencyService.Get<HubServices>();


                await hubServices.hubConnection.InvokeAsync("UserJoinRescueGroup", GlobalVariables.user.Account, Ehelper.Account);

                hubServices.hubConnection.On<SensorsData, string>("ReceiveOthersData", (data, type) =>
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
                        if (((SensorsData)pin.Tag).Account == data.Account)
                        {
                            Pins[Pins.IndexOf(pin)].Position = new Position(data.Latitude, data.Longitude);
                            Debug.WriteLine("\n" + data.Name + "\n" + data.dateTime);
                        }
                    }
                    Debug.WriteLine(Pins.Count);
                });

                hubServices.hubConnection.On<string>("OthersFinishRescue", async (message) =>
                {
                    await Application.Current.MainPage.DisplayAlert("通知", message, "返回");
                    GlobalVariables.Ehelpers.Remove(Ehelper);

                    await Application.Current.MainPage.Navigation.PopAsync();

                    hubServices.hubConnection.Remove("OthersFinishRescue");
                    hubServices.hubConnection.Remove("ReceiveOthersData");
                    hubServices.hubConnection.Remove("FinishResult");
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
