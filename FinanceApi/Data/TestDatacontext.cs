using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Data
{
    public class TestDatacontext : DataContext
    {
        public TestDatacontext(DbContextOptions<DataContext> options) : base(options)
        {
        }
    }
}
