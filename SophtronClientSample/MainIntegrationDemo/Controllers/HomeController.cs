using MainOAuthDemo.Models;
using SophtronClient;
using SophtronEntities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Mvc;

namespace MainOAuthDemo.Controllers
{
    public class HomeController : Controller
    {
        private static readonly string SophtronUserID = ConfigurationManager.AppSettings["SophtronUserID"];
        private static readonly string SophtronAccessKey = ConfigurationManager.AppSettings["SophtronAccessKey"];

        /// <summary>
        /// Get api client by using sophtron user ID/AccessKey
        /// </summary>
        private ApiClient GetSophtronApiClient()
        {
            var accessUser = new User
            {
                UserID = Guid.Parse(SophtronUserID), 
                AccessKey = SophtronAccessKey
            };
            ApiClient client = new ApiClient(accessUser);
            return client;
        }

        /// <summary>
        /// Demo landing page, this shows the logic for getting the integration key 
        /// before embedding the sophtron integration page
        /// Note, you need to get integration key each time you want load up the integration page
        /// Sophtron service will return you a new key if previous one has expired
        /// </summary>
        public ActionResult Index()
        {
            ApiClient client = GetSophtronApiClient();
            var response = client.GetIntegrationKeyByUserID();
            return new RedirectResult(string.Format("/home/integration?key={0}", response["IntegrationKey"]));
        }

        /// <summary>
        /// Demo page for embedding the integration page, see corresponding view page for more information
        /// </summary>
        /// <param name="key">this is the integration key</param>
        public ActionResult Integration(string key)
        {
            ViewBag.IntegrationKey = key;
            return View();
        }

        /// <summary>
        /// This is a demo callback handler
        /// 1, You need to implement a POST handler and expose to public network in order to be called by Sophtron server
        /// 2, Then you must signin to Sophtron developer dashboard, and register your public callback URL
        /// 3, You can specify whitelist IPs (ipv4) for your callback endpoint (Note, this is optional, once this is set, Sophtron will check callback IP before sending events)
        /// 4, Now you should be able to receive callback events once your customer login to institution on Sophtron integration page
        /// 5, Get detailed information through Sophtron API by leveraging the information comes from callback event
        /// </summary>
        /// <param name="body">IntegrationEventCallback this model shows the format of Sophtron integration callback event</param>
        [HttpPost]
        public ActionResult IntegrationCallback(IntegrationEventCallback body)
        {
            if (body == null)
            {
                Debug.WriteLine("got empty event");
            }
            else
            {
                Debug.WriteLine("callback status: {0}", (object)body.Status);
                if (String.Equals("integration_success", body.Status, StringComparison.OrdinalIgnoreCase))
                {
                    var userInstitutionID = body.UserInstitutionID;
                    ApiClient client = GetSophtronApiClient();
                    var response = client.GetUserInstitutionAccounts(Guid.Parse(userInstitutionID));
                    if (response != null)
                    {
                        foreach (var acct in response)
                        {
                            Debug.WriteLine("Name: {0}, Number: {1}, Balance: {2}", acct.AccountName, acct.AccountNumber, acct.Balance);
                        }
                    }
                    else
                    {
                        Debug.WriteLine("No account information for {0}", (object)userInstitutionID);
                    }
                }
                else if (String.Equals("integration_start", body.Status, StringComparison.OrdinalIgnoreCase))
                {
                    Debug.WriteLine("Start Integration");
                }
                else if (String.Equals("select_institution", body.Status, StringComparison.OrdinalIgnoreCase))
                {
                    Debug.WriteLine("Select Institution");
                }
            }
            return Json("event captured");
        }
    }
}