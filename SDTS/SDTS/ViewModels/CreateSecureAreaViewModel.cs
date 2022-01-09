using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps.Bindings;
using Xamarin.Forms.GoogleMaps;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Xamarin.Essentials;
using Models;
using System.Threading.Tasks;
using SDTS.Services;
//using Xamarin.Forms.Maps;

namespace SDTS.ViewModels
{
    //[QueryProperty(nameof(WardId), nameof(WardId))]
    public class CreateSecureAreaViewModel:BasesViewModel
    {
        //public CreateSecureAreaViewModel(Map map)
        //{
        //    //这里的经纬度需要请求服务器返回被监护人当前的位置
        //    MapSpan mapSpan = MapSpan.FromCenterAndRadius(new Position(22, 114), Distance.FromKilometers(0));
        //    map.MoveToRegion(mapSpan);
        //    Pin.IsDraggable = true;
        //}
        public CreateSecureAreaViewModel()
        {

        }
        public CreateSecureAreaViewModel(string warid,string wardname)
        {
            WardId = warid;
            WardName = wardname;
        }

        private string wardid;
        public string WardId
        {
            get
            {
                return wardid;
            }
            set
            {
                wardid = value;
                //LoadWardId(value);
            }
        }

        private string wardname;
        public string WardName
        {
            get
            {
                return wardname;
            }
            set
            {
                wardname = value;
                //LoadWardId(value);
            }
        }

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

                area.Add(args.Pin.Position);
            });

        public Command<PinDragEventArgs> PinDraggingCommand => new Command<PinDragEventArgs>(
            args =>
            {
                Pin = args.Pin;
                polygon.Positions.Add(args.Pin.Position);

                area.Add(args.Pin.Position);
            });

        public ObservableCollection<Polygon> Polygons { get; set; }
        List<SecureArea> secureAreas = new List<SecureArea>();
        
        public Command<PinDragEventArgs> PinDragEndCommand => new Command<PinDragEventArgs>(
            args =>
            {
                Pin = args.Pin;
                polygon.Positions.Add(args.Pin.Position);
                polygon.StrokeColor = Color.Green;
                polygon.StrokeWidth = 3f;
                polygon.FillColor = Color.FromRgba(255, 0, 0, 64);
                polygon.IsClickable = true;
                polygon.Tag = "POLYGON";
                polygon.Clicked += Polygon_Clicked;
                
                Pins.Clear();


                area.Add(args.Pin.Position);
                MainThread.BeginInvokeOnMainThread(async () => 
                {
                    string result =await Application.Current.MainPage.DisplayPromptAsync("Information", $"请输入此安全区域的描述信息");
                    if (result==null)
                    {
                        return;
                    }
                    SecureArea secureArea = new SecureArea();
                    secureArea.area = area;
                    secureArea.createtime = DateTime.Now;
                    secureArea.information = result;
                    secureArea.status = true;
                    secureArea.wardid = WardId;
                    secureArea.wardname = WardName;

                    CommunicateWithBackEnd postarea = new CommunicateWithBackEnd();
                    var newarea= await postarea.PutSecureArea(secureArea);
                    if(newarea!=null)
                    {
                        polygon.ZIndex = newarea.id;//暂时用这个属性充当唯一标识（id）
                        secureAreas.Add(newarea);
                        Polygons.Add(polygon);
                    }

                });
                
                //此处要跟后端通讯，传送安全区域的范围，creater需要在后端解析token后再添加,安全区域绑定的对象要在通讯时发送给后端
            });

        private async void Polygon_Clicked(object sender, EventArgs e)
        {
            //Polygon temppol = new Polygon();
            //SecureArea temparea = new SecureArea();
            var pol = (Polygon)sender;//选中的安全区域
            //temppol = pol;
            
            string action;
            var se = secureAreas.Find(p => p.id == pol.ZIndex);
            //temparea = se;
            bool res;
            res = await Application.Current.MainPage.DisplayAlert("安全区域基本信息", "创建者：" + se.creatername + "\n被监护人：" + se.wardname + "\n创建时间：" + se.createtime + "\n信息：" + se.information + "\n状态：" + se.status, "编辑", "取消");
            
            if(res==false)
            {
                return;
            }

            if (se.status)
            {
                action = await Application.Current.MainPage.DisplayActionSheet("编辑", null, null, "停用", "删除", "取消");
            }
            else
            {
                action = await Application.Current.MainPage.DisplayActionSheet("编辑", null, null, "启用", "删除", "取消");
            }

            CommunicateWithBackEnd alter = new CommunicateWithBackEnd();
            //await alterarea.PostSecureArea(se);
            if (action.Equals("启用"))
            {
                pol.StrokeColor = Color.Green;
                pol.FillColor = Color.FromRgba(255, 0, 0, 64);
                se.status = true;
            }
            else if (action.Equals("停用"))
            {
                //停用的话可能要判断被监护人和监护人的距离
                pol.StrokeColor = Color.Black;
                pol.FillColor = Color.FromRgba(126, 0, 0, 20);
                se.status = false;
            }
            else if (action.Equals("删除"))
            {
                Polygons.Remove(pol);
                secureAreas.Remove(se);
                return;
            }
            else
            {
                return;
            }
            //跟后端通讯
            var newarea = await alter.PostSecureArea(se);
            if (newarea != null)
            {
                se.createtime = newarea.createtime;
            }
            else
            {//回滚
                //pol = temppol;
                //se = temparea;
            }
        }
    }
}
