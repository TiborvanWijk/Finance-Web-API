using FinanceApi.Data.Dtos;
using FinanceApi.Models;

namespace FinanceApi.Services.Interfaces
{
    public interface IGoalService
    {
        bool TryGetAllOrderedOrDefault(string userId, out ICollection<Goal> goals, out int errorCode,
            out string errorMessage, DateTime? startDate, DateTime? endDate, string? listOrderBy, string? listDir, int? categoryId);
        decimal GetProgressAmountOfGoal(string userId, int goalId);
        bool ValidateGoal(GoalManageDto goalDto, out int errorCode, out string errorMessage);
        bool Update(User user, GoalManageDto goalDto, out int errorCode, out string errorMessage);
        bool Create(User user, GoalManageDto goalDto, out int errorCode, out string errorMessage);
        bool AddCategories(string userId, int goalId, ICollection<int> categoryIds, out string errorMessage, out int errorCode);
        bool TryDeleteGoal(User user, int goalId, out int errorCode, out string errorMessage);
        bool TryRemoveCategory(User user, int goalId, int categoryId, out int errorCode, out string errorMessage);
    }
}
