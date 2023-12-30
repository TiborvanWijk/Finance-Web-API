using FinanceApi.Models;
using FinanceApi.Repositories.Interfaces;

namespace FinanceApi.Repositories
{
    public class ExpenseRepository : IExpenseRepository
    {
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

        public bool Save()
        {
            throw new NotImplementedException();
        }

        public bool Update(Expense expense)
        {
            throw new NotImplementedException();
        }
    }
}
