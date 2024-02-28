using FinanceApi.Data;
using FinanceApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceApi.Test.TestDatabase
{
    internal abstract class DatabaseActions
    {
        private static ICollection<Income> incomes;
        private static ICollection<Goal> goals;
        private static ICollection<Expense> expenses;
        private static ICollection<Category> categories;
        private static ICollection<Budget> budgets;
        private static ICollection<BudgetCategory> budgetCategories;
        private static ICollection<ExpenseCategory> expenseCategories;
        private static ICollection<GoalCategory> goalCategories;
        private static ICollection<IncomeCategory> incomeCategories;
        private static ICollection<AuthorizedUserJoin> authorizedUsers;
        private static ICollection<AuthorizeUserInvite> authorizeUserInvite;
        private static ICollection<User> users;

        public static void SaveState(DataContext dataContext)
        {
            incomes = dataContext.Incomes.ToList();
            goals = dataContext.Goals.ToList();
            expenses = dataContext.Expenses.ToList();
            categories = dataContext.Categories.ToList();
            budgets = dataContext.Budgets.ToList();
            budgetCategories = dataContext.BudgetCategories.ToList();
            expenseCategories = dataContext.ExpenseCategories.ToList();
            goalCategories = dataContext.GoalCategories.ToList();
            incomeCategories = dataContext.IncomeCategories.ToList();
            authorizedUsers = dataContext.AuthorizedUsers.ToList();
            authorizeUserInvite = dataContext.AuthorizeUserInvite.ToList();
            users = dataContext.Users.ToList();
        }

        public static void ResetState(DataContext dataContext)
        {
            dataContext.RemoveRange(dataContext.Incomes);
            dataContext.RemoveRange(dataContext.Goals);
            dataContext.RemoveRange(dataContext.Expenses);
            dataContext.RemoveRange(dataContext.Categories);
            dataContext.RemoveRange(dataContext.Budgets);
            dataContext.RemoveRange(dataContext.BudgetCategories);
            dataContext.RemoveRange(dataContext.ExpenseCategories);
            dataContext.RemoveRange(dataContext.GoalCategories);
            dataContext.RemoveRange(dataContext.IncomeCategories);
            dataContext.RemoveRange(dataContext.AuthorizedUsers);
            dataContext.RemoveRange(dataContext.AuthorizeUserInvite);
            dataContext.RemoveRange(dataContext.Users);

            dataContext.AddRange(incomes);
            dataContext.AddRange(goals);
            dataContext.AddRange(expenses);
            dataContext.AddRange(categories);
            dataContext.AddRange(budgets);
            dataContext.AddRange(budgetCategories);
            dataContext.AddRange(expenseCategories);
            dataContext.AddRange(goalCategories);
            dataContext.AddRange(incomeCategories);
            dataContext.AddRange(authorizedUsers);
            dataContext.AddRange(authorizeUserInvite);
            dataContext.AddRange(users);

            dataContext.SaveChanges();
        }

    }
}
