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

        public async void LoadWardId(string userid)
        {
            try
            {
                //这里需要向服务器请求选中的用户数据，WardStore需要重写
                WardStore wardStore = new WardStore();
                var ward = await wardStore.GetWardAsync(userid);
                Id = ward.UserID;
                Name = ward.Name;
                Information = ward.Information;

            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Ward");
            }
        }
    }
}
