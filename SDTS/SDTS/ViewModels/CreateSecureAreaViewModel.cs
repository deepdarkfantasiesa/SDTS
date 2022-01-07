using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps.Bindings;
using Xamarin.Forms.GoogleMaps;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Xamarin.Essentials;
//using Xamarin.Forms.Maps;

namespace SDTS.ViewModels
{
    public class CreateSecureAreaViewModel:BasesViewModel
    {
        //public CreateSecureAreaViewModel(Map map)
        //{
        //    //这里的经纬度需要请求服务器返回被监护人当前的位置
        //    MapSpan mapSpan = MapSpan.FromCenterAndRadius(new Position(22, 114), Distance.FromKilometers(0));
        //    map.MoveToRegion(mapSpan);
        //    Pin.IsDraggable = true;
        //}

        private Pin _pin;
        public ObservableCollection<Pin> Pins
        {
            get;
            set;
        }
        public Pin Pin//仅仅用于页面数据绑定，不用于Pin的移动
        {
            get => _pin;
            set => SetProperty(ref _pin, value);
        }

        public Command<MapClickedEventArgs> MapClickedCommand => new Command<MapClickedEventArgs>(
            args =>
            {
                Pin = new Pin
                {
                    Label = $"test",
                    Position = args.Point,
                    IsDraggable = true
                };
                if(Pins.Count!=0)
                {
                    Pins.Clear();
                }
                Pins?.Add(Pin);
            });

        

        readonly Tuple<string, Color>[] _colors =
        {
            new Tuple<string, Color>("Green", Color.Green),
            new Tuple<string, Color>("Pink", Color.Pink),
            new Tuple<string, Color>("Aqua", Color.Aqua)
        };

        private List<Position> area;
        Polygon polygon;
        public Command<PinDragEventArgs> PinDragStartCommand => new Command<PinDragEventArgs>(
            args =>
            {
                area = new List<Position>();
                Pin = args.Pin;
                polygon = new Polygon();
                polygon.Positions.Add(args.Pin.Position);
            });

        public Command<PinDragEventArgs> PinDraggingCommand => new Command<PinDragEventArgs>(
            args =>
            {
                Pin = args.Pin;
                polygon.Positions.Add(args.Pin.Position);
            });

        public ObservableCollection<Polygon> Polygons { get; set; }
        int i = 0;
        public Command<PinDragEventArgs> PinDragEndCommand => new Command<PinDragEventArgs>(
            args =>
            {
                Pin = args.Pin;
                polygon.Positions.Add(args.Pin.Position);
                polygon.StrokeColor = Color.Green;
                polygon.StrokeWidth = 3f;
                polygon.FillColor = Color.FromRgba(255, 0, 0, 64);
                polygon.IsClickable = true;
                polygon.Tag = "POLYGON"+i++;
                polygon.Clicked += Polygon_Clicked;
                Polygons.Add(polygon);
                Pins.Clear();
            });

        private async void Polygon_Clicked(object sender, EventArgs e)
        {
            var pol = (Polygon)sender;
            Debug.WriteLine(Polygons[Polygons.IndexOf(pol)].Tag);
            //Polygons[Polygons.IndexOf(pol)]=pol;
            //await Application.Current.MainPage.DisplayAlert("Circle", $"{(string)pol.Tag} Clicked!", "Close");
            //string introduce= await Application.Current.MainPage.DisplayPromptAsync("Question 1", "What's your name?");
            //Debug.WriteLine(introduce);

        }
    }
}
