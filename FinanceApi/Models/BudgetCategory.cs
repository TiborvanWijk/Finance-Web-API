namespace FinanceApi.Models
{
    public class BudgetCategory
    {
        public int BudgetId { get; set; }
        public int CategoryId { get; set; }
        public Budget Budget { get; set; }
        public Category Category { get; set; }
    }
}
