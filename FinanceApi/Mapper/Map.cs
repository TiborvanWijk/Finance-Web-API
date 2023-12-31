﻿using FinanceApi.Data.Dtos;
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
                Balance = userDto.Balance,
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
                Balance = user.Balance,
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
                IsPaid = incomeDto.IsPaid,
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
                IsPaid = income.IsPaid,
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
                IsPaid = expenseDto.IsPaid,
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
                IsPaid = expense.IsPaid,
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

        internal static Goal ToGoal(GoalDto goalDto)
        {
            var goal = new Goal()
            {
                Id = goalDto.Id,
                Title = goalDto.Title,
                Description = goalDto.Description,
                Amount = goalDto.Amount,
                Currency = goalDto.Currency,
                Progress = goalDto.Progress,
                TargetDate = goalDto.TargetDate,
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
                Progress = goal.Progress,
                TargetDate = goal.TargetDate,
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
                TargetDate = goal.TargetDate
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
                TargetDate = goalManageDto.TargetDate
            };

            return goal;
        }
    }
}