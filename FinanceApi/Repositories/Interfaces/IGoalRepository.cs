﻿using FinanceApi.Models;

namespace FinanceApi.Repositories.Interfaces
{
    public interface IGoalRepository
    {
        Goal GetById(int goalId, bool tracking);
        ICollection<Goal> GetAllOfUser(string userId);
        ICollection<Goal> GetAllOfUserByCategoryId(string userId, int categoryId);
        bool HasGoals(string userId);
        bool ExistsById(string userId, int goalId);
        bool Create(Goal goal);
        bool Update(Goal goal);
        bool Delete(Goal goal);
        bool Save();
        bool ExistsByTitle(string userId, string title);
        bool AddCategory(GoalCategory goalCategory);
        bool DeleteGoalCategoryWithId(string userId, int categoryId, int goalId);
    }
}
