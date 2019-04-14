using DotNetOpenAuth.OAuth2;
using MainOAuthDemo.Models;
using System;
using System.Net.Http;
using System.Web.Mvc;

namespace MainOAuthDemo.Controllers
{
    public class HomeController : Controller
    {
        private WebServerClient _webServerClient;

        public ActionResult Index()
        {
            ViewBag.AccessToken = Request.Form["AccessToken"] ?? "";
            ViewBag.RefreshToken = Request.Form["RefreshToken"] ?? "";
            ViewBag.Action = "";
            ViewBag.ApiResponse = "";

            InitializeWebServerClient();
            var accessToken = Request.Form["AccessToken"];
            if (string.IsNullOrEmpty(accessToken))
            {
                //Get AccessToken&Refersh Token after getting Authorization Code
                var authorizationState = _webServerClient.ProcessUserAuthorization(Request);
                if (authorizationState != null)
                {
                    ViewBag.AccessToken = authorizationState.AccessToken;
                    ViewBag.RefreshToken = authorizationState.RefreshToken;
                    ViewBag.Action = Request.Path;
                }
            }
            //Get Authorization Code
            if (!string.IsNullOrEmpty(Request.Form.Get("submit.Authorize")))
            {
                var userAuthorization = _webServerClient.PrepareRequestUserAuthorization(new[] { "read" });
                userAuthorization.Send(HttpContext);
                Response.End();
            }
            //Refresh Token
            else if (!string.IsNullOrEmpty(Request.Form.Get("submit.Refresh")))
            {
                var state = new AuthorizationState
                {
                    AccessToken = Request.Form["AccessToken"],
                    RefreshToken = Request.Form["RefreshToken"]
                };
                if (_webServerClient.RefreshAuthorization(state))
                {
                    ViewBag.AccessToken = state.AccessToken;
                    ViewBag.RefreshToken = state.RefreshToken;
                }
            }
            //Call Sophtron API
            else if (!string.IsNullOrEmpty(Request.Form.Get("submit.CallApi")))
            {
                var resourceServerUri = new Uri(ServerInfo.ResourceServerBaseAddress);

                //try healthcheck, results as  "this is online."
                var client = new HttpClient(_webServerClient.CreateAuthorizingHandler(accessToken));
                var response = client.GetAsync(new Uri(resourceServerUri, "/api/Institution/HealthCheckAuth")).Result;
                var contents = response.Content.ReadAsStringAsync();
                ViewBag.ApiResponse = contents.Result;

                //try get userinstitution list results
                StringContent content = new StringContent("{\"UserID\": \"17AD9654-2915-4F5F-A311-A306B901931A\"}", System.Text.Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                response = client.PostAsync(new Uri(resourceServerUri, "/api/UserInstitution/GetUserInstitutionsByUser"), content).Result;

                contents = response.Content.ReadAsStringAsync();
                ViewBag.ApiResponse += contents.Result;
            }

            return View();
        }

        private void InitializeWebServerClient()
        {
            //ClientInfo(clientid, clientsecret)
            ClientInfo demoClient = new ClientInfo("c139bf2e-3801-454f-a7b9-20a1e2482e08", "f6e326d9-bf45-4052-8b6e-0b13b500a467");
            //authorizationserver: https://sophtron.com
            var authorizationServerUri = new Uri(ServerInfo.AuthorizationServerBaseAddress);
            var authorizationServer = new AuthorizationServerDescription
            {
                AuthorizationEndpoint = new Uri(authorizationServerUri, ServerInfo.AuthorizePath), // OAuth/Authorize
                TokenEndpoint = new Uri(authorizationServerUri, ServerInfo.TokenPath) // OAuth/Token
            };
            _webServerClient = new WebServerClient(authorizationServer, demoClient.ClientId, demoClient.ClientSecret);
        }
    }
}