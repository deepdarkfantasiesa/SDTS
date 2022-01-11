using Models;
using SDTS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace SDTS.Services
{
    public class CommunicateWithBackEnd
    {
        HttpClient client;
        JsonSerializerOptions serializerOptions;
        public CommunicateWithBackEnd()
        {
            client = new HttpClient();
            serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                IncludeFields = true
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
            Uri uri = new Uri(string.Format(Constants.SigninString + "?username=" + username + "&password=" + password, string.Empty));
            try
            {
                HttpResponseMessage response = null;
                response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    //存储token，SignalR传数据、以及修改资料添加（被）监护人要用
                    TokenString.token = await response.Content.ReadAsStringAsync();
                    //Debug.WriteLine(token);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
            //return user;//返回对象待定
        }

        public async Task<User> GetWardDetail(int userid)
        {
            Uri uri = new Uri(string.Format(Constants.GetWardDetailString + "?access_token=" + TokenString.token+"&userid="+userid, string.Empty));
            User ward=new User();
            try
            {
                HttpResponseMessage message = await client.GetAsync(uri);
                if (message.IsSuccessStatusCode)
                {
                    string content = await message.Content.ReadAsStringAsync();
                    ward = JsonSerializer.Deserialize<User>(content, serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }

            return ward;
        }

        public async Task<ObservableCollection<User>> RefreshDataAsync()
        {
            ObservableCollection<User> wards = new ObservableCollection<User>();
            //token放在哪需要验证
            Uri uri = new Uri(string.Format(Constants.ManageWardsString+"?access_token="+TokenString.token, string.Empty));
            
            try
            {
                HttpResponseMessage message = await client.GetAsync(uri);
                if (message.IsSuccessStatusCode)
                {
                    string content = await message.Content.ReadAsStringAsync();
                    wards = JsonSerializer.Deserialize<ObservableCollection<User>>(content, serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
            return wards;
        }

        public async Task<SecureArea> PutSecureArea(SecureArea area)
        {
            Uri uri = new Uri(string.Format(Constants.PutSecureAreaString + "?access_token=" + TokenString.token, string.Empty));
            SecureArea newarea=new SecureArea();
            try
            {
                 
                 var json= JsonSerializer.Serialize(area,serializerOptions);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                //Debug.WriteLine(content.ReadAsStringAsync().Result);

                HttpResponseMessage response = await client.PutAsync(uri, content);

                //HttpResponseMessage response = await client.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    string res = await response.Content.ReadAsStringAsync();
                    newarea = JsonSerializer.Deserialize<SecureArea>(res, serializerOptions);
                    return newarea;
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return null;
        }

        
        public async Task<SecureArea> PostSecureArea(SecureArea area)
        {
            Uri uri = new Uri(string.Format(Constants.PostSecureAreaString + "?access_token=" + TokenString.token, string.Empty));
            SecureArea newarea = new SecureArea();

            try
            {

                var json = JsonSerializer.Serialize(area, serializerOptions);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                //Debug.WriteLine(content.ReadAsStringAsync().Result);

                HttpResponseMessage response = await client.PostAsync(uri, content);

                //HttpResponseMessage response = await client.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    string res = await response.Content.ReadAsStringAsync();
                    newarea = JsonSerializer.Deserialize<SecureArea>(res, serializerOptions);
                    return newarea;
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return null;
        }

        public async Task<bool> DeleteSecureArea(int areaid)
        {
            Uri uri = new Uri(string.Format(Constants.DeleteSecureAreaString + "?access_token=" + TokenString.token+"&areaid="+areaid, string.Empty));
            bool res;

            try
            {
                HttpResponseMessage message = await client.DeleteAsync(uri);
                if (message.IsSuccessStatusCode)
                {
                    string content = await message.Content.ReadAsStringAsync();
                    res = JsonSerializer.Deserialize<bool>(content, serializerOptions);
                    return res;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
            return false;
        }

    }
}
