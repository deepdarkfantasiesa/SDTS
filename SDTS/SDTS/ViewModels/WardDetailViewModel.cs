using SDTS.GuardianViews;
using SDTS.Services;
using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace SDTS.ViewModels
{
    [QueryProperty(nameof(Account), nameof(Account))]
    public class WardDetailViewModel : BasesViewModel
    {
        private string name;
        private string information;
        private string gender;
        private string age;

        private string account;

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

        public Command ManageSecureArea => new Command(async () => {
            await Application.Current.MainPage.Navigation.PushAsync(new ManageSecureArea(Account, Name));
        });

        public Command RemoveWard => new Command(async () => {
            var code = await Application.Current.MainPage.DisplayPromptAsync("移除被监护人", $"请在框内输入操作码", "完成", "取消");
            CommunicateWithBackEnd cwb = new CommunicateWithBackEnd();
            var result = await cwb.RemoveWard(code,Account);
            if (result.Equals(true))
            {
                await Application.Current.MainPage.Navigation.PopToRootAsync();
            }
        });

    }
}
