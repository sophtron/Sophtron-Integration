using System;

namespace SophtronEntities
{
    public class Transaction
    {
        public Guid UserInstitutionAccountID { get; set; }
        public Guid TransactionID { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public decimal? Amount { get; set; }
        public string Currency { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? TransactionDate { get; set; }
        public DateTime? PostDate { get; set; }
        public string Description { get; set; }
        public string Merchant { get; set; }
        public string Category { get; set; }
        public string CheckNum { get; set; }
        public string Memo { get; set; }

        public class GetTransactionByDateParams
        {
            public Guid AccountID;
            public DateTime StartDate;
            public DateTime EndDate;
        }
    }
}
