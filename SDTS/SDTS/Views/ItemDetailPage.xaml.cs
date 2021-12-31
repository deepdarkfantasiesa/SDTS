using SDTS.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace SDTS.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}