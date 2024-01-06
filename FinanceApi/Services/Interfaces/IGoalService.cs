using FinanceApi.Data.Dtos;
using FinanceApi.Models;

namespace FinanceApi.Services.Interfaces
{
    public interface IGoalService
    {
        Goal GetById(int goalId);
        ICollection<Goal> GetAllOfUser(string userId);
        bool HasGoals(string userId);
        bool ExistsById(string userId, int goalId);
        bool ExistsByTitle(string userId, string title);
        bool Update(Goal goal);
        bool Delete(Goal goal);
        bool Create(User user, GoalDto goalDto, out int errorCode, out string errorMessage);
    }
}
