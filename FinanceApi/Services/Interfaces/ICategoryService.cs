using FinanceApi.Data.Dtos;
using FinanceApi.Models;

namespace FinanceApi.Services.Interfaces
{
    public interface ICategoryService
    {
        bool TryGetCategoryDtosOrderedOrDefault(string userId, out ICollection<CategoryDto> categories, out int errorCode,
            out string errorMessage, string ListOrderBy, string listDir);
        bool ExistsById(string userId, int id);
        bool ExistsBytitle(string userId, string title);
        bool Create(User user, CategoryManageDto categoryManageDto, out int errorCode, out string errorMessage);
        bool Update(User user, CategoryManageDto categoryDto, out int errorCode, out string errorMessage);
        bool TryDelete(User user, int categoryId, out int errorCode, out string errorMessage);
    }
}
