﻿using FinanceApi.Data;
using FinanceApi.Models;
using FinanceApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Repositories
{
    public class IncomeRepository : IIncomeRepository
    {
        private readonly DataContext dataContext;

        public IncomeRepository(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public bool AddCategory(IncomeCategory incomeCategory)
        {
            dataContext.IncomeCategories.Add(incomeCategory);
            return Save();
        }

        public bool Create(Income income)
        {
            dataContext.Incomes.Add(income);
            return Save();
        }

        public bool Delete(Income income)
        {
            dataContext.Incomes.Remove(income);
            return Save();
        }

        public bool ExistsById(string userId, int incomeId)
        {
            return dataContext.Incomes.Any(i => i.Id == incomeId && i.User.Id.Equals(userId));
        }

        public ICollection<Income> GetAllOfUser(string userId)
        {
            return dataContext.Incomes.Where(i => i.User.Id == userId).ToList();
        }

        public Income GetById(int incomeId, bool tracking)
        {
            if(tracking)
            {
                return dataContext.Incomes.FirstOrDefault(i => i.Id == incomeId);
            }
            return dataContext.Incomes.AsNoTracking().FirstOrDefault(i => i.Id == incomeId);
        }

        public bool Save()
        {
            var saved = dataContext.SaveChanges();
            return saved > 0;
        }

        public bool Update(Income income)
        {
            dataContext.Incomes.Update(income);
            return Save();
        }
    }
}
