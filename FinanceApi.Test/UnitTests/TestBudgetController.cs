using FinanceApi.Controllers;
using FinanceApi.Data;
using FinanceApi.Repositories;
using FinanceApi.Services;
using FinanceApi.Test.TestDatabase;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinanceApi.Test.Tests
{
    public class TestBudgetController
    {
        private DataContext dataContext;
        public TestBudgetController()
        {
            var testDatabaseFixture = new TestDatabaseFixture();
            dataContext = testDatabaseFixture.dataContext;
        }
    }
}
