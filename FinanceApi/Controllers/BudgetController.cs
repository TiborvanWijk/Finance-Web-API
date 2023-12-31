using FinanceApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BudgetController : Controller
    {
        private readonly IBudgetService budgetService;

        public BudgetController(IBudgetService budgetService)
        {
            this.budgetService = budgetService;
        }
    }
}
