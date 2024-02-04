using FinanceApi.Data;
using FinanceApi.Enums;
using FinanceApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.Intrinsics.X86;

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
            SeedDatabase();
        }

        public void SeedDatabase()
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
                        Date = DateTime.Now.AddDays(-incomeNr),
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
                        Date = DateTime.Now.AddDays(-expenseNr),
                        DocumentUrl = $"URL-{expenseNr}",
                        User = user,
                    };
                    expenses.Add(expense);
                    ++expenseNr;
                }

                var goals = new List<Goal>();
                for (int j = 1; j< 5; ++j)
                {

                    var goal = new Goal()
                    {
                        Id = goalNr,
                        Title = $"Title-{goalNr}",
                        Description = $"Description-{goalNr}",
                        Amount = 1000 * goalNr,
                        Currency = goalNr % 2 == 0 ? "EUR" : "USD",
                        StartDate = DateTime.Now.AddDays(-goalNr),
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
                        StartDate = DateTime.Now.AddDays(-budgetNr),
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
                if(i > 1 && i < 4)
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









            dataContext.SaveChanges();
        }
    }
}
