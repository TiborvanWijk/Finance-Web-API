using Microsoft.AspNetCore.Identity;

namespace FinanceApi.Models
{
    public class User : IdentityUser
    {
        public string Id { get; set; }
        public decimal Balance { get; set; } = 0;
        public string Currency { get; set; } = "EUR";
        public ICollection<Expense> Expenses{ get; set; }
        public ICollection<Income> Incomes { get; set; }
        public ICollection<Budget> Budgets { get; set; }
        public ICollection<Goal> Goals { get; set; }
    }
}
