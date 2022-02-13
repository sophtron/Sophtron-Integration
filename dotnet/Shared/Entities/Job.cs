using System;

namespace SophtronEntities
{
    public class JobMFA
    {
        public Guid JobID { get; set; }
        public bool? SuccessFlag { get; set; }
        public string SecurityQuestion { get; set; }
        public string SecurityAnswer { get; set; }
        public string TokenMethod { get; set; }
        public string TokenChoice { get; set; }
        public string TokenInput { get; set; }
        public bool? TokenSentFlag { get; set; }
        public string TokenRead { get; set; }
        public bool? VerifyPhoneFlag { get; set; }
        public string CaptchaImage { get; set; }
        public string CaptchaInput { get; set; }
        public string LastStep { get; set; }
        public string LastStatus { get; set; }

        public class JobByIDParam
        {
            public Guid JobID;
        }
        public class UpdateJobByLastStatusParams
        {
            public Guid JobID;
            public string LastStep;
            public string LastStatus;
        }

        public class UpdateJobBySecurityAnswerParams
        {
            public Guid JobID;
            public string SecurityAnswer;
        }

        public class UpdateJobByCaptchaParams
        {
            public Guid JobID;
            public string CaptchaInput;
        }

        public class UpdateJobByTokenParams
        {
            public Guid JobID;
            public string TokenChoice;
            public string TokenInput;
            public bool? VerifyPhoneFlag;
        }
    }
    public class JobTracker
    {
        public Guid JobID { get; set; }
        public Guid UserInstitutionID { get; set; }

    }
}
