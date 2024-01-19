namespace FinanceApi.Models
{
    public class FinancialPartners
    {
        public string UserOneId { get; set; }
        public string UserTwoId { get; set; }
        public User UserOne { get; set; }
        public User UserTwo { get; set; }
    }
}
