using FinanceApi.Models;

namespace FinanceApi.Repositories.Interfaces
{
    public interface IGoalRepository
    {
        Goal GetById(int goalId);
        ICollection<Goal> GetAllOfUser(int userId);
        bool HasGoals(int userId);
        bool Exists(int goalId);
        bool Create(Goal goal);
        bool Update(Goal goal);
        bool Delete(Goal goal);
        bool Save();

    }
}
