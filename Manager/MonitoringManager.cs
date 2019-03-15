using System;
using System.Net.Http;
using System.Threading.Tasks;
using Models;
using Newtonsoft.Json;

namespace Manager
{
    public class MonitoringManager: IDisposable
    {
        private HttpClient _http;


        public MonitoringManager(HttpClient http = null)
        {
            _http = http ?? new HttpClient();
            _http.BaseAddress = new Uri("https://api.football-data.org/v2/");
            _http.DefaultRequestHeaders.Add("X-Auth-Token", "f74e0beb5501485895a1ebb03ba925db");
        }


        public void Dispose()
        {
            _http = null;
        }

        public async Task ResponseApi()
        {
            try
            {
                Console.WriteLine("----- Try to call API -----");
                var response = await _http.GetAsync("competitions/2000");
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine("----- Verify Api call content -----");
                JsonConvert.DeserializeObject<Competition>(responseContent);
                Console.WriteLine("----- API -----");
            }
            catch (Exception e)
            {
                Console.WriteLine("----- API ERROR SEND EMAIL TO WEBMASTER -----");
                using (var emailManager = new EmailManager())
                {
                    emailManager.SendWebMasterEmail(e);
                }
            }

        }
    }
}
