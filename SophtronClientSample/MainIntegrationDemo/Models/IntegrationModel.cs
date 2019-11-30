using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainOAuthDemo.Models
{
    public class IntegrationEventCallback
    {
        public string Status { get; set; }
        public string EndUserId { get; set; }
        public string UserInstitutionID { get; set; }
    }
}