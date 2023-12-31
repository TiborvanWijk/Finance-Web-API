namespace FinanceApi.Models
{
    public class GoalCategory
    {
        public int GoalId { get; set; }
        public int CategoryId { get; set; }
        public Goal Goal { get; set; }
        public Category Category { get; set; }
    }
}
