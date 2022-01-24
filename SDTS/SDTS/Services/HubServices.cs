using Microsoft.AspNetCore.SignalR.Client;
using Models;
using SDTS.Sensors;
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
                hubConnection.On<string>("wardreceive", (message) =>
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
            //await hubConnection.InvokeAsync("ConnectToHub");
        }

        public async Task DisConnectAsync()
        {
            if (!IsConnected)
                return;

            //await hubConnection.InvokeAsync("DisConnectToHub");
            await hubConnection.StopAsync();
            IssConnected = false;
            IsConnected = false;
        }

        

        //System.Timers.Timer timer = new System.Timers.Timer();
        

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if(GlobalVariables.user.Type.Equals("被监护人"))
            {
                if (propertyName.Equals("isconnected"))
                {
                    System.Timers.Timer timerGps = new System.Timers.Timer();
                    System.Timers.Timer timerSensors = new System.Timers.Timer();

                    if (IsConnected.Equals(true))
                    {   //gps一秒发两次,被监护人端退出登录时会报错，初步判断是定时器在传感器关闭后依然在跟后端通讯
                        timerGps.Interval = 500;
                        timerGps.Elapsed += SendGPSData;
                        timerGps.AutoReset = true;
                        timerGps.Enabled = true;

                        //其他传感器一秒发一次
                        timerSensors.Interval = 500;
                        timerSensors.Elapsed += SendSensorsData;
                        timerSensors.AutoReset = true;
                        timerSensors.Enabled = true;
                    }
                    else if (IsConnected.Equals(false))
                    {
                        

                        timerGps.Enabled = false;

                        timerSensors.Enabled = false;

                        timerGps.Elapsed -= SendGPSData;
                        timerSensors.Elapsed -= SendSensorsData;

                        readSensors.ToggleAccelerometer();
                    }
                }
            }
            
            
        }

        ReadSensorsrData readSensors = new ReadSensorsrData();
        public async void SendSensorsData(Object source, System.Timers.ElapsedEventArgs e)
        {
            await SendSensorsDataToGuardian();
        }
        public async Task SendSensorsDataToGuardian()
        {
            if (!IsConnected)
                throw new InvalidOperationException("Not connected");

            if (!Accelerometer.IsMonitoring)
            {
                //打开除了gps以外的传感器
                readSensors.ToggleAccelerometer();


            }

            await hubConnection.InvokeAsync("SendSensorsDataToBackEnd", new SensorData()
            {
                //把传感器数据全部读出来打包好发给后端
                user = GlobalVariables.user,
                //dataAcc = readSensors.dataAcc,
                dataBar = readSensors.dataBar,
                dataMag = readSensors.dataMag,
                dataGyr = readSensors.dataGyr,
                dataOri = readSensors.dataOri,
                dateTime = DateTime.Now,

                dataAcc = readSensors.dataAcc
            });
            //readSensors.dataAcc.Clear();
            readSensors.ClearData();
        }


        public async void SendGPSData(Object source, System.Timers.ElapsedEventArgs e)
        {
            //await SendMessageToGuardian();
            await SendGPSDataToGuardian();
        }
        public async Task SendGPSDataToGuardian()
        {
            if (!IsConnected)
                throw new InvalidOperationException("Not connected");

            //if(!Accelerometer.IsMonitoring)
            //    readAccelerometer.ToggleAccelerometer();

            var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromMilliseconds(1));
            CancellationTokenSource cts = new CancellationTokenSource();
            Location location = await Geolocation.GetLocationAsync(request, cts.Token);

            await hubConnection.InvokeAsync("SendDataToGuardian", new SensorData()
            {
                user = GlobalVariables.user,
                Latitude = location.Latitude,
                Longitude = location.Longitude
            });
        }



        public async Task SendMessageToGuardian()
        {
            if (!IsConnected)
                throw new InvalidOperationException("Not connected");
            await hubConnection.InvokeAsync("SendMessageToGuardian", "123");
        }

        
        

    }
}
