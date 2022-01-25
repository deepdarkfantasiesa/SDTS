﻿using Models;
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
using Xamarin.Forms.GoogleMaps.Bindings;

namespace SDTS.ViewModels
{
    public class EmergencyViewModel: BasesViewModel
    {
        private string userId;
        private string name;
        private string information;
        private string gender;
        private string age;

        public User helper;

        private Pin _pin;
        public Pin Pin//仅仅用于页面数据绑定，不用于Pin的移动
        {
            get => _pin;
            //get
            //{
            //    helper = (User)_pin.Tag;
            //    return _pin;

            //}
            set => SetProperty(ref _pin, value);
        }
        public ObservableCollection<Pin> Pins
        {
            get;
            set;
        }

        

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
            //set
            //{
            //    userId = value;
            //    LoadWardId(value);
            //}
            set => SetProperty(ref userId, value);
        }

        public string Gender
        {
            get => gender;
            set => SetProperty(ref gender, value);
        }

        public string Age
        {
            get => age;
            set => SetProperty(ref age, value);
        }

        public Command LoadHelpersCommand { get; }
        public EmergencyViewModel()
        {
            LoadHelpersCommand = new Command(async () => await ExecuteLoadWardsCommand());

        }

        public Dictionary<string, int> PinsIndex = new Dictionary<string, int>();

        readonly Tuple<string, Color>[] _colors =
        {
            new Tuple<string, Color>("Green", Color.Green),
            new Tuple<string, Color>("Pink", Color.Pink),
            new Tuple<string, Color>("Aqua", Color.Aqua)
        };

        public MoveToRegionRequest Request { get; } = new MoveToRegionRequest();
        async Task ExecuteLoadWardsCommand()
        {
            IsBusy = true;

            try
            {
                Pins.Clear();
                PinsIndex.Clear();


                //CommunicateWithBackEnd getwards = new CommunicateWithBackEnd();

                //var wards = await getwards.RefreshWardsDataAsync();

                if(GlobalVariables.helpers==null)
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
                        Tag = ward
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
                    hubServices.hubConnection.On<SensorData>("ReceiveEmergencyData", (data) =>
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

                            Request.MoveToRegion(
                                MapSpan.FromCenterAndRadius(new Position(newlocation.Latitude,newlocation.Longitude),
                                Distance.FromKilometers(0)),
                true);
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
