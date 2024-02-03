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
            
            

            dataContext.AddRange(incomes);
            dataContext.AddRange(expenses);
            dataContext.AddRange(budgets);
            dataContext.AddRange(goals);
            dataContext.AddRange(categories);
            dataContext.Add(incomeCategory);
            dataContext.SaveChanges();


        }
    }
}
