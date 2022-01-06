using SDTS.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;

namespace SDTS.ViewModels
{
    [QueryProperty(nameof(UserId), nameof(UserId))]
    public class WardDetailViewModel : BasesViewModel
    {
        private string userId;
        private string name;
        private string information;
        private string gender;
        private string age;
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

    }
}
