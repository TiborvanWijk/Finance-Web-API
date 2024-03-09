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

//[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace FinanceApi.Test.IntegrationTests
{
    public class IncomeEndpointsIntegrationTests : IDisposable
    {
        private CustomWebApplicationFactory factory;
        private HttpClient client;
        public IncomeEndpointsIntegrationTests()
        {
            factory = new CustomWebApplicationFactory();
            client = factory.CreateClient();

        }



        [Theory]
        [MemberData(nameof(TestData.GetIncomesValidInputTestData), MemberType = typeof(TestData))]
        public async Task GetIncomes_ReturnsOkObjectResult_WhenUserIsValid(
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
                var requestUrl = $"/api/Income/current";
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

                List<IncomeDto> responseData = JsonConvert.DeserializeObject<List<IncomeDto>>(await response.Content.ReadAsStringAsync());


                var ownerId = optionalOwnerId ?? user.Id;

                List<IncomeDto> correctResponse = null;

                correctResponse = db.Incomes.Where(x =>
                    x.User.Id.Equals(ownerId)
                    && (from == null || x.Date >= from)
                    && (to == null || x.Date <= to))
                    .Select(Map.ToIncomeDto).ToList();



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
        [MemberData(nameof(TestData.CreateIncomeValidInputTestData), MemberType = typeof(TestData))]
        public async Task CreateIncome_ReturnsOk_WhenInputIsValid(
            string username,
            IncomeDto incomeDto,
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
                    ? $"api/Income/post"
                    : $"api/Income/post?optionalOwnerId={optionalOwnerId}";

                var jsonContent = new StringContent(JsonConvert.SerializeObject(incomeDto), Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);



                var response = await client.PostAsync(requestUrl, jsonContent);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            }
        }

        [Theory]
        [MemberData(nameof(TestData.CreateIncomeBadRequestInputTestData), MemberType = typeof(TestData))]
        public async Task CreateIncome_ReturnsBadRequest_WhenInputIsInvalid(
            string username,
            IncomeDto incomeDto,
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
                    ? $"api/Income/post"
                    : $"api/Income/post?optionalOwnerId={optionalOwnerId}";

                var jsonContent = new StringContent(JsonConvert.SerializeObject(incomeDto), Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);


                var response = await client.PostAsync(requestUrl, jsonContent);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            }
        }


        [Theory]
        [MemberData(nameof(TestData.UpdateIncomeValidInputTestData), MemberType = typeof(TestData))]
        public async Task UpdateIncome_ReturnsOk_WhenIncomeExistsAndInputIsValid(
            string username,
            IncomeDto incomeDto,
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

                var jsonContent = new StringContent(JsonConvert.SerializeObject(incomeDto), Encoding.UTF8, "application/json");

                var requestUrl = optionalOwnerId == null
                    ? $"api/Income/put"
                    : $"api/Income/put?optionalOwnerId={optionalOwnerId}";

                try
                {
                    var response = await client.PutAsync(requestUrl, jsonContent);
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                }
                finally
                {
                    var isUpdated = db.Incomes.AsNoTracking().ToList()
                        .Any(x =>
                        {
                            bool isSame =
                                x.Id == incomeDto.Id &&
                                x.Title.ToLower().Equals(incomeDto.Title.ToLower()) &&
                                x.Description.ToLower().Equals(incomeDto.Description.ToLower()) &&
                                x.Currency.ToLower().Equals(incomeDto.Currency.ToLower()) &&
                                x.Date.Equals(incomeDto.Date) &&
                                x.DocumentUrl.ToLower().Equals(incomeDto.DocumentUrl.ToLower()) &&
                                x.Amount.Equals(incomeDto.Amount);

                            return isSame;
                        });

                    Assert.True(isUpdated);
                }
            }
        }


        [Theory]
        [MemberData(nameof(TestData.UpdateIncomeBadrequestInputTestData), MemberType = typeof(TestData))]
        public async Task UpdateIncome_ReturnsBadRequest_WhenIncomeExistsAndInputIsInvalid(
            string username,
            IncomeDto incomeDto,
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

                var jsonContent = new StringContent(JsonConvert.SerializeObject(incomeDto), Encoding.UTF8, "application/json");

                var requestUrl = optionalOwnerId == null
                    ? $"api/Income/put"
                    : $"api/Income/put?optionalOwnerId={optionalOwnerId}";

                var response = await client.PutAsync(requestUrl, jsonContent);
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            }
        }


        [Theory]
        [MemberData(nameof(TestData.UpdateIncomeNotFoundInputTestData), MemberType = typeof(TestData))]
        public async Task UpdateIncome_ReturnsNotFound_WhenIncomeDoesNotExistOrIsNotUsersIncome(
            string username,
            IncomeDto incomeDto,
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

                var jsonContent = new StringContent(JsonConvert.SerializeObject(incomeDto), Encoding.UTF8, "application/json");

                var requestUrl = optionalOwnerId == null
                    ? $"api/Income/put"
                    : $"api/Income/put?optionalOwnerId={optionalOwnerId}";

                var response = await client.PutAsync(requestUrl, jsonContent);
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            }
        }


        [Theory]
        [MemberData(nameof(TestData.DeleteIncomeValidInputTestData), MemberType = typeof(TestData))]
        public async Task DeleteIncome_ReturnsNoContent_WhenIncomeExists(
            string username,
            int incomeId,
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
                    ? $"api/Income/delete/{incomeId}"
                    : $"api/Income/delete/{incomeId}?optionalOwnerId={optionalOwnerId}";

                try
                {
                    var response = await client.DeleteAsync(requestUrl);
                    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
                }
                finally
                {
                    bool isDeleted = !db.Incomes.Any(x => x.Id == incomeId);
                    Assert.True(isDeleted);

                }

            }
        }

        [Theory]
        [MemberData(nameof(TestData.DeleteIncomeNotFoundInputTestData), MemberType = typeof(TestData))]
        public async Task DeleteIncome_ReturnsNotFound_WhenIncomeDoesNotExists(
            string username,
            int incomeId,
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
                    ? $"api/Income/delete/{incomeId}"
                    : $"api/Income/delete/{incomeId}?optionalOwnerId={optionalOwnerId}";

                var response = await client.DeleteAsync(requestUrl);

                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }




        [Theory]
        [MemberData(nameof(TestData.AddCategoryToIncomeValidInputTestData), MemberType = typeof(TestData))]
        public async Task AddCategory_ReturnsOk_WhenInputIsValidAndDoesNotExist(
            string username,
            int incomeId,
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
                    ? $"api/Income/associate_categories/{incomeId}"
                    : $"api/Income/associate_categories/{incomeId}?optionalOwnerId={optionalOwnerId}";


                try
                {
                    var response = await client.PostAsync(requestUrl, jsonCategoryIds);

                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                }
                finally
                {
                    int amountAdded = db.IncomeCategories
                        .Where(x => x.IncomeId == incomeId && categoryIds.Contains(x.CategoryId)).Count();
                    bool allAdded = categoryIds.Count() == amountAdded;
                    Assert.True(allAdded);

                }
            }
        }

        [Theory]
        [MemberData(nameof(TestData.AddCategoryToIncomeBadRequestInputTestData), MemberType = typeof(TestData))]
        public async Task AddCategory_ReturnsBadRequest_WhenInputIsInvalid(
            string username,
            int incomeId,
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
                    ? $"api/Income/associate_categories/{incomeId}"
                    : $"api/Income/associate_categories/{incomeId}?optionalOwnerId={optionalOwnerId}";

                var response = await client.PostAsync(requestUrl, jsonCategoryIds);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [Theory]
        [MemberData(nameof(TestData.AddCategoryToIncomeNotFoundInputTestData), MemberType = typeof(TestData))]
        public async Task AddCategory_ReturnsNotFound_WhenIncomeOrCategoryDoNotExist(
            string username,
            int incomeId,
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
                    ? $"api/Income/associate_categories/{incomeId}"
                    : $"api/Income/associate_categories/{incomeId}?optionalOwnerId={optionalOwnerId}";


                var response = await client.PostAsync(requestUrl, jsonCategoryIds);

                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Theory]
        [MemberData(nameof(TestData.RemoveCategoriesFromIncomeValidInputTestData), MemberType = typeof(TestData))]
        public async Task RemoveCategories_ReturnsOk_WhenIncomeAndCategoryExistsAndIsUsers(
            string username,
            int incomeId,
            int categoryId,
            string? optionalOwnerUsername
            )
        {
            using (var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();

                var user = db.Users.First(x => x.UserName.Equals(username));

                var incomeCategoriesToBeDeleted =
                    db.IncomeCategories.AsNoTracking().First(x => x.IncomeId == incomeId && x.CategoryId == categoryId);

                string? optionalOwnerId = null;
                if (optionalOwnerUsername != null)
                {
                    optionalOwnerId = db.Users.FirstOrDefault(x => x.UserName.Equals(optionalOwnerUsername)).Id;
                }


                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var requestUrl = optionalOwnerId == null
                    ? $"api/Income/remove_category/{incomeId}/{categoryId}"
                    : $"api/Income/remove_category/{incomeId}/{categoryId}?optionalOwnerId={optionalOwnerId}";


                try
                {
                    var response = await client.DeleteAsync(requestUrl);

                    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
                }
                finally
                {
                    bool isDeleted = !db.IncomeCategories.Any(x => x.Equals(incomeCategoriesToBeDeleted));
                    Assert.True(isDeleted);
                    if (isDeleted)
                    {
                        db.IncomeCategories.Add(incomeCategoriesToBeDeleted);
                        db.SaveChanges();
                    }
                }
            }
        }


        [Theory]
        [MemberData(nameof(TestData.RemoveCategoriesIncomeNotFoundInputsTestData), MemberType = typeof(TestData))]
        public async Task RemoveCategories_ReturnsNotFound_WhenGoalOrCategoryDoesNotExist(
            string username,
            int incomeId,
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
                    ? $"api/Income/remove_category/{incomeId}/{categoryId}"
                    : $"api/Income/remove_category/{incomeId}/{categoryId}?optionalOwnerId={optionalOwnerId}";


                var response = await client.DeleteAsync(requestUrl);
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            }
        }


        [Theory]
        [MemberData(nameof(TestData.RemoveCategoriesFromIncomeBadRequestInputsTestData), MemberType = typeof(TestData))]
        public async Task RemoveCategories_ReturnsBadRequest_WhenGoalDoesNotHaveACategoryOrACategpry(
            string username,
            int incomeId,
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
                    ? $"api/Income/remove_category/{incomeId}/{categoryId}"
                    : $"api/Income/remove_category/{incomeId}/{categoryId}?optionalOwnerId={optionalOwnerId}";


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
