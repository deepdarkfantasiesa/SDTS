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
        public async Task<string> Signup(User user)
        {
            Uri uri = new Uri(string.Format(Constants.SignupString, string.Empty));
            string result=null;
            try
            {
                string json = JsonSerializer.Serialize<User>(user, serializerOptions);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = null;
                response = await client.PostAsync(uri, content);

                result = await response.Content.ReadAsStringAsync();

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return result;
        }
        public async Task Signin(string account, string password)
        {
            User user = new User();
            //user.Name = username;
            //user.PassWord = password;
            Uri uri = new Uri(string.Format(Constants.SigninString + "?account=" + account + "&password=" + password, string.Empty));
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

        public async Task<bool> AddWard(string code)
        {
            Uri uri = new Uri(string.Format(Constants.AddWardString + "?access_token=" + GlobalVariables.token + "&code=" + code, string.Empty));
            bool result=false;
            try
            {
                HttpResponseMessage message = await client.PutAsync(uri, null);
                if (message.IsSuccessStatusCode)
                {
                    string content = await message.Content.ReadAsStringAsync();
                    result = JsonSerializer.Deserialize<bool>(content, serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
            return result;
        }

        public async Task<bool> RemoveWard(string code,string wardaccount)
        {
            Uri uri = new Uri(string.Format(Constants.RemoveWardString + "?access_token=" + GlobalVariables.token + "&code=" + code+"&wardaccount="+ wardaccount, string.Empty));
            bool result = false;
            try
            {
                HttpResponseMessage message = await client.PutAsync(uri, null);
                if (message.IsSuccessStatusCode)
                {
                    string content = await message.Content.ReadAsStringAsync();
                    result = JsonSerializer.Deserialize<bool>(content, serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
            return result;
        }
        public async Task<User> GetWardDetailWithAccount(string account)
        {
            Uri uri = new Uri(string.Format(Constants.GetWardDetailWithAccountString + "?access_token=" + GlobalVariables.token + "&account=" + account, string.Empty));
            User ward = new User();
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

        public async Task<User> GetWardDetail(int userid)
        {
            Uri uri = new Uri(string.Format(Constants.GetWardDetailString + "?access_token=" + GlobalVariables.token+"&userid="+userid, string.Empty));
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
        public async Task<ObservableCollection<User>> RefreshGuardiansDataAsync()
        {
            ObservableCollection<User> guardians = new ObservableCollection<User>();
            //token放在哪需要验证
            Uri uri = new Uri(string.Format(Constants.GetGuardiansString + "?access_token=" + GlobalVariables.token, string.Empty));

            try
            {
                HttpResponseMessage message = await client.GetAsync(uri);
                if (message.IsSuccessStatusCode)
                {
                    string content = await message.Content.ReadAsStringAsync();
                    guardians = JsonSerializer.Deserialize<ObservableCollection<User>>(content, serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
            return guardians;
        }

        public async Task<ObservableCollection<User>> RefreshWardsDataAsync()
        {
            ObservableCollection<User> wards = new ObservableCollection<User>();
            //token放在哪需要验证
            Uri uri = new Uri(string.Format(Constants.ManageWardsString+"?access_token="+ GlobalVariables.token, string.Empty));
            
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
            Uri uri = new Uri(string.Format(Constants.PutSecureAreaString + "?access_token=" + GlobalVariables.token, string.Empty));
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
            Uri uri = new Uri(string.Format(Constants.PostSecureAreaString + "?access_token=" + GlobalVariables.token, string.Empty));
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

        public async Task<bool> DeleteSecureArea(string areaid)
        {
            Uri uri = new Uri(string.Format(Constants.DeleteSecureAreaString + "?access_token=" + GlobalVariables.token + "&areaid=" + areaid, string.Empty));
            bool result;

            try
            {
                HttpResponseMessage message = await client.PostAsync(uri, null);
                if (message.IsSuccessStatusCode)
                {
                    string res = await message.Content.ReadAsStringAsync();
                    result = JsonSerializer.Deserialize<bool>(res, serializerOptions);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
            return false;
        }

        //public async Task<List<SecureArea>> GetWardSecureArea(int wardid)
        public async Task<List<SecureArea>> GetWardSecureArea(string wardaccount)
        {
            List<SecureArea> areas = new List<SecureArea>();
            
            Uri uri = new Uri(string.Format(Constants.GetSecureAreasString + "?access_token=" + GlobalVariables.token + "&wardaccount=" + wardaccount, string.Empty));

            try
            {
                HttpResponseMessage message = await client.GetAsync(uri);
                if (message.IsSuccessStatusCode)
                {
                    string content = await message.Content.ReadAsStringAsync();
                    areas = JsonSerializer.Deserialize<List<SecureArea>>(content, serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
            return areas;
        }

        public async Task GetUserInfo()
        {
            Uri uri = new Uri(string.Format(Constants.GetUserInfoString + "?access_token=" + GlobalVariables.token, string.Empty));
            User user=new User();
            try
            {
                HttpResponseMessage message = await client.GetAsync(uri);
                if (message.IsSuccessStatusCode)
                {
                    string content = await message.Content.ReadAsStringAsync();
                    user = JsonSerializer.Deserialize<User>(content, serializerOptions);
                    GlobalVariables.user = user;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
        }

        public async Task<string> GetInvitationCode()
        {
            Uri uri = new Uri(string.Format(Constants.GetInvitationCodeString + "?access_token=" + GlobalVariables.token, string.Empty));
            //User user = new User();

            string content=null;

            try
            {
                HttpResponseMessage message = await client.GetAsync(uri);
                if (message.IsSuccessStatusCode)
                {
                    content = await message.Content.ReadAsStringAsync();
                    //user = JsonSerializer.Deserialize<User>(content, serializerOptions);
                    //GlobalVariables.user = user;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
            return content;
        }
    }
}
