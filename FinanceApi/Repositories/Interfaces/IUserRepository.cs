﻿using FinanceApi.Models;

namespace FinanceApi.Repositories.Interfaces
{
    public interface IUserRepository
    {
        User GetById(string userId, bool tracking);
        User GetByUsername(string username);
        bool ExistsById(string userId);
        bool ExistsByUsername(string username);
        bool Create(User user);
        bool Update(User user);
        bool Delete(User user);
        bool Save();

    }
}
