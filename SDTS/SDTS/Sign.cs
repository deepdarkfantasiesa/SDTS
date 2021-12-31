using Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
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
            user.password = password;
            Uri uri = new Uri(string.Format(Constants.SigninString, string.Empty));
            try
            {
                string json = JsonSerializer.Serialize<User>(user, serializerOptions);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                
                HttpResponseMessage response = null;

                IEnumerable<string> token;
                response = await client.PostAsync(uri, content);
                client.PostAsync(uri, content).Result.Content.Headers.TryGetValues("token", out token);
                if (response.IsSuccessStatusCode)
                {
                    IEnumerable<string> token1;
                    response.Content.Headers.GetValues("token");
                    //response.Content.Headers.GetEnumerator("token", out token);
                    response.Content.Headers.TryGetValues("token", out token1);
                    Debug.WriteLine(token);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
            return user;
        }
    }
}
