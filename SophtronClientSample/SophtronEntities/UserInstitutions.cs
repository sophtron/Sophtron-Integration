using System;

namespace SophtronEntities
{
    public class UserInstitution
    {
        public Guid UserID { get; set; }
        public Guid InstitutionID { get; set; }
        public Guid UserInstitutionID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PIN { get; set; }
        

        public class CreateUserInstitutionParams
        {
            public Guid UserID;
            public Guid InstitutionID;
            public string UserName;
            public string Password;
            public string PIN;
        }

        public class UpdateUserInstitutionParams
        {
            public Guid UserInstitutionID;
            public string UserName;
            public string Password;
            public string PIN;
        }

        public class RetryAddingUserInstitutionParams
        {
            public Guid UserInstitutionID;
        }

        public class GetUserInstitutionParams
        {
            public Guid UserID;
            public Guid InstitutionID;
        }
        public class DeleteUserInstitutionParams
        {
            public Guid UserInstitutionID;
        }

        public class GetUserInstitutionByIDParams
        {
            public Guid UserInstitutionID;
        }

        public class GetUserInstitutionAccountsParams
        {
            public Guid UserInstitutionID;
        }
    }
}
