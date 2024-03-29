﻿using FinanceApi.Models;

namespace FinanceApi.Repositories.Interfaces
{
    public interface IIncomeRepository
    {
        Income GetById(int incomeId, bool tracking);
        ICollection<Income> GetAllOfUser(string userId);
        Task<ICollection<Income>> GetAllOfUserAsync(string userId);
        ICollection<Income> GetAllOfUserByCategoryId(string userId, int categoryId);
        ICollection<Income> GetAllOfUserByGoalId(string userId, int goalId);
        bool ExistsById(string userId, int incomeId);
        bool Create(Income income);
        bool Update(Income income);
        bool Delete(Income income);
        bool Save();
        bool AddCategory(IncomeCategory incomeCategory);
        bool DeleteIncomeCategoryWithId(string userId, int categoryId, int incomeId);
    }
}
