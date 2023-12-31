namespace FinanceApi.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ICollection<BudgetCategory> BudgetCategories{ get; set; }
        public ICollection<IncomeCategory> IncomeCategories { get; set; }
        public ICollection<ExpenseCategory> ExpenseCategories { get; set; }
        public ICollection<GoalCategory> GoalCategories { get; set; }
    }
}
