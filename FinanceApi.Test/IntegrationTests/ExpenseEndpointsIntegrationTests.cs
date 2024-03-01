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
using FinanceApi.Test.TestDatabase;

namespace FinanceApi.Test.IntegrationTests
{
    public class ExpenseEndpointsIntegrationTests : IDisposable
    {
        private CustomWebApplicationFactory factory;
        private HttpClient client;
        public ExpenseEndpointsIntegrationTests()
        {
            factory = new CustomWebApplicationFactory();
            client = factory.CreateClient();

        }



        [Theory]
        [MemberData(nameof(TestData.GetExpenseValidInputTestData), MemberType = typeof(TestData))]
        public async Task GetExpenses_ReturnsOkObjectResult_WhenUserIsValid(
            string userName,
            DateTime? from,
            DateTime? to,
            string? listOrderBy,
            string? listDir,
            string? optionalOwnerUsername
            )
        {
            using (var scope = factory.Services.CreateScope())
            {

                var db = scope.ServiceProvider.GetRequiredService<DataContext>();

                string? optionalOwnerId = null;
                if (optionalOwnerUsername != null)
                {
                    optionalOwnerId = db.Users.FirstOrDefault(x => x.UserName == optionalOwnerUsername).Id;
                }
                var user = db.Users.First(x => x.UserName == userName);


                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                object[] paramValues = { from, to, listOrderBy, listDir, optionalOwnerId };
                string[] paramNames = { "from", "to", "listOrderBy", "listDir", "optionalOwnerId" };
                var requestUrl = $"/api/Expense/current";
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

                List<ExpenseDto> responseData = JsonConvert.DeserializeObject<List<ExpenseDto>>(await response.Content.ReadAsStringAsync());


                var ownerId = optionalOwnerId ?? user.Id;

                List<ExpenseDto> correctResponse = null;

                correctResponse = db.Expenses.Where(x =>
                    x.User.Id.Equals(ownerId)
                    && (from == null || x.Date >= from)
                    && (to == null || x.Date <= to))
                    .Select(Map.ToExpenseDto).ToList();



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
                        case "urgency":
                            correctResponse = listDir != null && listDir.Equals("desc") ?
                                correctResponse.OrderByDescending(x => x.Urgency).ToList()
                                :
                                correctResponse.OrderBy(x => x.Urgency).ToList();
                            break;
                        default:
                            correctResponse = listDir != null && listDir.Equals("desc") ?
                                correctResponse.OrderByDescending(x => x.Date).ToList()
                                :
                                correctResponse.OrderBy(x => x.Date).ToList();
                            break;
                    }
                }

                Assert.Equivalent(correctResponse, responseData);
            }
        }

        [Theory]
        [MemberData(nameof(TestData.CreateExpenseValidInputTestData), MemberType = typeof(TestData))]
        public async Task CreateExpense_ReturnsOk_WhenInputIsValid(
            string username,
            ExpenseDto expenseDto,
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

                var requestUrl = optionalOwnerId == null
                    ? $"api/Expense/post"
                    : $"api/Expense/post?optionalOwnerId={optionalOwnerId}";

                var jsonContent = new StringContent(JsonConvert.SerializeObject(expenseDto), Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);



                var response = await client.PostAsync(requestUrl, jsonContent);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            }
        }

        [Theory]
        [MemberData(nameof(TestData.CreateExpenseBadRequestInputTestData), MemberType = typeof(TestData))]
        public async Task CreateExpense_ReturnsBadRequest_WhenInputIsInvalid(
            string username,
            ExpenseDto expenseDto,
            string optionalOwnerUsername
            )
        {
            using (var scope = factory.Services.CreateScope())
            {
                // Arrange
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();

                string? optionalOwnerId = null;
                var user = db.Users.First(u => u.UserName.Equals(username));

                if (optionalOwnerUsername != null)
                {
                    optionalOwnerId = db.Users.FirstOrDefault(u => u.UserName.Equals(optionalOwnerUsername)).Id;
                }

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");

                var requestUrl = optionalOwnerId == null
                    ? $"api/Expense/post"
                    : $"api/Expense/post?optionalOwnerId={optionalOwnerId}";

                var jsonContent = new StringContent(JsonConvert.SerializeObject(expenseDto), Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);


                var response = await client.PostAsync(requestUrl, jsonContent);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            }
        }


        [Theory]
        [MemberData(nameof(TestData.UpdateExpenseValidInputTestData), MemberType = typeof(TestData))]
        public async Task UpdateExpense_ReturnsOk_WhenIncomeExistsAndInputIsValid(
            string username,
            ExpenseDto expenseDto,
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

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var jsonContent = new StringContent(JsonConvert.SerializeObject(expenseDto), Encoding.UTF8, "application/json");

                var requestUrl = optionalOwnerId == null
                    ? $"api/Expense/put"
                    : $"api/Expense/put?optionalOwnerId={optionalOwnerId}";

                try
                {
                    var response = await client.PutAsync(requestUrl, jsonContent);
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                }
                finally
                {
                    var isUpdated = db.Expenses.AsNoTracking().ToList()
                        .Any(x =>
                        {
                            bool isSame =
                                x.Id == expenseDto.Id &&
                                x.Title.ToLower().Equals(expenseDto.Title.ToLower()) &&
                                x.Description.ToLower().Equals(expenseDto.Description.ToLower()) &&
                                x.Currency.ToLower().Equals(expenseDto.Currency.ToLower()) &&
                                x.Date.Equals(expenseDto.Date) &&
                                x.DocumentUrl.ToLower().Equals(expenseDto.DocumentUrl.ToLower()) &&
                                x.Amount.Equals(expenseDto.Amount);

                            return isSame;
                        });

                    Assert.True(isUpdated);
                }
            }
        }


        [Theory]
        [MemberData(nameof(TestData.UpdateExpenseBadrequestInputTestData), MemberType = typeof(TestData))]
        public async Task UpdateExpense_ReturnsBadRequest_WhenIncomeExistsAndInputIsInvalid(
            string username,
            ExpenseDto expenseDto,
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

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var jsonContent = new StringContent(JsonConvert.SerializeObject(expenseDto), Encoding.UTF8, "application/json");

                var requestUrl = optionalOwnerId == null
                    ? $"api/Expense/put"
                    : $"api/Expense/put?optionalOwnerId={optionalOwnerId}";

                var response = await client.PutAsync(requestUrl, jsonContent);
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            }
        }


        [Theory]
        [MemberData(nameof(TestData.UpdateExpenseNotFoundInputTestData), MemberType = typeof(TestData))]
        public async Task UpdateExpense_ReturnsNotFound_WhenIncomeDoesNotExistOrIsNotUsersIncome(
            string username,
            ExpenseDto expenseDto,
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

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var jsonContent = new StringContent(JsonConvert.SerializeObject(expenseDto), Encoding.UTF8, "application/json");

                var requestUrl = optionalOwnerId == null
                    ? $"api/Expense/put"
                    : $"api/Expense/put?optionalOwnerId={optionalOwnerId}";

                var response = await client.PutAsync(requestUrl, jsonContent);
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            }
        }


        [Theory]
        [MemberData(nameof(TestData.DeleteExpenseValidInputTestData), MemberType = typeof(TestData))]
        public async Task DeleteExpense_ReturnsNoContent_WhenIncomeExists(
            string username,
            int expenseId,
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

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var requestUrl = optionalOwnerId == null
                    ? $"api/Expense/delete/{expenseId}"
                    : $"api/Expense/delete/{expenseId}?optionalOwnerId={optionalOwnerId}";

                try
                {
                    var response = await client.DeleteAsync(requestUrl);
                    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
                }
                finally
                {
                    bool isDeleted = !db.Expenses.Any(x => x.Id == expenseId);
                    Assert.True(isDeleted);

                }

            }
        }

        [Theory]
        [MemberData(nameof(TestData.DeleteExpenseNotFoundInputTestData), MemberType = typeof(TestData))]
        public async Task DeleteExpense_ReturnsNotFound_WhenIncomeDoesNotExists(
            string username,
            int expenseId,
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

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var requestUrl = optionalOwnerId == null
                    ? $"api/Expense/delete/{expenseId}"
                    : $"api/Expense/delete/{expenseId}?optionalOwnerId={optionalOwnerId}";

                var response = await client.DeleteAsync(requestUrl);

                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }




        [Theory()]
        [MemberData(nameof(TestData.AddCategoryToExpenseValidInputTestData), MemberType = typeof(TestData))]
        public async Task AddCategoryToExpense_ReturnsOk_WhenInputIsValidAndDoesNotExist(
            string username,
            int expenseId,
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

                var jsonCategoryIds = new StringContent(JsonConvert.SerializeObject(categoryIds), Encoding.UTF8, "application/json");

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var requestUrl = optionalOwnerId == null
                    ? $"api/Expense/associate_categories/{expenseId}"
                    : $"api/Expense/associate_categories/{expenseId}?optionalOwnerId={optionalOwnerId}";


                try
                {
                    var response = await client.PostAsync(requestUrl, jsonCategoryIds);
                    bool equal = HttpStatusCode.OK == response.StatusCode;
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    ///////////////////////////////////////////////
                }
                finally
                {
                    int amountAdded = db.ExpenseCategories
                        .Where(x => x.ExpenseId == expenseId && categoryIds.Contains(x.CategoryId)).Count();

                    bool allAdded = categoryIds.Count() == amountAdded;
                    if (!allAdded)
                    {

                    }
                    Assert.True(allAdded);
                }
            }
        }

        [Theory]
        [MemberData(nameof(TestData.AddCategoryToExpenseBadRequestInputTestData), MemberType = typeof(TestData))]
        public async Task AddCategoryToExpense_ReturnsBadRequest_WhenInputIsInvalid(
            string username,
            int expenseId,
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
                    ? $"api/Expense/associate_categories/{expenseId}"
                    : $"api/Expense/associate_categories/{expenseId}?optionalOwnerId={optionalOwnerId}";

                var response = await client.PostAsync(requestUrl, jsonContent);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [Theory]
        [MemberData(nameof(TestData.AddCategoryToExpenseNotFoundInputTestData), MemberType = typeof(TestData))]
        public async Task AddCategoryToExpense_ReturnsNotFound_WhenIncomeOrCategoryDoNotExist(
            string username,
            int expenseId,
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
                    ? $"api/Expense/associate_categories/{expenseId}"
                    : $"api/Expense/associate_categories/{expenseId}?optionalOwnerId={optionalOwnerId}";


                var response = await client.PostAsync(requestUrl, jsonContent);

                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Theory]
        [MemberData(nameof(TestData.RemoveCategoriesFromExpenseValidInputTestData), MemberType = typeof(TestData))]
        public async Task RemoveCategoriesFromExpense_ReturnsOk_WhenIncomeAndCategoryExistsAndIsUsers(
            string username,
            int expenseId,
            int categoryId,
            string? optionalOwnerUsername
            )
        {
            using (var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();

                var user = db.Users.First(x => x.UserName.Equals(username));

                var expenseCategoriesToBeDeleted =
                    db.ExpenseCategories.AsNoTracking().First(x => x.ExpenseId == expenseId && x.CategoryId == categoryId);

                string? optionalOwnerId = null;
                if (optionalOwnerUsername != null)
                {
                    optionalOwnerId = db.Users.FirstOrDefault(x => x.UserName.Equals(optionalOwnerUsername)).Id;
                }


                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var requestUrl = optionalOwnerId == null
                    ? $"api/Expense/remove_category/{expenseId}/{categoryId}"
                    : $"api/Expense/remove_category/{expenseId}/{categoryId}?optionalOwnerId={optionalOwnerId}";


                try
                {
                    var response = await client.DeleteAsync(requestUrl);

                    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
                }
                finally
                {
                    bool isDeleted = !db.ExpenseCategories.Any(x => x.Equals(expenseCategoriesToBeDeleted));
                    Assert.True(isDeleted);
                    if (isDeleted)
                    {
                        db.ExpenseCategories.Add(expenseCategoriesToBeDeleted);
                        db.SaveChanges();
                    }
                }
            }
        }


        [Theory]
        [MemberData(nameof(TestData.RemoveCategoriesFromExpenseNotFoundInputsTestData), MemberType = typeof(TestData))]
        public async Task RemoveCategoriesFromExpense_ReturnsNotFound_WhenGoalOrCategoryDoesNotExist(
            string username,
            int expenseId,
            int categoryId,
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

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var requestUrl = optionalOwnerId == null
                    ? $"api/Expense/remove_category/{expenseId}/{categoryId}"
                    : $"api/Expense/remove_category/{expenseId}/{categoryId}?optionalOwnerId={optionalOwnerId}";

                var response = await client.DeleteAsync(requestUrl);
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }


        [Theory]
        [MemberData(nameof(TestData.RemoveCategoriesFromExpenseBadRequestInputsTestData), MemberType = typeof(TestData))]
        public async Task RemoveCategoriesFromExpense_ReturnsBadRequest_WhenGoalDoesNotHaveACategoryOrACategpry(
            string username,
            int expenseId,
            int categoryId,
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

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var requestUrl = optionalOwnerId == null
                    ? $"api/Expense/remove_category/{expenseId}/{categoryId}"
                    : $"api/Expense/remove_category/{expenseId}/{categoryId}?optionalOwnerId={optionalOwnerId}";


                var response = await client.DeleteAsync(requestUrl);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
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
