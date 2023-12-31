namespace FinanceApi.Models
{
    public class Income
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description{ get; set; }
        public string DocumentUrl { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public bool Status { get; set; }
        public DateTime Date { get; set; }
        public User User { get; set; }
        public ICollection<IncomeCategory> IncomeCategories { get; set; }
    }
}
