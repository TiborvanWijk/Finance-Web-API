namespace FinanceApi.Models
{
    public class IncomeCategory
    {
        public int IncomeId { get; set; }
        public int CategoryId { get; set; }
        public Income Income { get; set; }
        public Category Category { get; set; }
    }
}
