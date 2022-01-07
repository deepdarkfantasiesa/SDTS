using SDTS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.Xaml;

namespace SDTS.GuardianViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateSecureArea : ContentPage
    {
        CreateSecureAreaViewModel _viewModel;
        public CreateSecureArea()
        {
            InitializeComponent();
            //此处需要向服务器请求返回被监护人的位置
            MapSpan mapSpan = MapSpan.FromCenterAndRadius(new Position(22.26, 114.12), Distance.FromKilometers(0));
            map.MoveToRegion(mapSpan);
            //DisplayPromptAsync
            BindingContext = _viewModel = new CreateSecureAreaViewModel();

        }
    }
}