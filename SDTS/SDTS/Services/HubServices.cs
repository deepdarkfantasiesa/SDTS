using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SDTS.Services
{
    public class HubServices
    {
        bool IsConnected { get; set; }
        HubConnection hubConnection;
        Random random;
        public Command ConnectCommand { set; get; }

        //const string ServerUrl = "http://192.168.50.113:24082";

     

        public void Init(string host)
        {
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

        }

        public async Task ConnectAsync()
        {
            if (IsConnected)
                return;

            await hubConnection.StartAsync();
            IsConnected = true;
            await hubConnection.InvokeAsync("ConnectToHub");
        }

        public async Task DisConnectAsync()
        {
            if (!IsConnected)
                return;

            await hubConnection.InvokeAsync("DisConnectToHub");
            await hubConnection.StopAsync();
            IsConnected = false;
        }

        public async Task SendMessage()
        {
            if (!IsConnected)
                throw new InvalidOperationException("Not connected");
            await hubConnection.InvokeAsync("SendMessage","123");
        }

    }
}
