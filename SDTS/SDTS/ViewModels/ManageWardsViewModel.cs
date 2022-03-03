using Models;
using SDTS.GuardianViews;
using SDTS.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SDTS.ViewModels
{
    public class ManageWardsViewModel : BasesViewModel
    {
        public Command AddWardCommand { get; }
        public ObservableCollection<User> Wards { get; }
        public Command LoadWardsCommand { get; }
        public Command<User> WardTapped { get; }
        
        public ManageWardsViewModel()
        {
            Title = "被监护人";

            Wards = new ObservableCollection<User>();

            AddWardCommand = new Command(OnAddWard);

            LoadWardsCommand = new Command(async () => await ExecuteLoadWardsCommand());

            WardTapped = new Command<User>(OnItemSelected);
        }

        public void OnAppearing()
        {
            IsBusy = true;
            SelectedWard = null;
        }

        private User _selectedWard;
        public User SelectedWard
        {
            get => _selectedWard;
            set
            {
                SetProperty(ref _selectedWard, value);
                OnItemSelected(value);
            }
        }

        async void OnItemSelected(User ward)
        {
            if (ward == null)
                return;

            await Shell.Current.GoToAsync($"{nameof(WardsDetailPage)}?{nameof(WardDetailViewModel.Account)}={ward.Account}");
        }

        async Task ExecuteLoadWardsCommand()
        {
            IsBusy = true;

            try
            {
                Wards.Clear();

                CommunicateWithBackEnd getwards = new CommunicateWithBackEnd();

                var wards = await getwards.RefreshWardsDataAsync();

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
            var code = await Application.Current.MainPage.DisplayPromptAsync("添加被监护人", $"请在框内输入邀请码", "完成", "取消");
            CommunicateWithBackEnd cwb = new CommunicateWithBackEnd();
            var result= await cwb.AddWard(code);
            if(result.Equals(true))
            {
                OnAppearing();
            }
        }
    }
}
