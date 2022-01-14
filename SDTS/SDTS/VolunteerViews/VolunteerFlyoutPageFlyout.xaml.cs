using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SDTS.VolunteerViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VolunteerFlyoutPageFlyout : ContentPage
    {
        public ListView ListView;

        public VolunteerFlyoutPageFlyout()
        {
            InitializeComponent();

            BindingContext = new VolunteerFlyoutPageFlyoutViewModel();
            ListView = MenuItemsListView;
        }

        class VolunteerFlyoutPageFlyoutViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<VolunteerFlyoutPageFlyoutMenuItem> MenuItems { get; set; }

            public VolunteerFlyoutPageFlyoutViewModel()
            {
                MenuItems = new ObservableCollection<VolunteerFlyoutPageFlyoutMenuItem>(new[]
                {
                    new VolunteerFlyoutPageFlyoutMenuItem { Id = 0, Title = "Page 1" },
                    new VolunteerFlyoutPageFlyoutMenuItem { Id = 1, Title = "Page 2" },
                    new VolunteerFlyoutPageFlyoutMenuItem { Id = 2, Title = "Page 3" },
                    new VolunteerFlyoutPageFlyoutMenuItem { Id = 3, Title = "Page 4" },
                    new VolunteerFlyoutPageFlyoutMenuItem { Id = 4, Title = "Page 5" },
                });
            }

            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }
    }
}