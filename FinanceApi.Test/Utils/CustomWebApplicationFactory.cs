using FinanceApi.Data;
using FinanceApi.Test.TestDatabase;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace FinanceApi.Test.Utils
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                // remove the existing context configuration
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<TestDatacontext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<TestDatacontext>(options =>
                    options.UseInMemoryDatabase("TestDB"));
            });
        }
    }
}
