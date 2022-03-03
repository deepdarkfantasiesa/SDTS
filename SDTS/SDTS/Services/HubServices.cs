using Microsoft.AspNetCore.SignalR.Client;
using Models;
using SDTS.ENotification;
using SDTS.Sensors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SDTS.Services
{
    public class HubServices
    {
        public string ip;

        public bool IssConnected { get; private set; }

        bool isconnected;
        bool IsConnected 
        {
            get
            {
                return isconnected;
            }
            set 
            {
                isconnected = value;
                OnPropertyChanged("isconnected");
            } 
        }
        public HubConnection hubConnection;

        Random random;
        public Command ConnectCommand { get; set; }

        public void Init(string host)
        {
            ip = host;
            string url = $"http://{host}/hubs/data" + "?access_token=" + GlobalVariables.token;//填入主机IP和端口号
            hubConnection = new HubConnectionBuilder()
            .WithUrl(url)
            .Build();

            ConnectCommand = new Command(async () => await ConnectAsync());

            hubConnection.On<string>("Entered", (message) =>
            {
                Debug.WriteLine(message);
            });

            hubConnection.On<string>("Lefted", (message) =>
            {
                Debug.WriteLine(message);
            });

            hubConnection.Closed += async (error) =>
            {
                IssConnected = false;
                IsConnected = false;
                await Task.Delay(random.Next(0, 5) * 1000);
                try
                {
                    await ConnectAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            };

            if(GlobalVariables.user.Type.Equals("监护人"))
            {
                hubConnection.On<string>("GuardianReceive", (message) =>
                {
                    Debug.WriteLine(message);
                });
            }
            else if(GlobalVariables.user.Type.Equals("被监护人"))
            {
                hubConnection.On<string>("wardreceive", (message) =>
                {
                    Debug.WriteLine(message);
                });
            }
            if(GlobalVariables.user.Type.Equals("监护人") || GlobalVariables.user.Type.Equals("志愿者"))
            {
                hubConnection.On<EmergencyHelper>("loadhelpers", (user) =>
                {
                    if (GlobalVariables.Ehelpers == null)
                        GlobalVariables.Ehelpers = new List<EmergencyHelper>();
                    if (GlobalVariables.Ehelpers.Find(p => p.Account == user.Account) == null)
                    {
                        GlobalVariables.Ehelpers.Add(user);
                        Debug.WriteLine(user.Problem);
                        Debug.WriteLine(user.Name);

                        //发送通知
                        INotificationManager notificationManager;
                        notificationManager = DependencyService.Get<INotificationManager>();
                        string title = $"有人需要救助";
                        string message = user.Name + $"需要救助";
                        notificationManager.SendNotification(title, message);

                    }
                    else
                    {
                        Debug.WriteLine(user.Name);
                    }
                });

            }
        }

        public bool IsFirstOnpenGlobalView = true;
        public bool IsFirstOnpenRescuerView = true;
        public bool IsFirstTimeFinishRescuerView = true;

        public async Task ConnectAsync()
        {
            if (IsConnected)
                return;

            await hubConnection.StartAsync();
            IssConnected = true;
            IsConnected = true;
        }

        public async Task DisConnectAsync()
        {
            if (!IsConnected)
                return;

            await hubConnection.StopAsync();
            IssConnected = false;
            IsConnected = false;
        }


        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (propertyName.Equals("isconnected"))
            {
                System.Timers.Timer SensorsTimer = new System.Timers.Timer();
                if (IsConnected.Equals(true))
                {
                    readSensors = DependencyService.Get<ReadSensorsrData>();
                    readSensors.ToggleAccelerometer();

                    SensorsTimer.Interval = 1000;
                    SensorsTimer.Elapsed += SendSensorsData;
                    SensorsTimer.AutoReset = true;
                    SensorsTimer.Enabled = true;
                }
                else if (IsConnected.Equals(false))
                {
                    SensorsTimer.Enabled = false;
                    SensorsTimer.Elapsed -= SendSensorsData;
                    readSensors.ToggleAccelerometer();
                }
            }
        }

        ReadSensorsrData readSensors = null;
        public async void SendSensorsData(Object source, System.Timers.ElapsedEventArgs e)
        {
            await SendSensorsDataToGuardian();
        }
        public async Task SendSensorsDataToGuardian()
        {
            if (!IsConnected)
                throw new InvalidOperationException("Not connected");

            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromMilliseconds(1));
                CancellationTokenSource cts = new CancellationTokenSource();
                Location location = await Geolocation.GetLocationAsync(request, cts.Token);

                if(location!=null)
                {
                    await hubConnection.InvokeAsync("BackEndReceiveData", new SensorsData()
                    {
                        //把传感器数据全部读出来打包好发给后端
                        Account = GlobalVariables.user.Account,
                        Name=GlobalVariables.user.Name,
                        dataBar = readSensors.dataBar,
                        dataMag = readSensors.dataMag,
                        dataGyr = readSensors.dataGyr,
                        dataOri = readSensors.dataOri,
                        dateTime = DateTime.Now,
                        dataAcc = readSensors.dataAcc,
                        Latitude = location.Latitude,
                        Longitude = location.Longitude
                    });
                }
                else
                {
                    await hubConnection.InvokeAsync("BackEndReceiveData", new SensorsData()
                    {
                        //把传感器数据全部读出来打包好发给后端
                        Account = GlobalVariables.user.Account,
                        dataBar = readSensors.dataBar,
                        dataMag = readSensors.dataMag,
                        dataGyr = readSensors.dataGyr,
                        dataOri = readSensors.dataOri,
                        dateTime = DateTime.Now,
                        dataAcc = readSensors.dataAcc
                    });
                }
                readSensors.ClearData();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}
