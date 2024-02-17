﻿using FinanceApi.Data;
using FinanceApi.Test.TestDatabase;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace FinanceApi.Test.Utils
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {

                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<DataContext>)
                );

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<DataContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDbContext");
                });

                using (var scope = services.BuildServiceProvider().CreateScope())
                {
                    var testContext = scope.ServiceProvider.GetRequiredService<DataContext>();

                    testContext.Database.EnsureCreated();

                    if(testContext.Users.Count() < 2)
                    {
                        TestDatabaseFixture.SeedDatabase(testContext);
                    }


                    testContext.SaveChanges();
                }
            });
        }

    }
}
