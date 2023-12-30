using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        [Authorize]
        [HttpGet]
        [ProducesResponseType(200)]
        public IActionResult Testing()
        {
            return Ok("You made it");
        }

    }
}
