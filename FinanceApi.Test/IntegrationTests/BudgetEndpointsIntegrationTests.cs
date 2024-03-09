using FinanceApi.Data.Dtos;
using FinanceApi.Data;
using FinanceApi.Mapper;
using FinanceApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FinanceApi.Test.Utils;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using FinanceApi.Test.TestDataHolder;
using System.Data;

namespace FinanceApi.Test.IntegrationTests
{
    public class BudgetEndpointsIntegrationTests : IDisposable
    {
        private CustomWebApplicationFactory factory;
        private HttpClient client;
        public BudgetEndpointsIntegrationTests()
        {
            factory = new CustomWebApplicationFactory();
            client = factory.CreateClient();
        }



        [Theory]
        [MemberData(nameof(TestData.GetBudgetValidInputTestData), MemberType = typeof(TestData))]
        public async Task GetBudget_ReturnsOkObjectResult_WhenUserIsValid2(
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
            var requestUrl = $"/api/Budget/current";
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

            List<BudgetDto> responseData = JsonConvert.DeserializeObject<List<BudgetDto>>(await response.Content.ReadAsStringAsync());


            var ownerId = optionalOwnerId ?? user.Id;

            List<BudgetDto> correctResponse = null;
            using (var scope = factory.Services.CreateScope())
            {

                var db = scope.ServiceProvider.GetRequiredService<DataContext>();

                correctResponse = db.Budgets.Include(g => g.BudgetCategories).ThenInclude(gc => gc.Category.IncomeCategories)
                    .Where(x => x.User.Id.Equals(ownerId)
                        && (startDate == null || x.StartDate >= startDate)
                        && (endDate == null || x.EndDate <= endDate))
                    .Select(Map.ToBudgetDto)
                    .Select(x =>
                    {
                        x.Spending = db.Expenses
                        .Where(i => i.Date >= x.StartDate && i.Date <= DateTime.Now && i.User.Id.Equals(ownerId) && i.ExpenseCategories
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
                    case "spending":
                        correctResponse = listDir != null && listDir.Equals("desc") ?
                            correctResponse.OrderByDescending(x => x.Spending).ToList()
                            :
                            correctResponse.OrderBy(x => x.Spending).ToList();
                        break;
                    case "limitAmount":
                        correctResponse = listDir != null && listDir.Equals("desc") ?
                            correctResponse.OrderByDescending(x => x.LimitAmount).ToList()
                            :
                            correctResponse.OrderBy(x => x.LimitAmount).ToList();
                        break;
                    default:
                        correctResponse = listDir != null && listDir.Equals("desc") ?
                            correctResponse.OrderByDescending(x => x.EndDate).ToList()
                            :
                            correctResponse.OrderBy(x => x.EndDate).ToList();
                        break;
                }
            }

            Assert.Equivalent(correctResponse, responseData);

        }

        [Theory]
        [MemberData(nameof(TestData.CreateBudgetValidInputTestData), MemberType = typeof(TestData))]
        public async Task CreateBudget_ReturnsOk_WhenInputIsValid(
            string username,
            BudgetManageDto budgetManageDto,
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

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");

                var requestUrl = "api/Budget/post";

                if (optionalOwnerId != null)
                {
                    requestUrl += $"?optionalOwnerId={optionalOwnerId}";
                }

                var jsonContent = new StringContent(JsonConvert.SerializeObject(budgetManageDto), Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                // Act
                try
                {
                    var response = await client.PostAsync(requestUrl, jsonContent);

                    if(response.StatusCode == HttpStatusCode.BadRequest)
                    {

                    }
                    // Assert
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                }
                finally
                {

                    bool isAdded = db.Budgets.Any(g => g.Title.Equals(budgetManageDto.Title));
                    Assert.True(isAdded);
                }

            }
        }


        [Theory]
        [MemberData(nameof(TestData.CreateBudgetBadRequestTestTestData), MemberType = typeof(TestData))]
        public async Task CreateBudget_ReturnsBadrequest_WhenInputIsInvalid(
            string username,
            BudgetManageDto budgetManageDto,
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

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");

                var requestUrl = "api/Budget/post";

                if (optionalOwnerId != null)
                {
                    requestUrl += $"?optionalOwnerId={optionalOwnerId}";
                }

                var jsonContent = new StringContent(JsonConvert.SerializeObject(budgetManageDto), Encoding.UTF8, "application/json");

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
                    bool isAdded = db.Goals.Any(g => g.Title.Equals(budgetManageDto.Title));
                    Assert.False(isAdded);
                    if (isAdded)
                    {
                        string ownerId = optionalOwnerId ?? user.Id;

                        db.Goals.Remove(db.Goals.First(g => g.User.Id.Equals(ownerId)
                        && g.Title.Equals(budgetManageDto.Title)));
                        db.SaveChanges();
                    }
                }

            }
        }


        [Theory]
        [MemberData(nameof(TestData.UpdateBudgetValidInputTestData), MemberType = typeof(TestData))]
        public async Task UpdateBudget_ReturnsOk_WhenGoalExistsAndInputIsValid(
            string username,
            BudgetManageDto budgetManageDto,
            string? optionalOwnerUsername
            )
        {
            using (var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var user = db.Users.First(x => x.UserName.Equals(username));

                var budgetToBeUpdated = db.Budgets.AsNoTracking().First(x => x.Id == budgetManageDto.Id);

                string? optionalOwnerId = null;
                if (optionalOwnerUsername != null)
                {
                    optionalOwnerId = db.Users.FirstOrDefault(x => x.UserName.Equals(optionalOwnerUsername)).Id;
                }

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var jsonContent = new StringContent(JsonConvert.SerializeObject(budgetManageDto), Encoding.UTF8, "application/json");

                var requestUrl = optionalOwnerId == null
                    ? $"api/Budget/put"
                    : $"api/Budget/put?optionalOwnerId={optionalOwnerId}";

                try
                {
                    var response = await client.PutAsync(requestUrl, jsonContent);
                    if(response.StatusCode != HttpStatusCode.OK)
                    {

                    }
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                }
                finally
                {
                    var isUpdated = db.Budgets.AsNoTracking().ToList()
                        .Select(Map.ToBudgetDto)
                        .Any(x =>
                        {
                            bool isSame =
                                x.Id == budgetManageDto.Id &&
                                x.Title.ToLower().Equals(budgetManageDto.Title.ToLower()) &&
                                x.Description.ToLower().Equals(budgetManageDto.Description.ToLower()) &&
                                x.Currency.ToLower().Equals(budgetManageDto.Currency.ToLower()) &&
                                x.StartDate.Equals(budgetManageDto.StartDate) &&
                                x.StartDate.Equals(budgetManageDto.StartDate) &&
                                x.EndDate.Equals(budgetManageDto.EndDate) &&
                                x.LimitAmount == budgetManageDto.LimitAmount;

                            return isSame;
                        });

                    Assert.True(isUpdated);
                }
            }
        }


        [Theory]
        [MemberData(nameof(TestData.UpdateBudgetBadRequestInputTestData), MemberType = typeof(TestData))]
        public async Task UpdateBudget_ReturnsBadRequest_WhenGoalExistsAndInputIsInvalid(
            string username,
            BudgetManageDto budgetManageDto,
            string? optionalOwnerUsername
            )
        {

            using (var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var user = db.Users.First(x => x.UserName.Equals(username));

                var budgetToBeUpdated = db.Budgets.AsNoTracking().First(x => x.Id == budgetManageDto.Id);

                string? optionalOwnerId = null;
                if (optionalOwnerUsername != null)
                {
                    optionalOwnerId = db.Users.FirstOrDefault(x => x.UserName.Equals(optionalOwnerUsername)).Id;
                }

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var jsonContent = new StringContent(JsonConvert.SerializeObject(budgetManageDto), Encoding.UTF8, "application/json");

                var requestUrl = optionalOwnerId == null
                    ? $"api/Budget/put"
                    : $"api/Budget/put?optionalOwnerId={optionalOwnerId}";

                try
                {
                    var response = await client.PutAsync(requestUrl, jsonContent);
                    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                }
                finally
                {
                    var isUpdated = db.Budgets.AsNoTracking().ToList()
                        .Select(Map.ToBudgetDto)
                        .Any(x =>
                        {
                            bool isSame =
                                x.Id == budgetManageDto.Id &&
                                x.Title.ToLower().Equals(budgetManageDto.Title.ToLower()) &&
                                x.Description.ToLower().Equals(budgetManageDto.Description.ToLower()) &&
                                x.Currency.ToLower().Equals(budgetManageDto.Currency.ToLower()) &&
                                x.StartDate.Equals(budgetManageDto.StartDate) &&
                                x.StartDate.Equals(budgetManageDto.StartDate) &&
                                x.EndDate.Equals(budgetManageDto.EndDate) &&
                                x.LimitAmount == budgetManageDto.LimitAmount;

                            return isSame;
                        });

                    Assert.False(isUpdated);
                }
            }
        }


        [Theory]
        [MemberData(nameof(TestData.UpdateBudgetNotFoundRequestInputTestData), MemberType = typeof(TestData))]
        public async Task UpdateBudget_ReturnsNotFound_WhenGoalDoesNotExistOrIsNotUsersGoal(
            string username,
            BudgetManageDto budgetManageDto,
            string? optionalOwnerUsername
            )
        {
            using (var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var user = db.Users.First(x => x.UserName.Equals(username));

                var budgetToBeUpdatedIfExists = db.Goals.AsNoTracking().FirstOrDefault(x => x.Id == budgetManageDto.Id);

                string? optionalOwnerId = null;
                if (optionalOwnerUsername != null)
                {
                    optionalOwnerId = db.Users.FirstOrDefault(x => x.UserName.Equals(optionalOwnerUsername)).Id;
                }

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var jsonContent = new StringContent(JsonConvert.SerializeObject(budgetManageDto), Encoding.UTF8, "application/json");

                var requestUrl = optionalOwnerId == null
                    ? $"api/Budget/put"
                    : $"api/Budget/put?optionalOwnerId={optionalOwnerId}";

                try
                {
                    var response = await client.PutAsync(requestUrl, jsonContent);
                    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
                }
                finally
                {
                    if (budgetToBeUpdatedIfExists != null)
                    {
                        var isUpdated = db.Budgets.AsNoTracking().ToList()
                            .Select(Map.ToBudgetDto)
                            .Any(x =>
                            {
                                bool isSame =
                                    x.Id == budgetManageDto.Id &&
                                    x.Title.ToLower().Equals(budgetManageDto.Title.ToLower()) &&
                                    x.Description.ToLower().Equals(budgetManageDto.Description.ToLower()) &&
                                    x.Currency.ToLower().Equals(budgetManageDto.Currency.ToLower()) &&
                                    x.StartDate.Equals(budgetManageDto.StartDate) &&
                                    x.StartDate.Equals(budgetManageDto.StartDate) &&
                                    x.EndDate.Equals(budgetManageDto.EndDate) &&
                                    x.LimitAmount == budgetManageDto.LimitAmount;

                                return isSame;
                            });

                        Assert.False(isUpdated);
                    }
                }
            }
        }


        [Theory]
        [MemberData(nameof(TestData.DeleteBudgetValidTestData), MemberType = typeof(TestData))]
        public async Task DeleteBudget_ReturnsOk_WhenGoalExists(
            string username,
            int budgetId,
            string? optionalOwnerUsername
            )
        {
            using (var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var user = db.Users.First(x => x.UserName.Equals(username));

                var budgetToBeDeleted = db.Budgets.AsNoTracking().First(x => x.Id == budgetId);

                string? optionalOwnerId = null;
                if (optionalOwnerUsername != null)
                {
                    optionalOwnerId = db.Users.FirstOrDefault(x => x.UserName.Equals(optionalOwnerUsername)).Id;
                }

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var requestUrl = optionalOwnerId == null
                    ? $"api/Budget/delete/{budgetId}"
                    : $"api/Budget/delete/{budgetId}?optionalOwnerId={optionalOwnerId}";

                try
                {

                    var response = await client.DeleteAsync(requestUrl);
                    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
                }
                finally
                {
                    bool isDeleted = !db.Budgets.Any(x => x.Equals(budgetToBeDeleted));
                    if (!isDeleted)
                    {

                    }
                    Assert.True(isDeleted);
                }

            }
        }

        [Theory]
        [MemberData(nameof(TestData.DeleteBudgetNotFoundInputTestData), MemberType = typeof(TestData))]
        public async Task DeleteBudget_ReturnsNotFound_WhenUserDoesNotHaveAGoalWithInputId(
            string username,
            int budgetId,
            string? optionalOwnerUsername
            )
        {
            using (var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var user = db.Users.First(x => x.UserName.Equals(username));

                var budgetToBeDeleted = db.Budgets.AsNoTracking().FirstOrDefault(x => x.Id == budgetId);

                string? optionalOwnerId = null;
                if (optionalOwnerUsername != null)
                {
                    optionalOwnerId = db.Users.FirstOrDefault(x => x.UserName.Equals(optionalOwnerUsername)).Id;
                }

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var requestUrl = optionalOwnerId == null
                    ? $"api/Budget/delete/{budgetId}"
                    : $"api/Budget/delete/{budgetId}?optionalOwnerId={optionalOwnerId}";

                try
                {
                    var response = await client.DeleteAsync(requestUrl);

                    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
                }
                finally
                {
                    bool isDeleted = !db.Budgets.Any(x => x.Equals(budgetToBeDeleted));
                    Assert.False(isDeleted);
                }

            }
        }




        [Theory]
        [MemberData(nameof(TestData.AddCategoryToBudgetValidInputTestData), MemberType = typeof(TestData))]
        public async Task AddCategoryToBudget_ReturnsOk_WhenInputIsValidAndDoesNotExist(
            string username,
            int budgetId,
            List<int> categoryIds,
            string? optionalOwnerUsername
            )
        {
            using (var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();

                var user = db.Users.First(x => x.UserName.Equals(username));

                string? optionalOwnerId = null;
                if (optionalOwnerUsername != null)
                {
                    optionalOwnerId = db.Users.FirstOrDefault(x => x.UserName.Equals(optionalOwnerUsername)).Id;
                }

                var jsonContent = new StringContent(JsonConvert.SerializeObject(categoryIds), Encoding.UTF8, "application/json");

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var requestUrl = optionalOwnerId == null
                    ? $"api/Budget/associate_categories/{budgetId}"
                    : $"api/Budget/associate_categories/{budgetId}?optionalOwnerId={optionalOwnerId}";


                try
                {
                    var response = await client.PostAsync(requestUrl, jsonContent);

                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                }
                finally
                {
                    int amountAdded = db.BudgetCategories
                        .Where(x => x.BudgetId == budgetId && categoryIds.Contains(x.CategoryId)).Count();
                    bool allAdded = categoryIds.Count() == amountAdded;
                    Assert.True(allAdded);
                }
            }
        }

        [Theory]
        [MemberData(nameof(TestData.AddCategoryToBudgetBadRequestInputTestData), MemberType = typeof(TestData))]
        public async Task AddCategoryToBudget_ReturnsBadRequest_WhenInputIsWrongOrGoalCategoryAlreadyExists(
            string username,
            int budgetId,
            List<int> categoryIds,
            string? optionalOwnerUsername
            )
        {
            using (var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();

                var user = db.Users.First(x => x.UserName.Equals(username));

                string? optionalOwnerId = null;
                if (optionalOwnerUsername != null)
                {
                    optionalOwnerId = db.Users.FirstOrDefault(x => x.UserName.Equals(optionalOwnerUsername)).Id;
                }

                var jsonContent = new StringContent(JsonConvert.SerializeObject(categoryIds), Encoding.UTF8, "application/json");

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var requestUrl = optionalOwnerId == null
                    ? $"api/Goal/associate_categories/{budgetId}"
                    : $"api/Goal/associate_categories/{budgetId}?optionalOwnerId={optionalOwnerId}";

                var response = await client.PostAsync(requestUrl, jsonContent);
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [Theory]
        [MemberData(nameof(TestData.AddCategoryToBudgetNotFoundInputTestData), MemberType = typeof(TestData))]
        public async Task AddCategoryToBudget_ReturnsNotFound_WhenInputIsWrongOrGoalCategoryAlreadyExists(
            string username,
            int budgetId,
            List<int> categoryIds,
            string? optionalOwnerUsername
            )
        {
            using (var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();

                var user = db.Users.First(x => x.UserName.Equals(username));

                string? optionalOwnerId = null;
                if (optionalOwnerUsername != null)
                {
                    optionalOwnerId = db.Users.FirstOrDefault(x => x.UserName.Equals(optionalOwnerUsername)).Id;
                }

                var jsonContent = new StringContent(JsonConvert.SerializeObject(categoryIds), Encoding.UTF8, "application/json");

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var requestUrl = optionalOwnerId == null
                    ? $"api/Budget/associate_categories/{budgetId}"
                    : $"api/Budget/associate_categories/{budgetId}?optionalOwnerId={optionalOwnerId}";

                var response = await client.PostAsync(requestUrl, jsonContent);
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Theory]
        [MemberData(nameof(TestData.RemoveCategoriesFromBudgetValidInputTestData), MemberType = typeof(TestData))]
        public async Task RemoveCategoriesFromBudget_ReturnsOk_WhenGoalCategoryExistsAndIsUsers(
            string username,
            int budgetId,
            int categoryId,
            string? optionalOwnerUsername
            )
        {
            using (var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();

                var user = db.Users.First(x => x.UserName.Equals(username));

                var budgetCategoryToBeDeleted =
                    db.BudgetCategories.AsNoTracking().First(x => x.BudgetId == budgetId && x.CategoryId == categoryId);

                string? optionalOwnerId = null;
                if (optionalOwnerUsername != null)
                {
                    optionalOwnerId = db.Users.FirstOrDefault(x => x.UserName.Equals(optionalOwnerUsername)).Id;
                }

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var requestUrl = optionalOwnerId == null
                    ? $"api/Budget/remove_category/{budgetId}/{categoryId}"
                    : $"api/Budget/remove_category/{budgetId}/{categoryId}?optionalOwnerId={optionalOwnerId}";

                try
                {
                    var response = await client.DeleteAsync(requestUrl);

                    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
                }
                finally
                {
                    bool isDeleted = !db.BudgetCategories.Any(x => x.Equals(budgetCategoryToBeDeleted));
                    Assert.True(isDeleted);
                }
            }
        }


        [Theory]
        [MemberData(nameof(TestData.RemoveCategoriesFromBudgetNotFoundInputsTestData), MemberType = typeof(TestData))]
        public async Task RemoveCategoriesFromBudget_ReturnsNotFound_WhenGoalOrCategoryDoesNotExist(
            string username,
            int budgetId,
            int categoryId,
            string? optionalOwnerUsername
            )
        {
            using (var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();

                var user = db.Users.First(x => x.UserName.Equals(username));

                var budgetCategoryToBeDeleted =
                    db.BudgetCategories.AsNoTracking().FirstOrDefault(x => x.BudgetId == budgetId && x.CategoryId == categoryId);

                string? optionalOwnerId = null;
                if (optionalOwnerUsername != null)
                {
                    optionalOwnerId = db.Users.FirstOrDefault(x => x.UserName.Equals(optionalOwnerUsername)).Id;
                }

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var requestUrl = optionalOwnerId == null
                    ? $"api/Budget/remove_category/{budgetId}/{categoryId}"
                    : $"api/Budget/remove_category/{budgetId}/{categoryId}?optionalOwnerId={optionalOwnerId}";

                try
                {
                    var response = await client.DeleteAsync(requestUrl);

                    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
                }
                finally
                {
                    if (budgetCategoryToBeDeleted != null)
                    {
                        //CHECK GOAL INTEGRATION TEST
                        bool isDeleted = !db.BudgetCategories.Any(x => x.Equals(budgetCategoryToBeDeleted));
                        Assert.False(isDeleted);
                    }
                }
            }
        }


        [Theory]
        [MemberData(nameof(TestData.RemoveCategoriesFromBudgetBadRequestInputsTestData), MemberType = typeof(TestData))]
        public async Task RemoveCategoriesFromBudget_ReturnsBadRequest_WhenGoalDoesNotHaveACategoryOrACategpry(
            string username,
            int budgetId,
            int categoryId,
            string? optionalOwnerUsername
        )
        {
            using (var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();

                var user = db.Users.First(x => x.UserName.Equals(username));

                var budgetCategoryToBeDeleted =
                    db.BudgetCategories.AsNoTracking().FirstOrDefault(x => x.BudgetId == budgetId && x.CategoryId == categoryId);

                string? optionalOwnerId = null;
                if (optionalOwnerUsername != null)
                {
                    optionalOwnerId = db.Users.FirstOrDefault(x => x.UserName.Equals(optionalOwnerUsername)).Id;
                }

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var requestUrl = optionalOwnerId == null
                    ? $"api/Budget/remove_category/{budgetId}/{categoryId}"
                    : $"api/Budget/remove_category/{budgetId}/{categoryId}?optionalOwnerId={optionalOwnerId}";

                try
                {
                    var response = await client.DeleteAsync(requestUrl);

                    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                }
                finally
                {
                    if (budgetCategoryToBeDeleted != null)
                    {
                        //CHECK GOAL INTEGRATION TEST
                        bool isDeleted = !db.BudgetCategories.Any(x => x.Equals(budgetCategoryToBeDeleted));
                        Assert.False(isDeleted);
                    }
                }
            }
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
