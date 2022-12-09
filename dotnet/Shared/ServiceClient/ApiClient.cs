using Newtonsoft.Json;
using SophtronEntities;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SophtronClient
{
    public class ApiClient 
    {
        private string AuthUserId = Environment.GetEnvironmentVariable("SophtronApiUserId");
        private readonly HttpClient httpClient = new HttpClient()
        {
            BaseAddress = new Uri("https://api.sophtron.com/api/")
        };
        private readonly IAuthProvider authProvider;

        public ApiClient(IAuthProvider authProvider)
        {
            this.authProvider = authProvider;
        }
        public Task<string> HealthCheck()
        {
            return Request<string>(HttpMethod.Get, "Institution/HealthCheckAuth");
        }

        public Task<IDictionary<string, object>> GetIntegrationKeyByUserID()
        {
            return Request<IDictionary<string, object>>(HttpMethod.Post, "User/GetUserIntegrationKey", new User.GetIntegrationKeyParams { Id = AuthUserId });
        }

        public Task<IList<Dictionary<string, object>>> GetUserInstitutionsByUser()
        {
            return Request<IList<Dictionary<string, object>>>(HttpMethod.Post, "UserInstitution/GetUserInstitutionsByUser", new User.GetUserParams { UserID = new Guid(AuthUserId) });
        }

        public Task<IList<Institution>> GetInstitutionsByName(string name)
        {
            return Request<IList<Institution>>(HttpMethod.Post, "Institution/GetInstitutionByName", new Institution.GetInstitutionByNameParam { InstitutionName = name });
        }

        public Task<JobTracker> CreateUserInstitution(Guid userId, Guid institutionId, string username, string password, string pin)
        {
            return Request<JobTracker>(HttpMethod.Post, "UserInstitution/CreateUserInstitution", new UserInstitution.CreateUserInstitutionParams
            {
                UserID = userId,
                InstitutionID = institutionId,
                UserName = username,
                Password = password,
                PIN = pin
            });
        }

        public Task<JobMFA.JobByIDParam> RetryAddingUserInstitution(Guid userInstitutionId, Guid institutionId, string userName, string password, string pin)
        {
            return Request<JobMFA.JobByIDParam>(HttpMethod.Post, "UserInstitution/RetryAddingUserInstitution", new UserInstitution.RetryAddingUserInstitutionParams
            {
                UserInstitutionID = userInstitutionId
            });
        }

        public Task<JobMFA> GetJobInformationByID(Guid jobId)
        {
            return Request<JobMFA>(HttpMethod.Post, "Job/GetJobInformationByID", new JobMFA.JobByIDParam { JobID = jobId });
        }

        public Task<TransmitStatusEnums> UpdateJobSecurityAnswer(Guid jobID, string securityAnswer)
        {
            return Request<TransmitStatusEnums>(HttpMethod.Post, "Job/UpdateJobSecurityAnswer", new JobMFA.UpdateJobBySecurityAnswerParams
            {
                JobID = jobID,
                SecurityAnswer = securityAnswer
            });
        }

        public Task<TransmitStatusEnums> UpdateJobCaptchaInput(Guid jobID, string captchaInput)
        {
            return Request<TransmitStatusEnums>(HttpMethod.Post, "Job/UpdateJobCaptcha", new JobMFA.UpdateJobByCaptchaParams
            {
                JobID = jobID,
                CaptchaInput = captchaInput
            });
        }

        public Task<TransmitStatusEnums> UpdateJobTokenChoice(Guid jobID, string tokenChoice)
        {
            return Request<TransmitStatusEnums>(HttpMethod.Post, "Job/UpdateJobTokenInput", new JobMFA.UpdateJobByTokenParams
            {
                JobID = jobID,
                TokenChoice = tokenChoice,
                TokenInput = null,
                VerifyPhoneFlag = null,
            });
        }

        public Task<TransmitStatusEnums> UpdateJobTokenInput(Guid jobID, string tokenInput)
        {
            return Request<TransmitStatusEnums>(HttpMethod.Post, "Job/UpdateJobTokenInput", new JobMFA.UpdateJobByTokenParams
            {
                JobID = jobID,
                TokenChoice = null,
                TokenInput = tokenInput,
                VerifyPhoneFlag = null,
            });
        }

        public Task<TransmitStatusEnums> UpdateJobTokenPhoneVerify(Guid jobID, bool verifyPhoneFlag)
        {
            return Request<TransmitStatusEnums>(HttpMethod.Post, "Job/UpdateJobTokenInput", new JobMFA.UpdateJobByTokenParams
            {
                JobID = jobID,
                TokenChoice = null,
                TokenInput = null,
                VerifyPhoneFlag = verifyPhoneFlag,
            });
        }

        public Task<IList<UserInstitutionAccount>> GetUserInstitutionAccounts(Guid userInstitutionId)
        {
            return Request<IList<UserInstitutionAccount>>(HttpMethod.Post, "UserInstitution/GetUserInstitutionAccounts", new UserInstitution { UserInstitutionID = userInstitutionId });
        }

        public Task<JobMFA.JobByIDParam> RefreshUserInstitutionAccount(Guid userInstitutionAccountId)
        {
            return Request<JobMFA.JobByIDParam>(HttpMethod.Post, "UserInstitutionAccount/RefreshUserInstitutionAccount", new UserInstitutionAccount { AccountID = userInstitutionAccountId });
        }

        public Task<IList<Transaction>> GetTransactionsByTransactionDate(Guid userInstitutionAccountId, DateTime startDate, DateTime endDate)
        {
            return Request<IList<Transaction>>(HttpMethod.Post, "Transaction/GetTransactionsByTransactionDate", new Transaction.GetTransactionByDateParams
            {
                AccountID = userInstitutionAccountId,
                StartDate = startDate,
                EndDate = endDate
            });
        }

        private async Task<T> Request<T>(HttpMethod method, string url, object data = null)
        {
            var auth = await authProvider.GetAuthPhrase(method.ToString(), url);
            if(auth == null)
            {
                throw new Exception("Unable to get auth phrase");
            }
            //Console.WriteLine("Auth: " + auth);
            var req = new HttpRequestMessage(method, url);
            req.Headers.TryAddWithoutValidation("Authorization", auth);
            if (data != null)
            {
                req.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8,
                                    "application/json");
            }
            var res = await httpClient.SendAsync(req);
            if (res.IsSuccessStatusCode)
            {
                var raw = await res.Content.ReadAsStringAsync();
                return typeof(T) == typeof(string) ? (T)(object)raw : JsonConvert.DeserializeObject<T>(raw);
            }
            else
            {
                Console.WriteLine("Request got status: " + res.StatusCode);
            }
            return default(T);
        }

    }
}
