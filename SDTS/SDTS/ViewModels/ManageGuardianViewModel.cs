using Models;
using SDTS.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SDTS.ViewModels
{
    public class ManageGuardianViewModel: BasesViewModel
    {
        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        public ObservableCollection<User> Guardians { get; }

        public Command AddGuardianCommand { get; }

        public Command LoadGuardiansCommand { get; }

        public ManageGuardianViewModel()
        {
            Guardians = new ObservableCollection<User>();

            AddGuardianCommand = new Command(OnAddGuardian);

            LoadGuardiansCommand = new Command(async () => await ExecuteLoadGuardiansCommand());
        }

        private async void OnAddGuardian(object obj)
        {
            //await Shell.Current.GoToAsync(nameof(AddWardPage));
            CommunicateWithBackEnd cwb = new CommunicateWithBackEnd();
            var code =await cwb.GetInvitationCode();

            var result= await Application.Current.MainPage.DisplayAlert("邀请码","请在待添加的监护人端输入以下代码：\n"+code,"完成","取消");
            if(result.Equals(true))
            {
                OnAppearing();
            }
        }

        public void OnAppearing()
        {
            IsBusy = true;
        }

        async Task ExecuteLoadGuardiansCommand()
        {
            IsBusy = true;

            try
            {
                Guardians.Clear();
                //WardStore dataStore = new WardStore();//123
                //此处需要请求服务器返回与此监护人绑定的被监护人的信息，并遍历载入Wards集合中，WardStore需要重写
                //var wards = await dataStore.GetWardsAsync(true);
                CommunicateWithBackEnd getguardian = new CommunicateWithBackEnd();
                //此处需要添加登录成功后解析token并将用户信息以全局变量的形式存储好，取出userid传入RefreshDataAsync
                //var wards = await getwards.RefreshDataAsync();
                var guardians = await getguardian.RefreshGuardiansDataAsync();
                foreach (var guardian in guardians)
                {
                    Guardians.Add(guardian);
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
    }
}
