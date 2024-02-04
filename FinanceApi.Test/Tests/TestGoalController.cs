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

        }                                                                               		


        [Theory]
        [MemberData(nameof(GetGoalValidInputTestData))]
        public void GetGoal_ReturnsOkObjectResult_WhenUserIsValid(
            string userName,
            DateTime? startDate,
            DateTime? endDate,
            string? listOrderBy,
            string? listDir,
            string? optionalOwnerId
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
                    
                }
            };

            // Act
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






    }
}
