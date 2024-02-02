using FinanceApi.Data;
using FinanceApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceApi.Test.TestDatabase
{
    public class TestDatabaseFixture
    {
        private DataContext dataContext { get; }
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
            dataContext.Incomes.Add(new Income()
            {
                Id = 1,
                Title = "Title",
                Description = "Description",
                Currency = "Eur",
                Amount = 100,
                Date = DateTime.Now,
                DocumentUrl = "URL",
            });
            dataContext.SaveChanges();
        }
    }
}
