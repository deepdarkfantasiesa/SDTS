using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using System.Collections.ObjectModel;
using Xamarin.Essentials;
using Models;
using System.Threading.Tasks;
using SDTS.Services;

namespace SDTS.ViewModels
{
    public class ManageSecureAreaViewModel : BasesViewModel
    {
        public ManageSecureAreaViewModel()
        {

        }
        public ManageSecureAreaViewModel(string wardaccount, string wardname)
        {
            WardAccount = wardaccount;
            WardName = wardname;
            LoadSecureAreasCommand = new Command(async () => await ExecuteLoadSecureAreasCommand());
            Polygons = new ObservableCollection<Polygon>();
            LoadSecureAreasCommand.Execute(this);
        }

        public Command LoadSecureAreasCommand { get; }

        async Task ExecuteLoadSecureAreasCommand()
        {
            try
            {
                Polygons.Clear();
                secureAreas.Clear();
                CommunicateWithBackEnd getareas = new CommunicateWithBackEnd();
              
                secureAreas = await getareas.GetWardSecureArea(WardAccount);
                Polygon pol;
                foreach (var area in secureAreas)
                {
                    string[] latgroup = area.Latitude.Split(",");
                    string[] longroup = area.Longitude.Split(",");

                    pol = new Polygon();

                    for(int i=0;i<latgroup.Length;i++)
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
            }
            catch(Exception ex)
            {

            }
        }

        private string wardaccount;
        public string WardAccount
        {
            get
            {
                return wardaccount;
            }
            set
            {
                wardaccount = value;
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
                    List<MyPosition> myPositions = new List<MyPosition>();
                    MyPosition pos;
                    SecureArea secureArea = new SecureArea();

                    foreach(var newpos in area)
                    {
                        pos = new MyPosition();
                        pos.Latitude = newpos.Latitude;
                        pos.Longitude = newpos.Longitude;
                        myPositions.Add(pos);
                    }
                    secureArea.area = myPositions;
                    secureArea.createtime = DateTime.Now;
                    secureArea.information = result;
                    secureArea.status = true;
                    secureArea.wardaccount = WardAccount;
                    secureArea.wardname = WardName;

                    CommunicateWithBackEnd postarea = new CommunicateWithBackEnd();
                    var newarea= await postarea.PutSecureArea(secureArea);
                    if(newarea!=null)
                    {
                        polygon.Tag = newarea.areaid;
                        secureAreas.Add(newarea);
                        Polygons.Add(polygon);
                    }

                });
                
            });

        private async void Polygon_Clicked(object sender, EventArgs e)
        {
            var pol = (Polygon)sender;//选中的安全区域
            
            string action;
            var se = secureAreas.Find(p => p.areaid == pol.Tag.ToString());

            bool res;
            res = await Application.Current.MainPage.DisplayAlert("安全区域基本信息", "创建者：" + se.creatername + "\n被监护人：" + se.wardname + "\n创建时间：" + se.createtime + "\n说明：" + se.information + "\n状态：" + se.status, "编辑", "取消");
            
            if(res==false)
            {
                return;
            }

            if (se.status)
            {
                action = await Application.Current.MainPage.DisplayActionSheet("编辑", null, null,"修改说明", "停用", "删除", "取消");
            }
            else
            {
                action = await Application.Current.MainPage.DisplayActionSheet("编辑", null, null, "修改说明", "启用", "删除", "取消");
            }

            CommunicateWithBackEnd cwb = new CommunicateWithBackEnd();
            if (action.Equals("启用"))
            {
                se.status = true;
                var newarea = await cwb.PostSecureArea(se);
                if (newarea != null)
                {
                    se.createtime = newarea.createtime;
                    pol.StrokeColor = Color.Green;
                    pol.FillColor = Color.FromRgba(255, 0, 0, 64);
                }
                else
                {
                    se.status = !se.status;
                    //弹窗提示修改失败
                }
            }
            else if (action.Equals("停用"))
            {
                se.status = false;
                var newarea = await cwb.PostSecureArea(se);
                if (newarea != null)
                {
                    se.createrid = newarea.createrid;//改成createraccount
                    se.creatername = newarea.creatername;

                    se.createtime = newarea.createtime;
                    pol.StrokeColor = Color.Black;
                    pol.FillColor = Color.FromRgba(126, 0, 0, 20);
                }
                else
                {
                    se.status = !se.status;
                    //弹窗提示修改失败
                }
            }
            else if (action.Equals("修改说明"))
            {
                string result = await Application.Current.MainPage.DisplayPromptAsync("修改说明", $"请输入此安全区域的描述信息");
                if (result == null)
                {
                    return;
                }
                se.information = result;
                var newarea = await cwb.PostSecureArea(se);
                if(newarea!=null)
                {
                    se.createtime = newarea.createtime;
                    se.createraccount = newarea.createraccount;
                    se.creatername = newarea.creatername;
                }
            }
            else if (action.Equals("删除"))
            {
                //先跟后端通讯再移除客户端的对象
                var result = await cwb.DeleteSecureArea(se.areaid);
                if (result == true)
                {
                    Polygons.Remove(pol);
                    secureAreas.Remove(se);
                }
                else
                {
                    //弹窗提示删除失败
                }
            }
            else//取消
            {
                return;
            }

        }
    }
}
