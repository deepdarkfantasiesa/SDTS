using Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SDTS
{
    public class Sign
    {
        HttpClient client;
        JsonSerializerOptions serializerOptions;
        public Sign()
        {
            client = new HttpClient();
            serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }
        public async Task<bool> Signup(User user)
        {
            Uri uri = new Uri(string.Format(Constants.SignupString, string.Empty));

            try
            {
                string json = JsonSerializer.Serialize<User>(user, serializerOptions);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = null;
                response = await client.PostAsync(uri, content);
                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine(@"\tTodoItem successfully saved.");
                    return true;
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return false;
        }

        public async Task Signin(string username, string password)
        {
            User user = new User();
            //user.Name = username;
            //user.PassWord = password;
            Uri uri = new Uri(string.Format(Constants.SigninString+ "?username=" + username+ "&password=" + password, string.Empty));
            try
            {
                HttpResponseMessage response = null;
                response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    //存储token，SignalR传数据、以及修改资料添加（被）监护人要用
                    GlobalVariables.token = await response.Content.ReadAsStringAsync();
                    //Debug.WriteLine(token);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
            //return user;//返回对象待定
        }

        
    }
}
