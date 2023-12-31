﻿using FinanceApi.Models;

namespace FinanceApi.Repositories.Interfaces
{
    public interface IIncomeRepository
    {
        Income GetById(int incomeId, bool tracking);
        ICollection<Income> GetAllOfUser(string userId);
        bool ExistsById(string userId, int incomeId);
        bool Create(Income income);
        bool Update(Income income);
        bool Delete(Income income);
        bool Save();
        bool AddCategory(IncomeCategory incomeCategory);
    }
}
