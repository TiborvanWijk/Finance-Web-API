using FinanceApi.Enums;

namespace FinanceApi.Models
{
    public class Expense
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string DocumentUrl { get; set; }
        public Urgency Urgency { get; set; }
        public DateTime Date { get; set; }
        public ICollection<Category> Categories { get; set; }
    }
}
