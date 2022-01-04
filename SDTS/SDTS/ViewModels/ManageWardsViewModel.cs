using SDTS.GuardianViews;
using SDTS.Models;
using SDTS.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SDTS.ViewModels
{
    public class ManageWardsViewModel : BasesViewModel
    {
        public Command AddWardCommand { get; }
        public ObservableCollection<Ward> Wards { get; }
        public Command LoadWardsCommand { get; }
        public Command<Ward> WardTapped { get; }
        
        public ManageWardsViewModel()
        {
            Title = "被监护人";

            Wards = new ObservableCollection<Ward>();

            AddWardCommand = new Command(OnAddWard);

            LoadWardsCommand = new Command(async () => await ExecuteLoadWardsCommand());

            WardTapped = new Command<Ward>(OnItemSelected);
        }

        public void OnAppearing()
        {
            IsBusy = true;
            SelectedWard = null;
        }

        private Ward _selectedWard;
        public Ward SelectedWard
        {
            get => _selectedWard;
            set
            {
                SetProperty(ref _selectedWard, value);
                OnItemSelected(value);
            }
        }

        async void OnItemSelected(Ward ward)
        {
            if (ward == null)
                return;
            await Shell.Current.GoToAsync($"{nameof(WardsDetailPage)}?{nameof(WardDetailViewModel.UserId)}={ward.UserID}");
        }

        async Task ExecuteLoadWardsCommand()
        {
            IsBusy = true;

            try
            {
                Wards.Clear();
                WardStore dataStore = new WardStore();
                //此处需要请求服务器返回与此监护人绑定的被监护人的信息，并遍历载入Wards集合中，WardStore需要重写
                var wards = await dataStore.GetWardsAsync(true);
                foreach (var ward in wards)
                {
                    Wards.Add(ward);
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

        string title = string.Empty;

        private async void OnAddWard(object obj)
        {
            await Shell.Current.GoToAsync(nameof(AddWardPage));
        }
    }
}
