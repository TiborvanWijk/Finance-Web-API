using FinanceApi.Enums;

namespace FinanceApi.Models
{
    public class Budget
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal LimitAmount { get; set; }
        public Urgency Urgency { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ICollection<Category> Category { get; set; }
    }
}
