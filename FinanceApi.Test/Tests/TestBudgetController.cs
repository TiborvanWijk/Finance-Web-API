using FinanceApi.Data;
using FinanceApi.Test.TestDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
