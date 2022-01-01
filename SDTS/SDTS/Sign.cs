using Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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

        public async Task<User> Signin(string username, string password)
        {
            User user = new User();
            user.Name = username;
            user.PassWord = password;
            Uri uri = new Uri(string.Format(Constants.SigninString+ "?username=" + username+ "&password=" + password, string.Empty));
            try
            {
                HttpResponseMessage response = null;
                response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string token = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine(token);
                    //token需要通过全局变量存起来，每次请求要用
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
            return user;//返回对象待定
        }

        
    }
}
