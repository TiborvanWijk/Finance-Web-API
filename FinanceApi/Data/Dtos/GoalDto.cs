namespace FinanceApi.Data.Dtos
{
    public class GoalDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public decimal Progress { get; set; }
        public DateTime TargetDate { get; set; }
    }
}
