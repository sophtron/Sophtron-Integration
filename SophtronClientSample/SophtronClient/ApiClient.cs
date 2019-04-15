using SophtronEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SophtronClient
{
    public class ApiClient : BaseAuthorizedClient
    {
        public override string BaseEndpoint { get; protected set; } = "https://api.sophtron.com/api";
        public override string AuthUserId { get; protected set; }
        public override string AuthUserAccessKey { get; protected set; }

        public ApiClient(User user)
        {
            AuthUserId = user.UserID.ToString();
            AuthUserAccessKey = user.AccessKey;
        }

        public string HealthCheck()
        {
            return GetWithAuth<string>("Institution/HealthCheckAuth");
        }

        public IDictionary<string, object> GetIntegrationKeyByUserID()
        {
            return PostWithAuth<User.GetIntegrationKeyParams, IDictionary<string, object>>("User/GetUserIntegrationKey", () => new User.GetIntegrationKeyParams { Id = AuthUserId });
        }

        public IList<Dictionary<string, object>> GetUserInstitutionsByUser()
        {
            return PostWithAuth<User.GetUserParams, IList<Dictionary<string, object>>>("UserInstitution/GetUserInstitutionsByUser", () => new User.GetUserParams { UserID = new Guid(AuthUserId) });
        }

        public IList<Institution> GetInstitutionsByName(string name)
        {
            return PostWithAuth<Institution.GetInstitutionByNameParam, IList<Institution>>("Institution/GetInstitutionByName", () => new Institution.GetInstitutionByNameParam { InstitutionName = name });
        }

        public JobTracker CreateUserInstitution(Guid userId, Guid institutionId, string username, string password, string pin)
        {
            return PostWithAuth<UserInstitution.CreateUserInstitutionParams, JobTracker>("UserInstitution/CreateUserInstitution", () => new UserInstitution.CreateUserInstitutionParams
            {
                UserID = userId,
                InstitutionID = institutionId,
                UserName = username,
                Password = password,
                PIN = pin
            });
        }

        public JobMFA.JobByIDParam RetryAddingUserInstitution(Guid userInstitutionId, Guid institutionId, string userName, string password, string pin)
        {
            return PostWithAuth<UserInstitution.RetryAddingUserInstitutionParams, JobMFA.JobByIDParam>("UserInstitution/RetryAddingUserInstitution", () => new UserInstitution.RetryAddingUserInstitutionParams
            {
                UserInstitutionID = userInstitutionId
            });
        }

        public JobMFA GetJobInformationByID(Guid jobId)
        {
            return PostWithAuth<JobMFA.JobByIDParam, JobMFA>("Job/GetJobInformationByID", () => new JobMFA.JobByIDParam { JobID = jobId });
        }

        public TransmitStatusEnums UpdateJobSecurityAnswer(Guid jobID, string securityAnswer)
        {
            return PostWithAuth<JobMFA.UpdateJobBySecurityAnswerParams, TransmitStatusEnums>("Job/UpdateJobSecurityAnswer", () => new JobMFA.UpdateJobBySecurityAnswerParams
            {
                JobID = jobID,
                SecurityAnswer = securityAnswer
            });
        }

        public TransmitStatusEnums UpdateJobCaptchaInput(Guid jobID, string captchaInput)
        {
            return PostWithAuth<JobMFA.UpdateJobByCaptchaParams, TransmitStatusEnums>("Job/UpdateJobCaptcha", () => new JobMFA.UpdateJobByCaptchaParams
            {
                JobID = jobID,
                CaptchaInput = captchaInput
            });
        }

        public TransmitStatusEnums UpdateJobTokenChoice(Guid jobID, string tokenChoice)
        {
            return PostWithAuth<JobMFA.UpdateJobByTokenParams, TransmitStatusEnums>("Job/UpdateJobTokenInput", () => new JobMFA.UpdateJobByTokenParams
            {
                JobID = jobID,
                TokenChoice = tokenChoice,
                TokenInput = null,
                VerifyPhoneFlag = null,
            });
        }

        public TransmitStatusEnums UpdateJobTokenInput(Guid jobID, string tokenInput)
        {
            return PostWithAuth<JobMFA.UpdateJobByTokenParams, TransmitStatusEnums>("Job/UpdateJobTokenInput", () => new JobMFA.UpdateJobByTokenParams
            {
                JobID = jobID,
                TokenChoice = null,
                TokenInput = tokenInput,
                VerifyPhoneFlag = null,
            });
        }

        public TransmitStatusEnums UpdateJobTokenPhoneVerify(Guid jobID, bool verifyPhoneFlag)
        {
            return PostWithAuth<JobMFA.UpdateJobByTokenParams, TransmitStatusEnums>("Job/UpdateJobTokenInput", () => new JobMFA.UpdateJobByTokenParams
            {
                JobID = jobID,
                TokenChoice = null,
                TokenInput = null,
                VerifyPhoneFlag = verifyPhoneFlag,
            });
        }

        public IList<UserInstitutionAccount> GetUserInstitutionAccounts(Guid userInstitutionId)
        {
            return PostWithAuth<UserInstitution, IList<UserInstitutionAccount>>("UserInstitution/GetUserInstitutionAccounts", () => new UserInstitution { UserInstitutionID = userInstitutionId });
        }

        public JobMFA.JobByIDParam RefreshUserInstitutionAccount(Guid userInstitutionAccountId)
        {
            return PostWithAuth<UserInstitutionAccount, JobMFA.JobByIDParam>("UserInstitutionAccount/RefreshUserInstitutionAccount", () => new UserInstitutionAccount { AccountID = userInstitutionAccountId });
        }

        public IList<Transaction> GetTransactionsByTransactionDate(Guid userInstitutionAccountId, DateTime startDate, DateTime endDate)
        {
            return PostWithAuth<Transaction.GetTransactionByDateParams, IList<Transaction>>("Transaction/GetTransactionsByTransactionDate", () => new Transaction.GetTransactionByDateParams
            {
                AccountID = userInstitutionAccountId,
                StartDate = startDate,
                EndDate = endDate
            });
        }
    }
}
