namespace FinanceApi.Data.Dtos
{
    public class IncomeDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string DocumentUrl { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public bool IsPaid { get; set; }
        public DateTime Date { get; set; }
    }
}
