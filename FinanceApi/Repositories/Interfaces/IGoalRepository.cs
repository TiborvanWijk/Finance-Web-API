using FinanceApi.Models;

namespace FinanceApi.Repositories.Interfaces
{
    public interface IGoalRepository
    {
        Goal GetById(int goalId);
        ICollection<Goal> GetAllOfUser(string userId);
        bool HasGoals(string userId);
        bool Exists(string userId, int goalId);
        bool Create(Goal goal);
        bool Update(Goal goal);
        bool Delete(Goal goal);
        bool Save();

    }
}
