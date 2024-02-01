using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers.ApiResponseHelpers
{
    public static class ApiResponseHelper
    {
        public static IActionResult HandleErrorResponse(int errorCode, string errorMessage)
        {
            switch (errorCode)
            {
                case 400:
                    return new BadRequestObjectResult(new { Error = errorMessage });
                case 401:
                    return new UnauthorizedObjectResult(new { Error = errorMessage });
                case 403:
                    return new ForbidResult();
                case 404:
                    return new NotFoundObjectResult(new { Error = errorMessage });
                case 500:
                    return new ObjectResult(new { Error = errorMessage })
                    {
                        StatusCode = 500,
                    };
                default:
                    return new ObjectResult(new { Error = "An unexpected error occurred." })
                    {
                        StatusCode = 500,
                    };
            }

        }
    }

}
