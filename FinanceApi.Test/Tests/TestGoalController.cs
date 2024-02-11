using FinanceApi.Controllers;
using FinanceApi.Data;
using FinanceApi.Data.Dtos;
using FinanceApi.Repositories;
using FinanceApi.Services;
using FinanceApi.Test.TestDatabase;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinanceApi.Test.Tests
{
    public class TestGoalController
    {
        private DataContext dataContext;
        private readonly GoalRepository goalRepository;
        private readonly CategoryRepository categoryRepository;
        private readonly IncomeRepository incomeRepository;
        private readonly AuthorizeRepository authorizeRepository;
        private readonly AuthorizationInviteRepository authInviteRepo;
        private readonly UserRepository userRepo;
        private readonly ExpenseRepository expenseRepo;
        private readonly GoalService goalService;
        private readonly AuthorizeService authService;
        private readonly UserService userService;
        private readonly GoalController goalController;

        public TestGoalController()
        {
            var testDatabaseFixture = new TestDatabaseFixture();
            dataContext = testDatabaseFixture.dataContext;
            
            goalRepository = new GoalRepository(dataContext);
            categoryRepository = new CategoryRepository(dataContext);
            incomeRepository = new IncomeRepository(dataContext);
            authorizeRepository = new AuthorizeRepository(dataContext);
            authInviteRepo = new AuthorizationInviteRepository(dataContext);
            userRepo = new UserRepository(dataContext);
            expenseRepo = new ExpenseRepository(dataContext);

            goalService = new GoalService(goalRepository, categoryRepository, incomeRepository);
            authService = new AuthorizeService(authorizeRepository, authInviteRepo, userRepo);
            userService = new UserService(userRepo, expenseRepo, incomeRepository);

            goalController = new GoalController(goalService, userService, authService);
        }

        public static IEnumerable<object[]> GetGoalValidInputTestData()
        {
            yield return new object[] { "user1@example.com", new DateTime(2020, 3, 15), new DateTime(2022, 7, 10), "progress", "desc", null };
            yield return new object[] { "user1@example.com", new DateTime(2021, 6, 5), new DateTime(2023, 4, 25), "amount", "desc", null };
            yield return new object[] { "user1@example.com", new DateTime(2019, 2, 10), new DateTime(2024, 11, 30), "title", "desc", null };
            yield return new object[] { "user1@example.com", new DateTime(2020, 9, 20), new DateTime(2023, 1, 5), null, null, null };

            yield return new object[] { "user2@example.com", new DateTime(2020, 8, 12), new DateTime(2023, 6, 18), "progress", "desc", null };
            yield return new object[] { "user2@example.com", new DateTime(2022, 4, 30), new DateTime(2023, 2, 8), null, null, null };
            yield return new object[] { "user2@example.com", new DateTime(2019, 11, 8), new DateTime(2024, 9, 22), "amount", "desc", null };
            yield return new object[] { "user2@example.com", new DateTime(2021, 7, 3), new DateTime(2023, 4, 2), "title", "desc", null };

            yield return new object[] { "user3@example.com", new DateTime(2020, 5, 22), new DateTime(2023, 10, 15), null, null, null };
            yield return new object[] { "user3@example.com", new DateTime(2022, 1, 17), new DateTime(2024, 8, 7), "progress", "desc", null };
            yield return new object[] { "user3@example.com", new DateTime(2019, 10, 3), new DateTime(2024, 3, 12), "amount", "desc", null };
            yield return new object[] { "user3@example.com", new DateTime(2020, 6, 28), new DateTime(2023, 12, 5), "title", "desc", null };

            yield return new object[] { "user4@example.com", new DateTime(2021, 12, 7), new DateTime(2023, 5, 28), "progress", "desc", null };
            yield return new object[] { "user4@example.com", new DateTime(2019, 2, 25), new DateTime(2024, 1, 20), "amount", "desc", null };
            yield return new object[] { "user4@example.com", new DateTime(2020, 7, 14), new DateTime(2023, 9, 10), "title", "desc", null };
            yield return new object[] { "user4@example.com", new DateTime(2022, 4, 18), new DateTime(2024, 3, 30), null, null, null };

            yield return new object[] { "user1@example.com", null, null, "progress", "desc", null };
            yield return new object[] { "user1@example.com", null, null, "amount", "desc", null };
            yield return new object[] { "user1@example.com", null, null, "title", "desc", null };
            yield return new object[] { "user1@example.com", null, null, null, null, null };

            yield return new object[] { "user2@example.com", null, null, "progress", "desc", null };
            yield return new object[] { "user2@example.com", null, null, null, null, null };
            yield return new object[] { "user2@example.com", null, null, "amount", "desc", null };
            yield return new object[] { "user2@example.com", null, null, "title", "desc", null };

            yield return new object[] { "user3@example.com", null, null, null, null, null };
            yield return new object[] { "user3@example.com", null, null, "progress", "desc", null };
            yield return new object[] { "user3@example.com", null, null, "amount", "desc", null };
            yield return new object[] { "user3@example.com", null, null, "title", "desc", null };

            yield return new object[] { "user4@example.com", null, null, "progress", "desc", null };
            yield return new object[] { "user4@example.com", null, null, "amount", "desc", null };
            yield return new object[] { "user4@example.com", null, null, "title", "desc", null };
            yield return new object[] { "user4@example.com", null, null, null, null, null };

            yield return new object[] { "user2@example.com", null, null, null, null, "user1@example.com" };
            yield return new object[] { "user3@example.com", null, null, null, null, "user1@example.com" };

        }                                                                               		


        [Theory]
        [MemberData(nameof(GetGoalValidInputTestData))]
        public void GetGoal_ReturnsOkObjectResult_WhenUserIsValid(
            string userName,
            DateTime? startDate,
            DateTime? endDate,
            string? listOrderBy,
            string? listDir,
            string? optionalOwnerUsername
            )
        {
            // Arrange
            var user = dataContext.Users.First(u => u.UserName.Normalize().Equals(userName.Normalize()));

            goalController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id)
                    })),
                    Request = { Method = "GET" }
                }
            };

            // Act
            var optionalOwnerId = optionalOwnerUsername == null ? null : dataContext.Users.First(x => x.UserName.Normalize().Equals(optionalOwnerUsername.Normalize())).Id;
            var result = goalController.GetGoals(startDate, endDate, listOrderBy, listDir, optionalOwnerId);

            // Assert

            Assert.IsType<OkObjectResult>(result);
            var okResult = (OkObjectResult)result;

            Assert.NotNull(okResult);
            Assert.IsType<List<GoalDto>>(okResult.Value);

            var okGoalDtoList = (List<GoalDto>)okResult.Value;

            List<GoalDto> orderedListValid = new List<GoalDto>(okGoalDtoList);
            if (listOrderBy != null || listDir != null)
            {
                //listOrderBy = listOrderBy ?? "DEFAULT (NO ORDERING)";
                switch (listOrderBy.ToLower())
                {
                    case "title":
                         orderedListValid = listDir != null && listDir.Equals("desc")
                            ? orderedListValid.OrderByDescending(dto => dto.Title).ToList()
                            : orderedListValid.OrderBy(dto => dto.Title).ToList();
                        break;
                    case "amount":
                        orderedListValid = listDir != null && listDir.Equals("desc")
                            ? orderedListValid.OrderByDescending(dto => dto.Amount).ToList()
                            : orderedListValid.OrderBy(dto => dto.Amount).ToList();
                        break;
                    case "progress":
                        orderedListValid = listDir != null && listDir.Equals("desc")
                            ? orderedListValid.OrderByDescending(dto => dto.Progress).ToList()
                            : orderedListValid.OrderBy(dto => dto.Progress).ToList();
                        break;
                    default:
                        orderedListValid = listDir != null && listDir.Equals("desc") ?
                            orderedListValid.OrderByDescending(dto => dto.EndDate).ToList()
                            : orderedListValid.OrderBy(idto => idto.EndDate).ToList();
                        break;
                }
            }

            Assert.Equal(okGoalDtoList, orderedListValid);
            Assert.False(okGoalDtoList.Any(x => x.StartDate < startDate || x.EndDate > endDate));
        }


        public static IEnumerable<object[]> CreateGoalValidInputTestData()
        {
            yield return new object[] { "user1@example.com", new GoalManageDto() { Id = 1, Title = "GoalTitle-1", Description = "Description-1", Amount = 2000, Currency = "eur", StartDate = DateTime.Now, EndDate = new DateTime(2026, 1, 1) }, null };
            yield return new object[] { "user1@example.com", new GoalManageDto() { Id = 2, Title = "Valid Goal - Different Currency", Description = "Description for a valid goal with a different currency", Amount = 1500, Currency = "usd", StartDate = DateTime.Now.AddDays(5), EndDate = new DateTime(2026, 1, 1) }, null };
            yield return new object[] { "user1@example.com", new GoalManageDto() { Id = 3, Title = "Valid Goal - Longer Time Period", Description = "Description for a valid goal with a longer time period", Amount = 3000, Currency = "eur", StartDate = DateTime.Now.AddDays(10), EndDate = new DateTime(2026, 2, 15) }, null };
            yield return new object[] { "user1@example.com", new GoalManageDto() { Id = 4, Title = "Valid Goal - Larger Amount", Description = "Description for a valid goal with a larger amount", Amount = 5000, Currency = "php", StartDate = DateTime.Now.AddDays(3), EndDate = new DateTime(2026, 1, 1) }, null };
        }

        [Theory]
        [MemberData(nameof(CreateGoalValidInputTestData))]
        public void CreateGoal_ReturnsOkObjectResult_WhenInputIsValid(
            string username,
            GoalManageDto goalManageDto,
            string optionalOwnerUsername
            )
        {
            // Arrange
            var user = dataContext.Users
                .First(x => x.UserName.Normalize().Equals(username.Normalize()));

            goalController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id)
                    }))
                }
            };

            var optionalOwnerId = optionalOwnerUsername == null ? null : dataContext.Users.First(x => x.UserName.Normalize().Equals(optionalOwnerUsername.Normalize())).Id;

            // Act

            var result = goalController.CreateGoal(goalManageDto, optionalOwnerUsername);

            // Assert

            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
        }


        public static IEnumerable<object[]> CreateGoalInvalidtestTestData()
        {
            // Invalid amount (less than or equal to 0)
            yield return new object[] { "user1@example.com", new GoalManageDto { Amount = 0, Currency = "USD", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(30) }, null };

            // Invalid currency ISO code
            yield return new object[] { "user2@example.com", new GoalManageDto { Amount = 1000, Currency = "InvalidCurrency", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(30) }, null };

            // Invalid time period (e.g., end date earlier than start date)
            yield return new object[] { "user1@example.com", new GoalManageDto { Amount = 2000, Currency = "EUR", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(-7) }, null };


            // Combining multiple invalid conditions
            yield return new object[] {"user1@example.com", new GoalManageDto { Amount = 0, Currency = "InvalidCode", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(-15) }, null };

            // Invalid amount (zero or negative)
            yield return new object[] { "user1@example.com", new GoalManageDto { Amount = 0, Currency = "USD", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(30) }, null };
            yield return new object[] { "user1@example.com", new GoalManageDto { Amount = -500, Currency = "EUR", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(30) }, null };

            // Invalid currency ISO code
            yield return new object[] { "user1@example.com", new GoalManageDto { Amount = 1000, Currency = "", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(30) }, null };
            yield return new object[] { "user1@example.com", new GoalManageDto { Amount = 1500, Currency = "InvalidCurrencyCode", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(30) }, null };

            // Invalid time period (end date earlier than start date)
            yield return new object[] { "user1@example.com", new GoalManageDto { Amount = 2000, Currency = "GBP", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(-7) }, null };

            // Combining multiple invalid conditions
            yield return new object[] {"user1@example.com", new GoalManageDto { Amount = 0, Currency = "InvalidCode", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(-15) }, null };
            yield return new object[] {"user1@example.com", new GoalManageDto { Amount = -500, Currency = "", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(-15) }, null };
            yield return new object[] {"user1@example.com", new GoalManageDto { Amount = 1000, Currency = "InvalidCurrency", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(-15) }, null };
            yield return new object[] { "user1@example.com", new GoalManageDto { Amount = 2000, Currency = "USD", StartDate = DateTime.Now.AddDays(15), EndDate = DateTime.Now }, null };
            yield return new object[] { "user1@example.com", new GoalManageDto { Amount = 3000, Currency = "GBP", StartDate = DateTime.Now.AddDays(15), EndDate = DateTime.Now.AddDays(10) }, null };
        }


        [Theory]
        [MemberData(nameof(CreateGoalInvalidtestTestData))]
        public void CreateGoal_ReturnsBadRequestObjectResult_WhenGoalInputIsInvalid(
            string username, 
            GoalManageDto goalManageDto,
            string optionalOwnerUsername
            )
        {

            // Arrange
            var user = dataContext.Users
                .First(x => x.UserName.Normalize().Equals(username.Normalize()));

            goalController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id)
                    }))
                }
            };

            var optionalOwnerId = optionalOwnerUsername == null ? null : dataContext.Users.First(x => x.UserName.Normalize().Equals(optionalOwnerUsername.Normalize())).Id;

            // Act

            var result = goalController.CreateGoal(goalManageDto, optionalOwnerUsername);

            // Assert

            Assert.IsType<BadRequestObjectResult>(result);
            var badResult = result as BadRequestObjectResult;
            Assert.NotNull(badResult);


        }



    }
}
