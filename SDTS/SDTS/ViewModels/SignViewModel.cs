using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xamarin.Forms;


namespace SDTS.ViewModels
{
    class SignViewModel: INotifyPropertyChanged
    {
        public SignViewModel()
        {
            //client = new HttpClient();
            //serializerOptions = new JsonSerializerOptions
            //{
            //    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            //    WriteIndented = true
            //};
        }
        string password;
        public string PassWord 
        {
            get
            {
                return password;
            }
            set 
            {
                password = value;
                OnPropertyChanged("username"); 
            } 
        }

        string username;
        public string UserName
        {
            get
            {
                return username;
            }
            set
            {
                username = value;
                OnPropertyChanged("username");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public Command signup => new Command(async()=> { });

        public Command forgetpwd => new Command(async () => { });

        public Command signin => new Command(async () => 
        {
            
            //Debug.WriteLine("username:"+username);
            //Debug.WriteLine("password:" + password);
            
            Sign sign = new Sign();
            await sign.Signin(UserName, PassWord);
        });

        //HttpClient client;
        //JsonSerializerOptions serializerOptions;
        //public async Task<User> Signin(string username,string password)
        //{
        //    User user = new User();
        //    Uri uri = new Uri(string.Format(Constants.SigninString, string.Empty));
        //    try
        //    {
        //        HttpResponseMessage message = await client.GetAsync(uri);
        //        if (message.IsSuccessStatusCode)
        //        {
        //            string content = await message.Content.ReadAsStringAsync();
        //            // users = JsonSerializer.Deserialize<List<User>>(content, serializerOptions);
        //            user = JsonSerializer.Deserialize<User>(content, serializerOptions);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(@"\tERROR {0}", ex.Message);
        //    }
        //    return user;
        //}

    }
}
