﻿using SDTS.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.Xaml;

namespace SDTS.GuardianViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ManageSecureArea : ContentPage
    {
        ManageSecureAreaViewModel _viewModel;
        public ManageSecureArea(string wardaccount, string wardname)
        {
            InitializeComponent();
            //此处需要向服务器请求返回被监护人的位置
            MapSpan mapSpan = MapSpan.FromCenterAndRadius(new Position(22.3254973, 114.1671742), Distance.FromKilometers(0));
            map.MoveToRegion(mapSpan);
            BindingContext = _viewModel = new ManageSecureAreaViewModel(wardaccount, wardname);

        }
    }
}