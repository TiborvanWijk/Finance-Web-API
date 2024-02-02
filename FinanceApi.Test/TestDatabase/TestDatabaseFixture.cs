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

        }
    }
}
