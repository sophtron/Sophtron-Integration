using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SophtronClient
{
    public class OauthClient: IAuthProvider
    {

        private string UserName = Environment.GetEnvironmentVariable("SophtronUserName");
        private string ClientId = Environment.GetEnvironmentVariable("SophtronApiClientId");
        private string ClientSecret = Environment.GetEnvironmentVariable("SophtronApiClientSecret");

        private HttpClient client = new HttpClient() {
            BaseAddress = new Uri("https://sophtron.com/"),
        };

        private string token;
        private DateTime token_expires_at;

        public OauthClient()
        {
            client.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{ClientId}:{ClientSecret}")));
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlecoded");
        }

        public async Task<string> GetAuthPhrase(string httpMethod, string url)
        {
            if(token_expires_at > DateTime.Now)
            {
                return token;
            }
            var resp = await client.PostAsync("oauth/token", new StringContent($"grant_type=password&username={UserName}"));
            if (resp.IsSuccessStatusCode)
            {
                var content = await resp.Content.ReadAsStringAsync();
                Console.WriteLine("retrieved token: " + content);
                var tokenRes = JObject.Parse(content);
                token = "Bearer " + (string)tokenRes["access_token"];
                var expires_in = int.Parse((string)tokenRes["expires_in"]);
                token_expires_at = DateTime.Now.AddSeconds(expires_in);
                return token;
            }
            return null;
        }
    }
}
