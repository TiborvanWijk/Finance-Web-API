﻿using FinanceApi.Models;
using FinanceApi.Repositories.Interfaces;
using FinanceApi.Services.Interfaces;

namespace FinanceApi.Services
{
    public class IncomeService : IIncomeService
    {
        private readonly IIncomeRepository incomeRepository;

        public IncomeService(IIncomeRepository incomeRepository)
        {
            this.incomeRepository = incomeRepository;
        }
        public bool Create(Income income)
        {
            return incomeRepository.Create(income);
        }

        public bool Delete(Income income)
        {
            throw new NotImplementedException();
        }

        public bool Exists(int incomeId)
        {
            throw new NotImplementedException();
        }

        public ICollection<Income> GetAllByUserId(string userId)
        {
            return incomeRepository.GetAllByUserId(userId);
        }

        public Income GetById(int incomeId)
        {
            throw new NotImplementedException();
        }

        public bool Update(Income income)
        {
            throw new NotImplementedException();
        }
    }
}
