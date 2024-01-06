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
        bool ValidateGoal(GoalManageDto goalDto, out int errorCode, out string errorMessage);
        bool AddCategory(GoalCategory goalCategory);
        bool Delete(Goal goal);
        bool Update(User user, GoalManageDto goalDto, out int errorCode, out string errorMessage);
        bool Create(User user, GoalManageDto goalDto, out int errorCode, out string errorMessage);
        bool AddCategories(string userId, int goalId, ICollection<int> categoryIds, out string errorMessage, out int errorCode);
    }
}
