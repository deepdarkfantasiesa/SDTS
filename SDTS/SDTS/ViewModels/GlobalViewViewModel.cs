using SDTS.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Microsoft.AspNetCore.SignalR.Client;
using Models;

namespace SDTS.ViewModels
{
    public class GlobalViewViewModel: BasesViewModel
    {
        public ObservableCollection<Pin> Pins
        {
            get;
            set;
        }
 
        public Command LoadWardsCommand { get; }

        public GlobalViewViewModel()
        {
            LoadWardsCommand = new Command(async () => await ExecuteLoadWardsCommand());

            Pins = new ObservableCollection<Pin>();

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
                }

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

        private async void Polygon_Clicked(object sender, EventArgs e)
        {
            var pol = (Polygon)sender;//选中的安全区域

            var se = secureAreas.Find(p => p.areaid == pol.Tag.ToString());

            await Application.Current.MainPage.DisplayAlert(
                "安全区域详情", 
                $"创建者：{se.creatername}\n被监护人：{ se.wardname}\n创建时间：{se.createtime}\n说明：{se.information}", 
                "返回");

        }



    }
}
