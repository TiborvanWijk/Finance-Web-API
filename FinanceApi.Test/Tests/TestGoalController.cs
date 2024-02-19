using FinanceApi.Controllers;
using FinanceApi.Data;
using FinanceApi.Data.Dtos;
using FinanceApi.Mapper;
using FinanceApi.Models;
using FinanceApi.Repositories;
using FinanceApi.Services;
using FinanceApi.Test.TestDatabase;
using FinanceApi.Test.TestDataHolder;
using FinanceApi.Test.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

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

       


        

        




        [Theory]
        [MemberData(nameof(TestData.GetGoalValidInputTestData), MemberType = typeof(TestData))]
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


        [Theory]
        [MemberData(nameof(TestData.CreateGoalValidInputTestData), MemberType = typeof(TestData))]
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




        [Theory]
        [MemberData(nameof(TestData.CreateGoalInvalidtestTestData), MemberType = typeof(TestData))]
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



        [Theory]
        [MemberData(nameof(TestData.DeleteGoalValidTestData), MemberType = typeof(TestData))]
        public void DeleteGoal_ReturnsOkObjectResult_WhenInputIsValid(
            int goalId,
            string username,
            string? optionalOwnerUsername
            )
        {

            // Arrange

            var user = dataContext.Users.First(x => x.UserName.Normalize().Equals(username.Normalize()));



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

            var result = goalController.DeleteGoal(goalId, optionalOwnerId);
            // Assert


            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
        }



        [Theory]
        [MemberData(nameof(TestData.DeleteGoalInvalidInputTestData), MemberType = typeof(TestData))]
        public void DeleteIncome_ReturnsNotFoundObjectResult_WhenGoalDoesNotExist(
            int goalId,
            string username,
            string? optionalOwnerUsername
            )
        {
            // Arrange
            var user = dataContext.Users.First(x => x.UserName.Normalize().Equals(username.Normalize()));

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

            var result = goalController.DeleteGoal(goalId, optionalOwnerId);

            // Assert

            Assert.IsType<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
        }



        [Theory(Skip = "Skipping because it is not ready at the current state.")]
        [MemberData(nameof(TestData.RemoveCategoriesValidTestData), MemberType = typeof(TestData))]
        public void RemoveCategories_RetunsOkObjectResult_WhenGoalExistsAndCategoryIdsExist(
            string username,
            int goalId,
            ICollection<int> catgoryIds,
            string? optionalOwnerUsername
            )
        {

            // Arrange

            var user = dataContext.Users.First(x => x.UserName.Normalize().Equals(username.Normalize()));



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

            var result = goalController.RemoveCategories(goalId, catgoryIds, optionalOwnerId);

            // Assert


            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
        }

        
    }
}
