using FinanceApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExpenseController : Controller
    {
        private readonly IExpenseService expenseService;

        public ExpenseController(IExpenseService expenseService)
        {
            this.expenseService = expenseService;
        }
    }
}
