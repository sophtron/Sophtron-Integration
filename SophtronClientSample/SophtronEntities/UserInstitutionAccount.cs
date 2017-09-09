using System;

namespace SophtronEntities
{
    public enum UserInstitutionAccountStatus
    {
        Tracked,
        Untracked
    }
    public class UserInstitutionAccount
    {
        public Guid UserInstitutionID { get; set; }
        public Guid AccountID { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string AccountType { get; set; }
        public decimal? Balance { get; set; }
        public decimal? AvailableBalance { get; set; }
        public string BalanceCurrency { get; set; }
        public DateTime? LastUpdated { get; set; }
        public DateTime? DueDate { get; set; }
        public UserInstitutionAccountStatus Status { get; set; }
    }
}
