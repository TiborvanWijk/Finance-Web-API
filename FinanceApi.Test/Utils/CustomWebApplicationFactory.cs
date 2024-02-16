using FinanceApi.Data;
using FinanceApi.Test.TestDatabase;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace FinanceApi.Test.Utils
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {

    }
}
