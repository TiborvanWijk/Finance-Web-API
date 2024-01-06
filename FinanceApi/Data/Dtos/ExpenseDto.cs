using FinanceApi.Enums;
using System.Text.Json.Serialization;

namespace FinanceApi.Data.Dtos
{
    public class ExpenseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsPaid { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string DocumentUrl { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Urgency Urgency { get; set; }
        public DateTime Date { get; set; }
    }
}
