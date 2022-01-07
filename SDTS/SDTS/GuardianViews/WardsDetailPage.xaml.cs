using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SDTS.GuardianViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WardsDetailPage : ContentPage
    {
        public WardsDetailPage()
        {
            InitializeComponent();
        }

        private void Create(object sender, EventArgs e)
        {
            Navigation.PushAsync(new CreateSecureArea());
        }
    }
}