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
            WardsAccount = new List<string>();

            Polygons = new ObservableCollection<Polygon>();
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

        List<string> WardsAccount { get; set; }

        public ObservableCollection<Polygon> Polygons { get; set; }
        List<SecureArea> secureAreas = new List<SecureArea>();

        async Task ExecuteLoadWardsCommand()
        {
            IsBusy = true;

            try
            {
                Polygons.Clear();
                secureAreas.Clear();
                CommunicateWithBackEnd getareas = new CommunicateWithBackEnd();

                secureAreas = await getareas.GetWardSecureAreaWithGA(GlobalVariables.user.Account);
                Polygon pol;
                foreach (var area in secureAreas)
                {
                    string[] latgroup = area.Latitude.Split(",");
                    string[] longroup = area.Longitude.Split(",");

                    pol = new Polygon();

                    for (int i = 0; i < latgroup.Length; i++)
                    {
                        pol.Positions.Add(new Position(double.Parse(latgroup[i]), double.Parse(longroup[i])));
                    }
                    pol.Clicked += Polygon_Clicked;
                    pol.StrokeWidth = 3f;
                    pol.IsClickable = true;
                    pol.Tag = area.areaid;
                    if (area.status)
                    {
                        pol.StrokeColor = Color.Green;
                        pol.FillColor = Color.FromRgba(255, 0, 0, 64);
                    }
                    else
                    {
                        pol.StrokeColor = Color.Black;
                        pol.FillColor = Color.FromRgba(126, 0, 0, 20);
                    }
                    Polygons.Add(pol);
                }




                HubServices hubServices = DependencyService.Get<HubServices>();
                if (hubServices.IsFirstOnpenGlobalView)
                {
                    hubServices.hubConnection.On<SensorsData>("ReceiveDataFromOthers", (data) =>
                    {
                        if (WardsAccount.Exists(p => p == data.Account).Equals(false))
                        {
                            WardsAccount.Add(data.Account);
                            Pin Pin = new Pin
                            {
                                Label = data.Name,
                                Tag = data,
                                Position = new Position(data.Latitude, data.Longitude)
                            };
                            Pin.Icon = BitmapDescriptorFactory.DefaultMarker(Color.Bisque);
                            Pins?.Add(Pin);
                        }
                        else
                        {
                            foreach (var pin in Pins)
                            {
                                if (((SensorsData)pin.Tag).Account == data.Account)
                                {
                                    Pins[Pins.IndexOf(pin)].Position = new Position(data.Latitude, data.Longitude);
                                }
                            }
                        }
                    });
                    hubServices.IsFirstOnpenGlobalView = false;
                    //Debug.WriteLine($"\n\n{hubServices.IsFirstOnpenGlobalView}\n\n");
                }

                    //Pins.Clear();
                    //PinsIndex.Clear();
                    //此处需要请求服务器返回与此监护人绑定的被监护人的信息，并遍历载入Wards集合中
                    //CommunicateWithBackEnd getwards = new CommunicateWithBackEnd();
                    //此处需要添加登录成功后解析token并将用户信息以全局变量的形式存储好，取出userid传入RefreshDataAsync
                    //var wards = await getwards.RefreshWardsDataAsync();
                    //Pin Pin;
                    //foreach (var ward in wards)
                    //{
                    //    Pin = new Pin
                    //    {
                    //        Label = ward.Name,
                    //        Tag=ward
                    //    };
                    //    //Pin.Icon = BitmapDescriptorFactory.DefaultMarker(_colors[0].Item2);
                    //    Pin.Icon = BitmapDescriptorFactory.DefaultMarker(Color.AliceBlue);
                    //    Pins?.Add(Pin);
                    //    PinsIndex.Add(ward.Account, Pins.Count - 1);
                    //}
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
                    //hubServices.hubConnection.On<SensorData>("ReceiveData", (data) =>
                    //{
                    //    int index;
                    //    if (PinsIndex.TryGetValue(data.user.Account, out index))
                    //    {
                    //        Position newlocation = new Position(data.Latitude, data.Longitude);
                    //        MainThread.BeginInvokeOnMainThread(() =>
                    //        {
                    //            Pins[index].Position = newlocation;
                    //            //Debug.WriteLine("\nLatitude:" + Pins[index].Position.Latitude+ "\nLongitude:" + Pins[index].Position .Longitude);
                    //        });
                    //    }
                    //    else
                    //    {
                    //        Debug.WriteLine("没有找到对应用户的索引");
                    //    }

                    //});

                    //hubServices.hubConnection.On<SensorsData>("ReceiveDataFromOthers", (data) =>
                    //{
                    //    int index;
                    //    if (PinsIndex.TryGetValue(data.Account, out index))
                    //    {
                    //        Position newlocation = new Position(data.Latitude, data.Longitude);
                    //        MainThread.BeginInvokeOnMainThread(() =>
                    //        {
                    //            Pins[index].Position = newlocation;
                    //            //Debug.WriteLine("\nLatitude:" + Pins[index].Position.Latitude+ "\nLongitude:" + Pins[index].Position .Longitude);
                    //        });
                    //    }
                    //    else
                    //    {
                    //        Debug.WriteLine("没有找到对应用户的索引");
                    //    }

                    //});

                    //hubServices.hubConnection.On<SensorsData>("ReceiveDataFromOthers", (data) =>
                    //{
                    //    if (WardsAccount.Exists(p => p == data.Account).Equals(false))
                    //    {
                    //        WardsAccount.Add(data.Account);
                    //        Pin Pin = new Pin
                    //        {
                    //            Label = data.Name,
                    //            Tag = data,
                    //            Position = new Position(data.Latitude, data.Longitude)
                    //        };
                    //        Pin.Icon = BitmapDescriptorFactory.DefaultMarker(Color.Bisque);
                    //        Pins?.Add(Pin);
                    //    }
                    //    else
                    //    {
                    //        foreach (var pin in Pins)
                    //        {
                    //            if (((SensorsData)pin.Tag).Account == data.Account)
                    //            {
                    //                Pins[Pins.IndexOf(pin)].Position = new Position(data.Latitude, data.Longitude);
                    //            }
                    //        }
                    //    }
                    //});

            //        hubServices.IsFirstOnpenGlobalView = false;
            //}

            

            }
        }

        private async void Polygon_Clicked(object sender, EventArgs e)
        {
            var pol = (Polygon)sender;//选中的安全区域

            //string action;
            //var se = secureAreas.Find(p => p.id == pol.ZIndex);//p.id改成p.areaid
            //var se = secureAreas.Find(p => int.Parse(p.areaid) == pol.ZIndex);
            var se = secureAreas.Find(p => p.areaid == pol.Tag.ToString());

            //bool res;
            //res = await Application.Current.MainPage.DisplayAlert("安全区域基本信息", "创建者：" + se.creatername + "\n被监护人：" + se.wardname + "\n创建时间：" + se.createtime + "\n说明：" + se.information + "\n状态：" + se.status, "编辑", "取消");
            await Application.Current.MainPage.DisplayAlert(
                "安全区域详情", 
                $"创建者：{se.creatername}\n被监护人：{ se.wardname}\n创建时间：{se.createtime}\n说明：{se.information}", 
                "返回");

        }



    }
}
