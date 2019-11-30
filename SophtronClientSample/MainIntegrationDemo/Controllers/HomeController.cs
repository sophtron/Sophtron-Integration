using MainOAuthDemo.Models;
using SophtronClient;
using SophtronEntities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;

namespace MainOAuthDemo.Controllers
{
    public class HomeController : Controller
    {
        private static readonly string SophtronUserID = ConfigurationManager.AppSettings["SophtronUserID"];
        private static readonly string SophtronAccessKey = ConfigurationManager.AppSettings["SophtronAccessKey"];
        private static readonly string WidgetUrl = ConfigurationManager.AppSettings["SophtronWidgetUrl"];

        /// <summary>
        /// RequestId in this example is passed as the EndUserId parameter to sophtron. it is used for correlating the request and the callback 
        /// This collection is to persist the mappings between request ids and the user InstitutionId returned by the callback
        /// InstitutionId is the identifier to access that bank account added during the request, 
        /// </summary>
        private static IDictionary<string, UserInstitution> Established = new Dictionary<string, UserInstitution>();

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
        /// before embedding the sophtron integration page.
        /// Note, you need to get integration key each time you want load up the integration page.
        /// Sophtron service will return you a new key if the previous one has expired, each key expires after 60 minutes.
        /// Make sure the current page is refreshed or expired within 60 minutes as the corresponding Sophtron session will expire for security reason.
        /// </summary>
        public ActionResult Index()
        {
            ApiClient client = GetSophtronApiClient();
            var response = client.GetIntegrationKeyByUserID();
            ViewBag.IntegrationKey = response["IntegrationKey"];
            ViewBag.UserId = SophtronUserID;
            ViewBag.BaseUrl = WidgetUrl + "/addall"; //use "addall" to retrieve transactions on the go. if only account balances are needed, don't include the "/addall"
            ViewBag.RequestId = Guid.NewGuid().ToString();
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
            if (body == null || string.IsNullOrEmpty(body.EndUserId))
            {
                Debug.WriteLine("got empty event");
            }
            else
            {
                Debug.WriteLine("callback status: {0}", (object)body.Status);
                switch (body.Status.ToLower())
                {
                    case "integration_success":
                        //EndUserId is the parameter used by callback to correlate the request, in this case, it's the RequestId that we generated when user loaded the index page.
                        Established[body.EndUserId] = new UserInstitution { UserInstitutionId = Guid.Parse(body.UserInstitutionID) };
                        break;
                    case "integration_start":
                        Debug.WriteLine("Start Integration");
                        break;
                    case "select_institution":
                        Debug.WriteLine("Select Institution");
                        break;
                    default:
                        break;
                }
            }
            return Json("event captured");
        }

        /// <summary>
        /// Frontend is going to check this endpoint for the transaction details, and display it when ready
        /// Might also be reasonable to use a web socket instead of polling. 
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public ActionResult Transactions(string requestId)
        {
            if (string.IsNullOrEmpty(requestId))
            {
                return null;
            }
            if (Established.ContainsKey(requestId))
            {
                var institution = Established[requestId];
                if (institution.Accounts.Count == 0)
                {
                    ApiClient client = GetSophtronApiClient();
                    var response = client.GetUserInstitutionAccounts(institution.UserInstitutionId);
                    if (response != null)
                    {
                        foreach (var acct in response)
                        {
                            var acc = new TransactionModel
                            {
                                AccountBalance = acct.Balance,
                                AccountName = acct.AccountName,
                                AccountNumber = acct.AccountNumber
                            };
                            acc.Transactions = client.GetTransactionsByTransactionDate(acct.AccountID, DateTime.Now.AddDays(-30).Date, DateTime.Now.Date);
                            acc.Transactions = acc.Transactions.Take(3).ToList();
                            institution.Accounts.Add(acc);
                            Debug.WriteLine("Name: {0}, Number: {1}, Balance: {2}", acct.AccountName, acct.AccountNumber, acct.Balance);
                        }
                    }
                    else
                    {
                        Debug.WriteLine("No account information for {0}", institution.UserInstitutionId);
                    }
                }
                return PartialView(institution.Accounts);
            }
            return PartialView(null);
        }

        public class UserInstitution
        {
            public Guid UserInstitutionId { get; set; }
            public IList<TransactionModel> Accounts { get; set; } = new List<TransactionModel>();
        }

        public class TransactionModel
        {
            public string AccountName { get; set; }
            public string AccountNumber { get; set; }
            public decimal? AccountBalance { get; set; }
            public IList<Transaction> Transactions { get; set; } = new List<Transaction>();
        }
    }
}