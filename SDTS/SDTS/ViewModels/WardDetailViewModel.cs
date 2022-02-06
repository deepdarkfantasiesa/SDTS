
using SDTS.GuardianViews;
using SDTS.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace SDTS.ViewModels
{
    //[QueryProperty(nameof(UserId), nameof(UserId))]
    [QueryProperty(nameof(Account), nameof(Account))]
    public class WardDetailViewModel : BasesViewModel
    {
        private string userId;
        private string name;
        private string information;
        private string gender;
        private string age;

        private string account;
        //public Command CreateSecureArea = new Command(async () => { await Shell.Current.GoToAsync($"//WardsDetailPage/CreateSecureArea"); });

        public int Id { get; set; }

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        public string Information
        {
            get => information;
            set => SetProperty(ref information, value);
        }

        public string UserId
        {
            get
            {
                return userId;
            }
            set
            {
                userId = value;
                LoadWardId(value);
            }
        }

        public string Gender
        {
            get => gender;
            set => SetProperty(ref gender, value);
        }

        public string Age
        {
            get => age;
            set => SetProperty(ref age, value);
        }

        public string Account
        {
            get => account;
            set
            {
                account = value;
                LoadWardWithAccount(value);
            }
        }

        public WardDetailViewModel()
        {
            //CreateSafeArea = new Command(async (args) =>await createarea(args));
            //CreateSecureArea = new Command(async () => { await Shell.Current.GoToAsync($"CreateSecureArea"); });
        }

        public async void LoadWardWithAccount(string useraccount)
        {
            try
            {
                //这里需要向服务器请求选中的用户数据
                CommunicateWithBackEnd getwards = new CommunicateWithBackEnd();
                var ward = await getwards.GetWardDetailWithAccount(useraccount);

                Id = ward.UserID;
                Name = ward.Name;
                Information = ward.Information;
                Gender = ward.Gender;
                Age = (DateTime.Now.Year - ward.Birthday.Year).ToString();
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Ward");
            }
        }

        //向服务器请求被选中的被监护人的详细信息
        public async void LoadWardId(string userid)
        {
            try
            {
                //这里需要向服务器请求选中的用户数据
                CommunicateWithBackEnd getwards = new CommunicateWithBackEnd();
                var ward = await getwards.GetWardDetail(int.Parse(userid));
                
                Id = ward.UserID;
                Name = ward.Name;
                Information = ward.Information;
                Gender = ward.Gender;
                Age = (DateTime.Now.Year - ward.Birthday.Year).ToString();
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Ward");
            }
        }

        public Command ManageSecureArea => new Command(async () => {
            //await Application.Current.MainPage.Navigation.PushAsync(new ManageSecureArea(UserId, Name));
            await Application.Current.MainPage.Navigation.PushAsync(new ManageSecureArea(Account, Name));
        });

        public Command RemoveWard => new Command(async () => {
            var code = await Application.Current.MainPage.DisplayPromptAsync("移除被监护人", $"请在框内输入操作码", "完成", "取消");
            CommunicateWithBackEnd cwb = new CommunicateWithBackEnd();
            var result = await cwb.RemoveWard(code,Account);
            if (result.Equals(true))
            {
                //OnAppearing();
                await Application.Current.MainPage.Navigation.PopToRootAsync();
            }
        });

    }
}
