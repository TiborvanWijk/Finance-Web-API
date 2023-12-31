﻿using FinanceApi.Data.Dtos;
using FinanceApi.Models;

namespace FinanceApi.Services.Interfaces
{
    public interface ICategoryService
    {
        Category GetById(int categoryId, bool tracking);
        ICollection<Category> GetAllOfUser(string userId);
        bool ExistsById(string userId, int id);
        bool ExistsBytitle(string userId, string title);
        bool Create(User user, CategoryDto categoryDto, out int errorCode, out string errorMessage);
        bool Update(User user, CategoryDto categoryDto, out int errorCode, out string errorMessage);
        bool Delete(Category category);
        bool TryGetCategoriesSortedByExpenseAmount(User user, out ICollection<Category> categories, out int errorCode, out string errorMessage);
    }
}
