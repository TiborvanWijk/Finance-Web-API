using FinanceApi.Enums;
using System.Text.Json.Serialization;

namespace FinanceApi.Data.Dtos
{
    public class BudgetManageDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal LimitAmount { get; set; }
        public string Currency { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Urgency Urgency { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
