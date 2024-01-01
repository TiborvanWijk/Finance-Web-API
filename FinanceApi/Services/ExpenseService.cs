using FinanceApi.Models;
using FinanceApi.Repositories.Interfaces;
using FinanceApi.Services.Interfaces;

namespace FinanceApi.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IExpenseRepository expenseRepository;

        public ExpenseService(IExpenseRepository expenseRepository)
        {
            this.expenseRepository = expenseRepository;
        }

        public bool AddCategory(ExpenseCategory expenseCategory)
        {
            return expenseRepository.AddCategory(expenseCategory);
        }

        public bool Create(Expense expense)
        {
            return expenseRepository.Create(expense);
        }

        public bool Delete(Expense expense)
        {
            return expenseRepository.Delete(expense);
        }

        public bool Exists(int expenseId)
        {
            return expenseRepository.Exists(expenseId);
        }

        public ICollection<Expense> GetAllOfUser(string userId)
        {
            return expenseRepository.GetAllOfUser(userId);
        }

        public Expense GetById(int expenseId)
        {
            return expenseRepository.GetById(expenseId);
        }

        public bool Update(Expense expense)
        {
            return expenseRepository.Update(expense);
        }
    }
}
