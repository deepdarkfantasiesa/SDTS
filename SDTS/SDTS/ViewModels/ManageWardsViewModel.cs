using Models;
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
            //await Shell.Current.GoToAsync($"{nameof(WardsDetailPage)}?{nameof(WardDetailViewModel.UserId)}={ward.UserID}");
            await Shell.Current.GoToAsync($"{nameof(WardsDetailPage)}?{nameof(WardDetailViewModel.Account)}={ward.Account}");
        }

        async Task ExecuteLoadWardsCommand()
        {
            IsBusy = true;

            try
            {
                Wards.Clear();
                //WardStore dataStore = new WardStore();//123
                //此处需要请求服务器返回与此监护人绑定的被监护人的信息，并遍历载入Wards集合中，WardStore需要重写
                //var wards = await dataStore.GetWardsAsync(true);
                CommunicateWithBackEnd getwards = new CommunicateWithBackEnd();
                //此处需要添加登录成功后解析token并将用户信息以全局变量的形式存储好，取出userid传入RefreshDataAsync
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
            //await Shell.Current.GoToAsync(nameof(AddWardPage));
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
