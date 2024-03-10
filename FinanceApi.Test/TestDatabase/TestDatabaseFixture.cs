using FinanceApi.Data;
using FinanceApi.Enums;
using FinanceApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace FinanceApi.Test.TestDatabase
{
    public class TestDatabaseFixture
    {
        public DataContext dataContext { get; }
        public TestDatabaseFixture()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var builder = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase("TestDatabase")
                .UseInternalServiceProvider(serviceProvider);
            dataContext = new TestDatacontext(builder.Options);
            SeedDatabase(dataContext);
        }
        private static bool seedingHasBeenDone = false;
        private static ICollection<User> users;
        private static ICollection<IdentityRole> roles;
        private static ICollection<AuthorizedUserJoin> authorizedUsers;
        private static ICollection<AuthorizeUserInvite> authorizeUserInvites;
        private static ICollection<BudgetCategory> budgetCategories;
        private static ICollection<Budget> budgets;
        private static ICollection<Category> categories;
        private static ICollection<ExpenseCategory> expenseCategories;
        private static ICollection<Expense> expenses;
        private static ICollection<GoalCategory> goalCategories;
        private static ICollection<Goal> goals;
        private static ICollection<IncomeCategory> incomeCategories;
        private static ICollection<Income> incomes;
        private static object locker = new object();


        public static void SeedDatabase(DataContext dataContext)
        {

            lock (locker)
            {

                if (!seedingHasBeenDone)
                {
                    SeedDataManualy(dataContext);
                    SaveData(dataContext);
                    seedingHasBeenDone = true;
                    return;
                }
                ResetDataBase(dataContext);
                SeedWithStoredData(dataContext);
            }
        }

        private static void SaveData(DataContext dataContext)
        {
            users = dataContext.Users.ToList();
            roles = dataContext.Roles.ToList();
            authorizedUsers = dataContext.AuthorizedUsers.ToList();
            authorizeUserInvites = dataContext.AuthorizeUserInvite.ToList();
            budgetCategories = dataContext.BudgetCategories.ToList();
            budgets = dataContext.Budgets.ToList();
            categories = dataContext.Categories.ToList();
            expenseCategories = dataContext.ExpenseCategories.ToList();
            expenses = dataContext.Expenses.ToList();
            goalCategories = dataContext.GoalCategories.ToList();
            goals = dataContext.Goals.ToList();
            incomeCategories = dataContext.IncomeCategories.ToList();
            incomes = dataContext.Incomes.ToList();
        }


        private static void ResetDataBase(DataContext dataContext)
        {
            dataContext.RemoveRange(dataContext.Users);
            dataContext.RemoveRange(dataContext.Roles);
            dataContext.RemoveRange(dataContext.AuthorizedUsers);
            dataContext.RemoveRange(dataContext.AuthorizeUserInvite);
            dataContext.RemoveRange(dataContext.BudgetCategories);
            dataContext.RemoveRange(dataContext.BudgetCategories);
            dataContext.RemoveRange(dataContext.Budgets);
            dataContext.RemoveRange(dataContext.Categories);
            dataContext.RemoveRange(dataContext.ExpenseCategories);
            dataContext.RemoveRange(dataContext.Expenses);
            dataContext.RemoveRange(dataContext.GoalCategories);
            dataContext.RemoveRange(dataContext.Goals);
            dataContext.RemoveRange(dataContext.IncomeCategories);
            dataContext.RemoveRange(dataContext.Incomes);
            dataContext.SaveChanges();
        }

        private static void SeedWithStoredData(DataContext dataContext)
        {
            dataContext.AddRange(users);
            dataContext.AddRange(roles);
            dataContext.AddRange(authorizedUsers);
            dataContext.AddRange(authorizeUserInvites);
            dataContext.AddRange(budgetCategories);
            dataContext.AddRange(budgetCategories);
            dataContext.AddRange(budgets);
            dataContext.AddRange(categories);
            dataContext.AddRange(expenseCategories);
            dataContext.AddRange(expenses);
            dataContext.AddRange(goalCategories);
            dataContext.AddRange(goals);
            dataContext.AddRange(incomeCategories);
            dataContext.AddRange(incomes);
            dataContext.SaveChanges();
        }

        private static void SeedDataManualy(DataContext dataContext)
        {
            var passwordHasher = new PasswordHasher<User>();

            var users = new List<User>();
            int incomeNr = 1;
            int expenseNr = 1;
            int goalNr = 1;
            int budgetNr = 1;
            int categoryNr = 1;
            for (int i = 1; i < 5; ++i)
            {
                var user = new User()
                {
                    UserName = $"user{i}@example.com",
                    Email = $"user{i}@example.com",
                    NormalizedEmail = $"USER{i}@EXAMPLE.COM",
                    NormalizedUserName = $"USER{i}@EXAMPLE.COM"
                };
                user.PasswordHash = passwordHasher.HashPassword(user, "Password!2");

                var incomes = new List<Income>();
                for (int j = 1; j < 21; ++j)
                {

                    var income = new Income()
                    {
                        Id = incomeNr,
                        Title = $"Title-{incomeNr}",
                        Description = $"Description-{incomeNr}",
                        Amount = 100 * incomeNr,
                        Currency = incomeNr % 2 == 0 ? "EUR" : "USD",
                        Date = DateTime.Now.AddMonths(-incomeNr),
                        DocumentUrl = $"URL-{incomeNr}",
                        User = user,
                    };
                    incomes.Add(income);
                    ++incomeNr;
                }
                var expenses = new List<Expense>();

                for (int j = 1; j < 21; ++j)
                {

                    var expense = new Expense()
                    {
                        Id = expenseNr,
                        Title = $"Title-{expenseNr}",
                        Description = $"Description-{expenseNr}",
                        Amount = 100 * expenseNr,
                        Currency = expenseNr % 2 == 0 ? "EUR" : "USD",
                        Urgency = (Urgency)(expenseNr % 3),
                        Date = DateTime.Now.AddMonths(-expenseNr),
                        DocumentUrl = $"URL-{expenseNr}",
                        User = user,
                    };
                    expenses.Add(expense);
                    ++expenseNr;
                }

                var goals = new List<Goal>();
                for (int j = 1; j < 6; ++j)
                {

                    var goal = new Goal()
                    {
                        Id = goalNr,
                        Title = $"Title-{goalNr}",
                        Description = $"Description-{goalNr}",
                        Amount = 1000 * goalNr,
                        Currency = goalNr % 2 == 0 ? "EUR" : "USD",
                        StartDate = DateTime.Now.AddMonths(-goalNr),
                        EndDate = DateTime.Now.AddDays(goalNr),
                        User = user
                    };
                    goals.Add(goal);
                    ++goalNr;
                }
                var budgets = new List<Budget>();
                for (int j = 1; j < 5; ++j)
                {

                    var budget = new Budget()
                    {
                        Id = budgetNr,
                        Title = $"Title-{budgetNr}",
                        Description = $"Description-{budgetNr}",
                        LimitAmount = 1000 * budgetNr,
                        Currency = budgetNr % 2 == 0 ? "EUR" : "USD",
                        Urgency = (Urgency)(budgetNr % 3),
                        StartDate = DateTime.Now.AddMonths(-budgetNr),
                        EndDate = DateTime.Now.AddDays(budgetNr),
                        User = user
                    };
                    budgets.Add(budget);
                    ++budgetNr;
                }

                var categories = new List<Category>();
                for (int j = 1; j < 5; ++j)
                {
                    var category = new Category()
                    {
                        Id = categoryNr,
                        Title = $"Title-{categoryNr}",
                        Description = $"Description-{categoryNr}",
                        User = user,
                    };
                    categories.Add(category);
                    ++categoryNr;
                }


                var incomeCategories = new List<IncomeCategory>();
                for (int j = 0; j < incomes.Count; ++j)
                {
                    var incomeCategory = new IncomeCategory()
                    {
                        Income = incomes[j],
                        IncomeId = incomes[j].Id,
                        Category = categories[j % categories.Count],
                        CategoryId = categories[j % categories.Count].Id,
                    };

                    incomeCategories.Add(incomeCategory);
                }
                var expenseCategories = new List<ExpenseCategory>();
                for (int j = 0; j < expenses.Count; ++j)
                {
                    var expenseCategory = new ExpenseCategory()
                    {
                        Expense = expenses[j],
                        ExpenseId = expenses[j].Id,
                        Category = categories[j % categories.Count],
                        CategoryId = categories[j % categories.Count].Id,
                    };

                    expenseCategories.Add(expenseCategory);
                }

                var budgetCategories = new List<BudgetCategory>();
                for (int j = 0; j < budgets.Count; ++j)
                {
                    var budgetCategory = new BudgetCategory()
                    {
                        Budget = budgets[j],
                        BudgetId = budgets[j].Id,
                        Category = categories[j % categories.Count],
                        CategoryId = categories[j % categories.Count].Id,
                    };

                    budgetCategories.Add(budgetCategory);
                }
                var goalCategories = new List<GoalCategory>();
                for (int j = 0; j < goals.Count; ++j)
                {
                    var goalCategory = new GoalCategory()
                    {
                        Goal = goals[j],
                        GoalId = goals[j].Id,
                        Category = categories[j % categories.Count],
                        CategoryId = categories[j % categories.Count].Id,
                    };

                    goalCategories.Add(goalCategory);
                }

                users.Add(user);
                if (i > 1 && i < 4)
                {
                    var userAuthorization = new AuthorizedUserJoin()
                    {
                        AuthorizedUser = user,
                        AuthorizedUserId = user.Id,
                        Owner = users[0],
                        OwnerId = users[0].Id,
                        CanEdit = i % 2 == 1,
                    };
                    dataContext.Add(userAuthorization);
                }

                dataContext.AddRange(users);
                dataContext.AddRange(incomes);
                dataContext.AddRange(expenses);
                dataContext.AddRange(budgets);
                dataContext.AddRange(goals);
                dataContext.AddRange(categories);
                dataContext.AddRange(incomeCategories);
                dataContext.AddRange(expenseCategories);
                dataContext.AddRange(budgetCategories);
                dataContext.AddRange(goalCategories);
            }

            var authInvite = new AuthorizeUserInvite()
            {
                Owner = users[0],
                OwnerId = users[0].Id,
                AuthorizedUser = users[users.Count - 1],
                AuthorizedUserId = users[users.Count - 1].Id,
                Title = "THIS IS A RANDOM TITLE",
                Message = "THIS IS A RANDOM MESSAGE",
            };
            dataContext.AuthorizeUserInvite.Add(authInvite);

            dataContext.SaveChanges();
        }
    }
}
