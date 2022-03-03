using Models;
using SDTS.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

        public Command GenerateOpcodeCommand { get; }

        public Command LoadGuardiansCommand { get; }

        public ManageGuardianViewModel()
        {
            Guardians = new ObservableCollection<User>();

            GenerateOpcodeCommand = new Command(GenerateOpcode);

            LoadGuardiansCommand = new Command(async () => await ExecuteLoadGuardiansCommand());
        }

        private async void GenerateOpcode(object obj)
        {
            CommunicateWithBackEnd cwb = new CommunicateWithBackEnd();
            var code =await cwb.GetInvitationCode();

            var result= await Application.Current.MainPage.DisplayAlert("操作码","请在待操作的监护人端输入以下代码：\n"+code,"完成","取消");
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

                CommunicateWithBackEnd getguardian = new CommunicateWithBackEnd();
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
