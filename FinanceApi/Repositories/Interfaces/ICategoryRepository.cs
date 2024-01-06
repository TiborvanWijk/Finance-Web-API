﻿using FinanceApi.Models;

namespace FinanceApi.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Category GetById(int categoryId);
        ICollection<Category> GetAllOfUser(string userId);
        ICollection<ExpenseCategory> GetExpenseCategories(string userId, int expenseId);
        ICollection<IncomeCategory> GetIncomeCategories(string userId, int incomeId);
        ICollection<BudgetCategory> GetBudgetCategories(string userId, int budgetId);
        bool ExistsById(string userId, int id);
        bool ExistsBytitle(string userId, string title);
        bool Create(Category category);
        bool Update(Category category);
        bool Delete(Category category);
        bool Save();
        ICollection<GoalCategory> GetGoalCategories(string userId, int goalId);
    }
}
