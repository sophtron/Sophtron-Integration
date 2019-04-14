using System;
namespace SophtronEntities
{
    public class User
    {
        public Guid UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AccessKey { get; set; }

        public class GetUserParams
        {
            public Guid UserID;
        }

        public class GetIntegrationKeyParams
        {
            public string Id;
        }
    }
}
