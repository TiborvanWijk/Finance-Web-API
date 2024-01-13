using Microsoft.AspNetCore.Identity;
using System.Globalization;

namespace FinanceApi.Models
{
    public class User : IdentityUser
    {
        public string Currency { get; set; } = RegionInfo.CurrentRegion.ISOCurrencySymbol;
        public ICollection<Expense> Expenses{ get; set; }
        public ICollection<Income> Incomes { get; set; }
        public ICollection<Budget> Budgets { get; set; }
        public ICollection<Goal> Goals { get; set; }
    }
}
