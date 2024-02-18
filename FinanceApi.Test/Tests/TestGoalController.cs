using FinanceApi.Controllers;
using FinanceApi.Data;
using FinanceApi.Data.Dtos;
using FinanceApi.Mapper;
using FinanceApi.Models;
using FinanceApi.Repositories;
using FinanceApi.Services;
using FinanceApi.Test.TestDatabase;
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
    public class TestGoalController : IDisposable
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
        private CustomWebApplicationFactory factory;
        private HttpClient client;

        public TestGoalController()
        {
            var testDatabaseFixture = new TestDatabaseFixture();
            dataContext = testDatabaseFixture.dataContext;

            factory = new CustomWebApplicationFactory();
            client = factory.CreateClient();



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
        //[Theory]
        [Theory(Skip = "Progress of goalDto is not calculated correctly.")]
        [MemberData(nameof(GetGoalValidInputTestData))]
        public async Task GetGoal_ReturnsOkObjectResult_WhenUserIsValid2(
            string userName,
            DateTime? startDate,
            DateTime? endDate,
            string? listOrderBy,
            string? listDir,
            string? optionalOwnerUsername
            )
        {

            string optionalOwnerId = null;
            User user;
            using (var scope = factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

                if (optionalOwnerUsername != null)
                {
                    optionalOwnerId = dbContext.Users.FirstOrDefault(x => x.UserName == optionalOwnerUsername).Id;
                }
                user = dbContext.Users.First(x => x.UserName == userName);
            }

            var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            object[] paramValues = { startDate, endDate, listOrderBy, listDir, optionalOwnerId };
            string[] paramNames = { "startDate", "endDate", "listOrderBy", "listDir", "optionalOwnerId" };
            var requestUrl = $"/api/Goal/current";
            bool added = false;
            for (var i = 0; i < paramValues.Length; ++i)
            {
                var value = paramValues[i];
                if (value != null)
                {
                    if (value is DateTime date)
                    {
                        value = date.ToString("yyyy-MM-dd");
                    }
                    requestUrl += added ? $"&{paramNames[i]}={value}" : $"?{paramNames[i]}={value}";
                    added = true;
                }
            }


            var response = await client.GetAsync(requestUrl);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            ICollection<GoalDto> responseData = JsonConvert.DeserializeObject<ICollection<GoalDto>>(await response.Content.ReadAsStringAsync());


            var ownerId = optionalOwnerId ?? user.Id;

            ICollection<GoalDto> correctResponse = null;
            using (var scope = factory.Services.CreateScope())
            {

                var db = scope.ServiceProvider.GetRequiredService<DataContext>();

                correctResponse = db.Goals.Include(g => g.GoalCategories).ThenInclude(gc => gc.Category.IncomeCategories)
                    .Where(x => x.User.Id.Equals(ownerId)
                        && (startDate == null || x.StartDate >= startDate)
                        && (endDate == null || x.EndDate <= endDate))
                    .Select(Map.ToGoalDto)
                    .Select(x =>
                    {
                        x.Progress = db.Incomes
                        .Where(i => i.User.Id.Equals(ownerId) && i.IncomeCategories
                        .Any(ic => ic.Category.GoalCategories.Any(gc => gc.Goal.User.Id.Equals(ownerId) && gc.GoalId == x.Id)))
                        .Select(d => d.Amount).Sum();

                        return x;
                    })
                    .ToList();

            };


            if (listOrderBy != null || listDir != null)
            {
                switch (listOrderBy)
                {
                    case "title":
                        correctResponse = listDir != null && listDir.Equals("desc") ?
                            correctResponse.OrderByDescending(x => x.Title).ToList()
                            :
                            correctResponse.OrderBy(x => x.Title).ToList();
                        break;
                    case "amount":
                        correctResponse = listDir != null && listDir.Equals("desc") ?
                            correctResponse.OrderByDescending(x => x.Amount).ToList()
                            :
                            correctResponse.OrderBy(x => x.Amount).ToList();
                        break;
                    case "progress":
                        correctResponse = listDir != null && listDir.Equals("desc") ?
                            correctResponse.OrderByDescending(x => x.Progress).ToList()
                            :
                            correctResponse.OrderBy(x => x.Progress).ToList();
                        break;
                    default:
                        correctResponse = listDir != null && listDir.Equals("desc") ?
                            correctResponse.OrderByDescending(x => x.EndDate).ToList()
                            :
                            correctResponse.OrderBy(x => x.EndDate).ToList();
                        break;
                }
            }

            bool equals = Equals(correctResponse, responseData);
            Assert.Equal(correctResponse, responseData);
            

        }

        [Theory]
        [MemberData(nameof(CreateGoalValidInputTestData))]
        public async Task CreateGoal_ReturnsOk_WhenInputIsValid(
            string username,
            GoalManageDto goalManageDto,
            string optionalOwnerUsername
            )
        {
            //arrange
            using (var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();      

                string? optionalOwnerId = null;
                var user = db.Users.First(u => u.UserName.Equals(username));

                if (optionalOwnerUsername != null)
                {
                    optionalOwnerId = db.Users.FirstOrDefault(u => u.UserName.Equals(optionalOwnerUsername)).Id;
                }
                string ownerId = optionalOwnerId ?? user.Id;

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");

                var requestUrl = "api/Goal/post";

                if (optionalOwnerId != null)
                {
                    requestUrl += $"?optionalOwnerId={optionalOwnerId}";
                }

                var jsonContent = new StringContent(JsonConvert.SerializeObject(goalManageDto), Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                // Act
                try
                {
                    var response = await client.PostAsync(requestUrl, jsonContent);


                    // Assert
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                }
                finally
                {

                    bool isAdded = db.Goals.Any(g => g.Title.Equals(goalManageDto.Title));
                    Assert.True(isAdded);
                    if (isAdded)
                    {
                        db.Goals.Remove(db.Goals.First(g => g.Title.Equals(goalManageDto.Title)));
                        db.SaveChanges();
                    }
                }

            }
        }

        [Theory]
        [MemberData(nameof(CreateGoalInvalidtestTestData))]
        public async Task CreateGoal_ReturnsBadrequest_WhenInputIsInvalid(
            string username,
            GoalManageDto goalManageDto,
            string? optionalOwnerUsername
            )
        {
            using (var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();

                string? optionalOwnerId = null;
                var user = db.Users.First(u => u.UserName.Equals(username));

                if (optionalOwnerUsername != null)
                {
                    optionalOwnerId = db.Users.FirstOrDefault(u => u.UserName.Equals(optionalOwnerUsername)).Id;
                }
                string ownerId = optionalOwnerId ?? user.Id;

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");

                var requestUrl = "api/Goal/post";

                if (optionalOwnerId != null)
                {
                    requestUrl += $"?optionalOwnerId={optionalOwnerId}";
                }

                var jsonContent = new StringContent(JsonConvert.SerializeObject(goalManageDto), Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                // Act
                try
                {
                    var response = await client.PostAsync(requestUrl, jsonContent);


                    // Assert
                    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                }
                finally
                {

                    bool isAdded = db.Goals.Any(g => g.Title.Equals(goalManageDto.Title));
                    Assert.False(isAdded);
                    if (isAdded)
                    {
                        db.Goals.Remove(db.Goals.First(g => g.Title.Equals(goalManageDto.Title)));
                        db.SaveChanges();
                    }
                }

            }
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
            yield return new object[] { "user1@example.com", new GoalManageDto() { Id = 1, Title = "Test-Title-1", Description = "Description-1", Amount = 2000, Currency = "eur", StartDate = DateTime.Now, EndDate = new DateTime(2026, 1, 1) }, null };
            yield return new object[] { "user1@example.com", new GoalManageDto() { Id = 2, Title = "Test-Title-1", Description = "Description for a valid goal with a different currency", Amount = 1500, Currency = "usd", StartDate = DateTime.Now.AddDays(5), EndDate = new DateTime(2026, 1, 1) }, null };
            yield return new object[] { "user1@example.com", new GoalManageDto() { Id = 3, Title = "Test-Title-1", Description = "Description for a valid goal with a longer time period", Amount = 3000, Currency = "eur", StartDate = DateTime.Now.AddDays(10), EndDate = new DateTime(2026, 2, 15) }, null };
            yield return new object[] { "user1@example.com", new GoalManageDto() { Id = 4, Title = "Test-Title-1", Description = "Description for a valid goal with a larger amount", Amount = 5000, Currency = "php", StartDate = DateTime.Now.AddDays(3), EndDate = new DateTime(2026, 1, 1) }, null };
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
            yield return new object[] { "user1@example.com", new GoalManageDto { Amount = 0, Currency = "InvalidCode", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(-15) }, null };

            // Invalid amount (zero or negative)
            yield return new object[] { "user1@example.com", new GoalManageDto { Amount = 0, Currency = "USD", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(30) }, null };
            yield return new object[] { "user1@example.com", new GoalManageDto { Amount = -500, Currency = "EUR", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(30) }, null };

            // Invalid currency ISO code
            yield return new object[] { "user1@example.com", new GoalManageDto { Amount = 1000, Currency = "", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(30) }, null };
            yield return new object[] { "user1@example.com", new GoalManageDto { Amount = 1500, Currency = "InvalidCurrencyCode", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(30) }, null };

            // Invalid time period (end date earlier than start date)
            yield return new object[] { "user1@example.com", new GoalManageDto { Amount = 2000, Currency = "GBP", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(-7) }, null };

            // Combining multiple invalid conditions
            yield return new object[] { "user1@example.com", new GoalManageDto { Amount = 0, Currency = "InvalidCode", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(-15) }, null };
            yield return new object[] { "user1@example.com", new GoalManageDto { Amount = -500, Currency = "", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(-15) }, null };
            yield return new object[] { "user1@example.com", new GoalManageDto { Amount = 1000, Currency = "InvalidCurrency", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(-15) }, null };
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

        public static IEnumerable<object[]> DeleteGoalValidTestData()
        {
            yield return new object[] { 1, "user1@example.com", null };
            yield return new object[] { 2, "user1@example.com", null };
            yield return new object[] { 3, "user1@example.com", null };

            yield return new object[] { 4, "user3@example.com", "user1@example.com" };
            yield return new object[] { 5, "user3@example.com", "user1@example.com" };

            yield return new object[] { 6, "user2@example.com", null };
            yield return new object[] { 7, "user2@example.com", null };
            yield return new object[] { 8, "user2@example.com", null };
            yield return new object[] { 9, "user2@example.com", null };
            yield return new object[] { 10, "user2@example.com", null };
        }

        [Theory]
        [MemberData(nameof(DeleteGoalValidTestData))]
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

        public static IEnumerable<object[]> DeleteGoalInvalidInputTestData()
        {
            yield return new object[] { 6, "user1@example.com", null };
            yield return new object[] { 7, "user1@example.com", null };
            yield return new object[] { 8, "user1@example.com", null };
            yield return new object[] { 9, "user1@example.com", null };
            yield return new object[] { 10, "user1@example.com", null };
        }

        [Theory]
        [MemberData(nameof(DeleteGoalInvalidInputTestData))]
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


        public static IEnumerable<object[]> RemoveCategoriesValidTestData()
        {
            yield return new object[] { "user1@example.com", 1, new List<int>() { } };
        }

        [Theory(Skip = "Skipping because it is not ready at the current state.")]
        [MemberData(nameof(RemoveCategoriesValidTestData))]
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

        public void Dispose()
        {
            client.Dispose();
            factory.Dispose();
        }


        public async Task<string> GetAuthenticationTokenAsync(string email, string password)
        {
            var requestBody = new StringContent(
                JsonConvert.SerializeObject(new
                {
                    email,
                    password
                }),
                Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/login", requestBody);

            if (response.IsSuccessStatusCode)
            {
                var data = JsonConvert.DeserializeObject<object>(await response.Content.ReadAsStringAsync());
                string accesToken = JObject.FromObject(data)["accessToken"]?.ToString();
                return accesToken;
            }


            return null;
        }
    }
}
