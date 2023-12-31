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
        public bool Create(Expense expense)
        {
            throw new NotImplementedException();
        }

        public bool Delete(Expense expense)
        {
            throw new NotImplementedException();
        }

        public bool Exists(int expenseId)
        {
            throw new NotImplementedException();
        }

        public ICollection<Expense> GetAllOfUser(int userId)
        {
            throw new NotImplementedException();
        }

        public Expense GetById(int expenseId)
        {
            throw new NotImplementedException();
        }

        public bool Update(Expense expense)
        {
            throw new NotImplementedException();
        }
    }
}
