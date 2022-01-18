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
using Models;
using Xamarin.Essentials;

namespace SDTS.ViewModels
{
    public class GlobalViewViewModel: BasesViewModel
    {
        public ObservableCollection<Pin> Pins
        {
            get;
            set;
        }
        //key是被监护人的账户，value是对应的Pin索引
        public Dictionary<string, int> PinsIndex = new Dictionary<string, int>();

        //HubServices hubServices;
        public Command LoadWardsCommand { get; }

        public GlobalViewViewModel()
        {
            LoadWardsCommand = new Command(async () => await ExecuteLoadWardsCommand());

            //LoadWardsCommand.Execute(null);

            Pins = new ObservableCollection<Pin>();

            PinsIndex = new Dictionary<string, int>();
        }

        public void OnAppearing()
        {
            IsBusy = true;
        }

        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
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
                PinsIndex.Clear();
                //WardStore dataStore = new WardStore();//123
                //此处需要请求服务器返回与此监护人绑定的被监护人的信息，并遍历载入Wards集合中
                //var wards = await dataStore.GetWardsAsync(true);
                CommunicateWithBackEnd getwards = new CommunicateWithBackEnd();
                //此处需要添加登录成功后解析token并将用户信息以全局变量的形式存储好，取出userid传入RefreshDataAsync
                var wards = await getwards.RefreshWardsDataAsync();
                Pin Pin;
                foreach (var ward in wards)
                {
                    Pin = new Pin
                    {
                        Label = ward.Name,
                        Tag=ward
                    };
                    Pin.Icon = BitmapDescriptorFactory.DefaultMarker(_colors[0].Item2);
                    Pins?.Add(Pin);
                    PinsIndex.Add(ward.Account, Pins.Count - 1);
                    
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
                HubServices hubServices = DependencyService.Get<HubServices>();
                if (hubServices.IsFirstOnpenGlobalView)
                {
                    hubServices.hubConnection.On<SensorData>("ReceiveData", (data) =>
                    {
                        int index;
                        if (PinsIndex.TryGetValue(data.user.Account, out index))
                        {
                            Position newlocation = new Position(data.Latitude, data.Longitude);
                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                Pins[index].Position = newlocation;
                                //Debug.WriteLine("\nLatitude:" + Pins[index].Position.Latitude+ "\nLongitude:" + Pins[index].Position .Longitude);
                            });
                        }
                        else
                        {
                            Debug.WriteLine("没有找到对应用户的索引");
                        }

                    });

                    hubServices.IsFirstOnpenGlobalView = false;
                }

            }
        }
    }
}
