using SophtronClient;
using SophtronEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SophtronClientSample
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get userid, accesskey from sophtron site - develop page.
            User user = new User();
            user.UserID = new Guid("17AD9654-2915-4F5F-A311-A306B901931A");
            user.AccessKey = "AcY+a2C+keV3IJDuG7nxPahtPZ3VJEO5RmFCDcHeOodZskPKnVaCXbmvaNqsmrzIagchyfeuvb3OI1nfPFcPokhT8kujrtVk17WOhiu4H27=";

            // Direct authentication client
            Sample_DirectAPI(user);

            //TODO: Oauth flow
            //Sample_OAuthAPI(user);

            Console.WriteLine("this is a sophtron client sample code");
            Console.ReadKey();
        }

        private static void Sample_DirectAPI(User user)
        {
            ApiClient client = new ApiClient(user);

            // Healthcheck: "this is online."
            string onlineRes = client.HealthCheck();

            // Basic Work Flow 
            string institutionName = "Citibank";
            Guid institutionId = client.GetInstitutionsByName(institutionName).ToList().FirstOrDefault().InstitutionID; //choose target bank 

            Guid userId = user.UserID;
            string username = "bankloginname"; //input your bank login name
            string password = "bankloginpwd"; //input your bank login password
            string pin = string.Empty;
            
            JobTracker jobTracker = client.CreateUserInstitution(userId, institutionId, username, password, pin);            
            HandleMFA(client, jobTracker.JobID);

            // IF MFA Failed -> Retry: 
            // JobTracker jobTracker = client.RetryUserInstitution(jobTracker.UserInstitutionID, institutionId, userName, password, pin);
            // HandleMFA(client, jobTracker.JobID)
            
            // IF MFA Succeed -> Get Account:
            IList<UserInstitutionAccount> accounts = client.GetUserInstitutionAccounts(jobTracker.UserInstitutionID).ToList();
            if(accounts != null && accounts.Count > 0)
            {
                // Refresh Account:
                Guid jobId = client.RefreshUserInstitutionAccount(accounts.FirstOrDefault().AccountID).JobID;
                HandleMFA(client, jobId);

                // IF MFA Succeed -> Get Transactions:
                IList<Transaction> transactionsHistory = client.GetTransactionsByTransactionDate(accounts.FirstOrDefault().AccountID, DateTime.Now.AddDays(-30), DateTime.Now);
            }
        }

        private static void HandleMFA(ApiClient client, Guid jobID)
        {
            JobMFA mfa = client.GetJobInformationByID(jobID);
            while (!(bool)mfa.SuccessFlag)
            {
                //only laststep not correct will lead to an error. successflag = 0 means the job is still under processing.
                if (!string.IsNullOrEmpty(mfa.LastStep))
                {
                    //for example, possibly step = 'login' and status='timeout'. Maybe the wrong username, password.
                    break;
                }

                if (mfa.SecurityQuestion != null)
                {
                    //security questions example:  '["What was the first name of your first manager?","What is your mother&#39;s middle name","What is your cat&#39;s name"]'
                    //require user to input security answer. Pay attention to the format.
                    string securityAns = "[\"managername\",\"mothername\",\"catname\"]";
                    client.UpdateJobSecurityAnswer(jobID, securityAns);
                }
                else if (mfa.TokenMethod != null)
                {
                    //token method example: '[":  Email me   Mail me verification code at s*********@gmail.com", ":  Call me  Send verification code by voice call to ***-***-3800"]'
                    //require user to choose.
                    string tokenChoice = ":  Email me   Mail me verification code at s*********@gmail.com";
                    client.UpdateJobTokenChoice(jobID, tokenChoice);
                }
                else if (mfa.TokenSentFlag == true)
                {
                    //require user to fill in token he received.
                    string tokenInput = "123";
                    client.UpdateJobTokenInput(jobID, tokenInput);
                }
                else if (mfa.TokenRead != null)
                {
                    //answer the phone to read the content of 'tokenread'
                    bool phoneVerified = true;
                    client.UpdateJobTokenPhoneVerify(jobID, phoneVerified);
                }
                else if (mfa.CaptchaImage != null)
                {
                    //input captcha password
                    string captchaInput = "12345";
                    client.UpdateJobCaptchaInput(jobID, captchaInput);
                }
                Thread.Sleep(5000);
                mfa = client.GetJobInformationByID(jobID);
            }
        }


    }
}
