using FinanceApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GoalController : Controller
    {
        private readonly IGoalService goalService;

        public GoalController(IGoalService goalService)
        {
            this.goalService = goalService;
        }
    }
}
