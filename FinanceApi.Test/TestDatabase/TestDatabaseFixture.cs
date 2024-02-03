using FinanceApi.Data;
using FinanceApi.Enums;
using FinanceApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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

            var user1 = new User { UserName = "user1@example.com", Email = "user1@example.com" };
            var user2 = new User { UserName = "user2@example.com", Email = "user2@example.com" };
            var user3 = new User { UserName = "user3@example.com", Email = "user3@example.com" };
            
            user1.NormalizedEmail = user1.Email.Normalize();
            user2.NormalizedEmail = user2.Email.Normalize();
            user1.NormalizedUserName = user1.UserName.Normalize();
            user2.NormalizedUserName = user2.UserName.Normalize();
            user1.PasswordHash = passwordHasher.HashPassword(user1, "PASSWORD");
            user2.PasswordHash = passwordHasher.HashPassword(user2, "PASSWORD!");

            var incomes = new List<Income>();
            for (int i = 1; i < 21; ++i)
            {

                var income = new Income()
                {
                    Id = i,
                    Title = $"Title-{i}",
                    Description = $"Description-{i}",
                    Amount = 100 * i,
                    Currency = i % 2 == 0 ? "EUR" : "USD",
                    Date = DateTime.Now.AddDays(-i),
                    DocumentUrl = $"URL-{i}",
                    User = i % 2 == 0 ? user1 : user2,
                };
                incomes.Add(income);
            }
            var expenses = new List<Expense>();
            for (int i = 1; i < 21; ++i)
            {

                var expense = new Expense()
                {
                    Id = i,
                    Title = $"Title-{i}",
                    Description = $"Description-{i}",
                    Amount = 100 * i,
                    Currency = i % 2 == 0 ? "EUR" : "USD",
                    Urgency = (Urgency)(i % 3),
                    Date = DateTime.Now.AddDays(-i),
                    DocumentUrl = $"URL-{i}",
                    User = i % 2 == 0 ? user1 : user2,
                };
                expenses.Add(expense);
            }
            var goals = new List<Goal>();
            for (int i = 1; i < 5; ++i)
            {

                var goal = new Goal()
                {
                    Id = i,
                    Title = $"Title-{i}",
                    Description = $"Description-{i}",
                    Amount = 1000 * i,
                    Currency = i % 2 == 0 ? "EUR" : "USD",
                    StartDate = DateTime.Now.AddDays(-i),
                    EndDate = DateTime.Now.AddDays(i),
                    User = i % 2 == 0 ? user1 : user2,
                };
                goals.Add(goal);
            }
            var budgets = new List<Budget>();
            for (int i = 1; i < 5; ++i)
            {

                var budget = new Budget()
                {
                    Id = i,
                    Title = $"Title-{i}",
                    Description = $"Description-{i}",
                    LimitAmount = 1000 * i,
                    Currency = i % 2 == 0 ? "EUR" : "USD",
                    Urgency = (Urgency)(i % 3),
                    StartDate = DateTime.Now.AddDays(-i),
                    EndDate = DateTime.Now.AddDays(i),
                    User = i % 2 == 0 ? user1 : user2,
                };
                budgets.Add(budget);
            }
            var categories = new List<Category>();
            for (int i = 1; i < 5; ++i)
            {
                var category = new Category()
                {
                    Id = i,
                    Title = $"Title-{i}",
                    Description = $"Description-{i}",
                    User = i % 2 == 0 ? user1 : user2,
                };
                categories.Add(category);
            }

            var incomesOfUser1 = incomes.Where(x =>  x.User == user1).ToList();
            var incomesOfUser2 = incomes.Where(x => x.User == user2).ToList();
            var categoriesOfUser1 = categories.Where(x => x.User == user1).ToList();
            var categoriesOfUser2 = categories.Where(x => x.User == user2).ToList();
            var expensesOfUser1 = expenses.Where(x => x.User == user1).ToList();
            var expensesOfUser2 = expenses.Where(x => x.User == user2).ToList();
            var budgetsOfUser1 = budgets.Where(x => x.User == user1).ToList();
            var budgetsOfUser2 = budgets.Where(x => x.User == user2).ToList();
            var goalsOfUser1 = goals.Where(x => x.User == user1).ToList();
            var goalsOfUser2 = goals.Where(x => x.User == user2).ToList();


            var incomeCategories = new List<IncomeCategory>();
            for (int i = 0; i < incomesOfUser1.Count; ++i)
            {
                var incomeCategory = new IncomeCategory()
                {
                    Income = incomesOfUser1[i],
                    IncomeId = incomesOfUser1[i].Id,
                    Category = categoriesOfUser1[i % categoriesOfUser1.Count],
                    CategoryId = categoriesOfUser1[i % categoriesOfUser1.Count].Id,
                };

                incomeCategories.Add(incomeCategory);
            }
            for (int i = 0; i < incomesOfUser2.Count; ++i)
            {
                var incomeCategory = new IncomeCategory()
                {
                    Income = incomesOfUser2[i],
                    IncomeId = incomesOfUser2[i].Id,
                    Category = categoriesOfUser2[i % categoriesOfUser2.Count],
                    CategoryId = categoriesOfUser2[i % categoriesOfUser2.Count].Id,
                };

                incomeCategories.Add(incomeCategory);
            }


            var expenseCategories = new List<ExpenseCategory>();
            for (int i = 0; i < expensesOfUser1.Count; ++i)
            {
                var expenseCategory = new ExpenseCategory()
                {
                    Expense = expensesOfUser1[i],
                    ExpenseId = expensesOfUser1[i].Id,
                    Category = categoriesOfUser1[i % categoriesOfUser1.Count],
                    CategoryId = categoriesOfUser1[i % categoriesOfUser1.Count].Id,
                };

                expenseCategories.Add(expenseCategory);
            }
            for (int i = 0; i < expensesOfUser2.Count; ++i)
            {
                var expenseCategory = new ExpenseCategory()
                {
                    Expense = expensesOfUser2[i],
                    ExpenseId = expensesOfUser2[i].Id,
                    Category = categoriesOfUser2[i % categoriesOfUser2.Count],
                    CategoryId = categoriesOfUser2[i % categoriesOfUser2.Count].Id,
                };

                expenseCategories.Add(expenseCategory);
            }


            var budgetCategories = new List<BudgetCategory>();
            for (int i = 0; i < budgetsOfUser1.Count; ++i)
            {
                var budgetCategory = new BudgetCategory()
                {
                    Budget = budgetsOfUser1[i],
                    BudgetId = budgetsOfUser1[i].Id,
                    Category = categoriesOfUser1[i % categoriesOfUser1.Count],
                    CategoryId = categoriesOfUser1[i % categoriesOfUser1.Count].Id,
                };

                budgetCategories.Add(budgetCategory);
            }
            for (int i = 0; i < budgetsOfUser2.Count; ++i)
            {
                var budgetCategory = new BudgetCategory()
                {
                    Budget = budgetsOfUser2[i],
                    BudgetId = budgetsOfUser2[i].Id,
                    Category = categoriesOfUser2[i % categoriesOfUser2.Count],
                    CategoryId = categoriesOfUser2[i % categoriesOfUser2.Count].Id,
                };

                budgetCategories.Add(budgetCategory);
            }


            var goalCategories = new List<GoalCategory>();
            for (int i = 0; i < goalsOfUser1.Count; ++i)
            {
                var goalCategory = new GoalCategory()
                {
                    Goal = goalsOfUser1[i],
                    GoalId = goalsOfUser1[i].Id,
                    Category = categoriesOfUser1[i % categoriesOfUser1.Count],
                    CategoryId = categoriesOfUser1[i % categoriesOfUser1.Count].Id,
                };

                goalCategories.Add(goalCategory);
            }
            for (int i = 0; i < goalsOfUser2.Count; ++i)
            {
                var goalCategory = new GoalCategory()
                {
                    Goal = goalsOfUser2[i],
                    GoalId = goalsOfUser2[i].Id,
                    Category = categoriesOfUser2[i % categoriesOfUser2.Count],
                    CategoryId = categoriesOfUser2[i % categoriesOfUser2.Count].Id,
                };
            
                goalCategories.Add(goalCategory);
            }

            var authorizeUser2ToUser1 = new AuthorizedUserJoin()
            {
                AuthorizedUser = user2,
                AuthorizedUserId = user2.Id,
                Owner = user1,
                OwnerId = user1.Id,
            };
            var authorizeUser3ToUser1WithEdits = new AuthorizedUserJoin()
            {
                AuthorizedUser = user3,
                AuthorizedUserId = user3.Id,
                Owner = user1,
                OwnerId = user1.Id,
                CanEdit = true,
            };

            dataContext.AddRange(incomes);
            dataContext.AddRange(expenses);
            dataContext.AddRange(budgets);
            dataContext.AddRange(goals);
            dataContext.AddRange(categories);
            dataContext.AddRange(incomeCategories);
            dataContext.AddRange(expenseCategories);
            dataContext.AddRange(budgetCategories);
            dataContext.AddRange(goalCategories);
            dataContext.Add(authorizeUser2ToUser1);
            dataContext.Add(authorizeUser3ToUser1WithEdits);
            dataContext.SaveChanges();
        }
    }
}
