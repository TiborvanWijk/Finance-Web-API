namespace FinanceApi.Models
{
    public class Goal
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public decimal Progress { get; set; }
        public DateTime TargetDate { get; set; }
        public User User { get; set; }
        public ICollection<GoalCategory> GoalCategories { get; set; }
    }
}
