using FinanceApi.Data.Dtos;
using FinanceApi.Models;

namespace FinanceApi.Mapper
{
    public abstract class Map
    {

        internal static User ToUser(UserDto userDto)
        {
            var user = new User()
            {
                Id = userDto.Id,
                UserName = userDto.Username,
                Currency = userDto.Currency,
            };
            return user;
        }

        internal static UserDto ToUserDto(User user)
        {
            var userDto = new UserDto()
            {
                Id = user.Id,
                Username = user.UserName,
                Currency = user.Currency,

            };

            return userDto;
        }

        internal static Income ToIncome(IncomeDto incomeDto)
        {
            var income = new Income()
            {
                Id = incomeDto.Id,
                Title = incomeDto.Title,
                Description = incomeDto.Description,
                Amount = incomeDto.Amount,
                Currency = incomeDto.Currency,
                DocumentUrl = incomeDto.DocumentUrl,
                Date = incomeDto.Date,
            };
            return income;
        }

        internal static IncomeDto ToIncomeDto(Income income)
        {
            var incomeDto = new IncomeDto()
            {
                Id = income.Id,
                Title = income.Title,
                Description = income.Description,
                Currency = income.Currency,
                Amount = income.Amount,
                DocumentUrl = income.DocumentUrl,
                Date = income.Date,
            };
            return incomeDto;
        }

        internal static Expense ToExpense(ExpenseDto expenseDto)
        {
            var expense = new Expense()
            {
                Id = expenseDto.Id,
                Title = expenseDto.Title,
                Description = expenseDto.Description,
                Urgency = expenseDto.Urgency,
                DocumentUrl = expenseDto.DocumentUrl,
                Date = expenseDto.Date,
                Currency = expenseDto.Currency,
                Amount = expenseDto.Amount,
            };
            return expense;
        }


        internal static ExpenseDto ToExpenseDto(Expense expense)
        {
            var expenseDto = new ExpenseDto()
            {
                Id = expense.Id,
                Title = expense.Title,
                Description = expense.Description,
                Urgency = expense.Urgency,
                DocumentUrl = expense.DocumentUrl,
                Date = expense.Date,
                Currency = expense.Currency,
                Amount = expense.Amount,
            };
            return expenseDto;
        }

        internal static Budget ToBudget(BudgetDto budgetDto)
        {
            var budget = new Budget()
            {
                Id = budgetDto.Id,
                Title = budgetDto.Title,
                Description = budgetDto.Description,
                LimitAmount = budgetDto.LimitAmount,
                Currency = budgetDto.Currency,
                Urgency = budgetDto.Urgency,
                StartDate = budgetDto.StartDate,
                EndDate = budgetDto.EndDate,
            };
            return budget;
        }

        internal static BudgetDto ToBudgetDto(Budget budget)
        {
            var budgetDto = new BudgetDto()
            {
                Id = budget.Id,
                Title = budget.Title,
                Description = budget.Description,
                LimitAmount = budget.LimitAmount,
                Currency= budget.Currency,
                Urgency = budget.Urgency,
                StartDate = budget.StartDate,
                EndDate = budget.EndDate,
            };
            return budgetDto;
        }


        internal static Budget ToBudgetFromBudgetManageDto(BudgetManageDto budgetDto)
        {
            var budget = new Budget()
            {
                Id = budgetDto.Id,
                Title = budgetDto.Title,
                Description = budgetDto.Description,
                LimitAmount = budgetDto.LimitAmount,
                Currency = budgetDto.Currency,
                Urgency = budgetDto.Urgency,
                StartDate = budgetDto.StartDate,
                EndDate = budgetDto.EndDate,
            };
            return budget;
        }

        internal static Goal ToGoalFromGoalManageDto(GoalManageDto goalDto)
        {
            var goal = new Goal()
            {
                Id = goalDto.Id,
                Title = goalDto.Title,
                Description = goalDto.Description,
                Amount = goalDto.Amount,
                Currency = goalDto.Currency,
                StartDate = goalDto.StartDate,
                EndDate = goalDto.EndDate,
            };
            return goal;
        }

        internal static Goal ToGoalFromGoalDto(GoalDto goalDto)
        {
            var goal = new Goal()
            {
                Id = goalDto.Id,
                Title = goalDto.Title,
                Description = goalDto.Description,
                Amount = goalDto.Amount,
                Currency = goalDto.Currency,
                StartDate= goalDto.StartDate,
                EndDate = goalDto.EndDate,
            };
            return goal;
        }

        internal static GoalDto ToGoalDto(Goal goal)
        {
            var goalDto = new GoalDto()
            {
                Id = goal.Id,
                Title = goal.Title,
                Description = goal.Description,
                Amount = goal.Amount,
                Currency = goal.Currency,
                StartDate = goal.StartDate,
                EndDate = goal.EndDate,
            };
            return goalDto;
        }

        internal static GoalManageDto ToGoalManageDto(Goal goal)
        {
            var goalDto = new GoalManageDto()
            {
                Id = goal.Id,
                Title = goal.Title,
                Description = goal.Description,
                Amount = goal.Amount,
                Currency = goal.Currency,
                StartDate = goal.StartDate,
                EndDate = goal.EndDate
            };

            return goalDto;
        }

        internal static Category ToCategory(CategoryDto categoryDto)
        {
            var category = new Category()
            {
                Id = categoryDto.Id,
                Title = categoryDto.Title,
                Description = categoryDto.Description,
            };
            return category;
        }

        internal static CategoryDto ToCategoryDto(Category category)
        {
            var categoryDto = new CategoryDto()
            {
                Id = category.Id,
                Title = category.Title,
                Description = category.Description,
            };
            return categoryDto;
        }

        internal static Goal ToGoalFromManageDto(GoalManageDto goalManageDto)
        {
            var goal = new Goal()
            {
                Id = goalManageDto.Id,
                Title = goalManageDto.Title,
                Description = goalManageDto.Description,
                Amount = goalManageDto.Amount,
                Currency = goalManageDto.Currency,
                StartDate = goalManageDto.StartDate,
                EndDate = goalManageDto.EndDate
            };

            return goal;
        }

        internal static AuthorizeUserInviteDto ToAuthorizeUserInviteDto(AuthorizeUserInvite authorizeUserInvite)
        {
            var authorizeUserInviteDto = new AuthorizeUserInviteDto()
            {
                Title = authorizeUserInvite.Title,
                Message = authorizeUserInvite.Message,
            };

            return authorizeUserInviteDto;
        }
    }
}