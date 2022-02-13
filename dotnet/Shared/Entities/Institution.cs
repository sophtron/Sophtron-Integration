using System;

namespace SophtronEntities
{
    public class Institution
    {
        public Guid InstitutionID { get; set; }
        public string InstitutionName { get; set; }
        public string URL { get; set; }

        public class GetInstitutionByNameParam
        {
            public string InstitutionName;
        }
    }
}
