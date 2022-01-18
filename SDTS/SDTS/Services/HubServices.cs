using Microsoft.AspNetCore.SignalR.Client;
using Models;
using SDTS.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
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

        //const string ServerUrl = "http://192.168.50.113:24082";

     

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
                    // Exception!
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
                hubConnection.On<string>("", (message) =>
                {
                    Debug.WriteLine(message);
                });
            }
        }


        public bool IsFirstOnpenGlobalView = true;

        //public void StartReceiveData()
        //{
        //    if (!isfirst)
        //    {
        //        hubConnection.On<string>("ReceiveData", (message) =>
        //        {
        //            Debug.WriteLine(message);
        //        });
        //        isfirst = true;
        //    }
        //}

        public async Task ConnectAsync()
        {
            if (IsConnected)
                return;

            await hubConnection.StartAsync();
            IssConnected = true;
            IsConnected = true;
            await hubConnection.InvokeAsync("ConnectToHub");
        }

        public async Task DisConnectAsync()
        {
            if (!IsConnected)
                return;

            await hubConnection.InvokeAsync("DisConnectToHub");
            await hubConnection.StopAsync();
            IssConnected = false;
            IsConnected = false;
        }

        

        System.Timers.Timer timer = new System.Timers.Timer();
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if(GlobalVariables.user.Type.Equals("被监护人"))
            {
                if (propertyName.Equals("isconnected"))
                {
                    if (IsConnected.Equals(true))
                    {
                        timer.Interval = 500;
                        timer.Elapsed += SendData;
                        timer.AutoReset = true;
                        timer.Enabled = true;
                    }
                    else if (IsConnected.Equals(false))
                    {
                        timer.Enabled = false;
                    }
                }
            }
            
            
        }

        public async void SendData(Object source, System.Timers.ElapsedEventArgs e)
        {
            //await SendMessageToGuardian();
            await SendDataToGuardian();
        }

        public async Task SendMessageToGuardian()
        {
            if (!IsConnected)
                throw new InvalidOperationException("Not connected");
            await hubConnection.InvokeAsync("SendMessageToGuardian", "123");
        }

        public async Task SendDataToGuardian()
        {
            if (!IsConnected)
                throw new InvalidOperationException("Not connected");

            var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromMilliseconds(1));
            CancellationTokenSource cts = new CancellationTokenSource();
            Location location = await Geolocation.GetLocationAsync(request, cts.Token);

            await hubConnection.InvokeAsync("SendDataToGuardian", new SensorData() { user=GlobalVariables.user,Latitude=location.Latitude,Longitude=location.Longitude});
        }

    }
}
