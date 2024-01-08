using FinanceApi.Data;
using FinanceApi.Models;
using FinanceApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Repositories
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly DataContext dataContext;

        public ExpenseRepository(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public bool AddCategory(ExpenseCategory expenseCategory)
        {
            dataContext.Add(expenseCategory);
            return Save();
        }

        public bool Create(Expense expense)
        {
            dataContext.Expenses.Add(expense);
            return Save();
        }

        public bool Delete(Expense expense)
        {
            dataContext.Expenses.Remove(expense);
            return Save();
        }

        public bool ExistsById(string userId, int expenseId)
        {
            return dataContext.Expenses.Any(e => e.Id == expenseId && e.User.Id.Equals(userId));
        }

        public ICollection<Expense> GetAllOfUser(string userId)
        {
            return dataContext.Expenses.Where(e => e.User.Id.Equals(userId)).ToList();
        }

        public Expense GetById(int expenseId, bool tracking)
        {
            if(tracking)
            {
                return dataContext.Expenses.FirstOrDefault(e => e.Id == expenseId);
            }
            return dataContext.Expenses.AsNoTracking().FirstOrDefault(e => e.Id == expenseId);
        }

        public bool Save()
        {
            var saved = dataContext.SaveChanges();
            return saved > 0;
        }

        public bool Update(Expense expense)
        {
            dataContext.Expenses.Update(expense);
            return Save();
        }
    }
}
