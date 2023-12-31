using FinanceApi.Models;

namespace FinanceApi.Services.Interfaces
{
    public interface IGoalService
    {
        Goal GetById(int goalId);
        ICollection<Goal> GetAllOfUser(int userId);
        bool HasGoals(int userId);
        bool Exists(int goalId);
        bool Create(Goal goal);
        bool Update(Goal goal);
        bool Delete(Goal goal);
    }
}
