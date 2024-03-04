using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinanceApi.Test.Utils;
using FinanceApi.Test.TestDataHolder;
using Microsoft.Extensions.DependencyInjection;
using FinanceApi.Data;
using System.Net.Http.Headers;
using System.Net;
using FinanceApi.Data.Dtos;
using FinanceApi.Mapper;
using Azure;

namespace FinanceApi.Test.IntegrationTests
{
    public class CategoryEndpointsIntegrationTests : IDisposable
    {
        private CustomWebApplicationFactory factory;
        private HttpClient client;
        public CategoryEndpointsIntegrationTests()
        {
            factory = new CustomWebApplicationFactory();
            client = factory.CreateClient();
        }


        [Theory]
        [MemberData(nameof(TestData.GetCategoryValidInputTestData), MemberType = typeof(TestData))]
        public async Task GetCategories_returnsOk_WhenUserIsLoggedIn(
            string username,
            string? listOrderBy,
            string? listDir,
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
                    optionalOwnerId = db.Users.First(x => x.UserName.Equals(optionalOwnerUsername)).Id;
                }

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var requestUrl = "api/Category/current";

                string?[] paramValues = { listOrderBy, listDir, optionalOwnerId };
                string[] paramNames = { "listOrderBy", "listDir", "optionalOwnerId" };
                bool added = false;
                for (int i = 0; i < paramValues.Length; ++i)
                {
                    var value = paramValues[i];
                    if (value != null)
                    {
                        requestUrl += added ? $"&{paramNames[i]}={value}" : $"?{paramNames[i]}={value}";
                        added = true;
                    }
                }

                var response = await client.GetAsync(requestUrl);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                List<CategoryDto> responseData = JsonConvert.DeserializeObject<List<CategoryDto>>(await response.Content.ReadAsStringAsync());

                var ownerId = optionalOwnerId ?? user.Id;


                List<CategoryDto> correctResponse = db.Categories
                    .Where(x => x.User.Id.Equals(ownerId))
                    .Select(Map.ToCategoryDto)
                    .Select(x =>
                    {
                        x.IncomeAmount = db.Incomes
                        .Where(i => i.IncomeCategories.Any(ic => ic.Category.Id.Equals(x.Id)))
                        .Select(i => i.Amount).Sum();

                        x.ExpenseAmount = db.Expenses
                        .Where(i => i.ExpenseCategories.Any(ic => ic.Category.Id.Equals(x.Id)))
                        .Select(i => i.Amount).Sum();

                        return x;
                    })
                    .ToList();



                if (listDir != null || listOrderBy != null)
                {
                    switch (listOrderBy)
                    {
                        case "title":
                            correctResponse = listDir != null && listDir.Equals("desc") ?
                                correctResponse.OrderByDescending(x => x.Title).ToList()
                                :
                                correctResponse.OrderBy(x => x.Title).ToList();
                            break;
                        case "expense":
                            correctResponse = listDir != null && listDir.Equals("desc") ?
                                correctResponse.OrderByDescending(x => x.ExpenseAmount).ToList()
                                :
                                correctResponse.OrderBy(x => x.ExpenseAmount).ToList();
                            break;
                        case "income":
                            correctResponse = listDir != null && listDir.Equals("desc") ?
                                correctResponse.OrderByDescending(x => x.IncomeAmount).ToList()
                                :
                                correctResponse.OrderBy(x => x.IncomeAmount).ToList();
                            break;
                        default:
                            correctResponse = listDir != null && listDir.Equals("desc") ?
                                correctResponse.OrderByDescending(x => x.Id).ToList()
                                :
                                correctResponse.OrderBy(x => x.Id).ToList();
                            break;
                    }
                }

                Assert.Equivalent(correctResponse, responseData);
            }
        }

        [Fact]
        public async Task GetCategories_returnsUnauthorized_WhenUserIsNotLoggedIn()
        {
            var requestUrl = "api/Category/current";

            var response = await client.GetAsync(requestUrl);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }



        [Theory]
        [MemberData(nameof(TestData.CreateCategoryValidInputTestData), MemberType = typeof(TestData))]
        public async Task CreateCategory_ReturnsOk_WhenInputIsValid(
            string username,
            CategoryManageDto categoryManageDto,
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
                    optionalOwnerId = db.Users.First(x => x.UserName.Equals(optionalOwnerUsername)).Id;
                }

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                var jsonContent = new StringContent(JsonConvert.SerializeObject(categoryManageDto), Encoding.UTF8, "application/json");

                var requestUrl = optionalOwnerId == null
                    ? "api/Category/post"
                    : $"api/Category/post?optionalOwnerId={optionalOwnerId}";


                var response = await client.PostAsync(requestUrl, jsonContent);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }



        [Theory]
        [MemberData(nameof(TestData.CreateCategoryBadRequestTestData), MemberType = typeof(TestData))]
        public async Task CreateCategory_ReturnsBadRequest_WhenTitleAlreadyExists(
            string username,
            CategoryManageDto categoryManageDto,
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
                    optionalOwnerId = db.Users.First(x => x.UserName.Equals(optionalOwnerUsername)).Id;
                }

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                var jsonContent = new StringContent(JsonConvert.SerializeObject(categoryManageDto), Encoding.UTF8, "application/json");

                var requestUrl = optionalOwnerId == null
                    ? "api/Category/post"
                    : $"api/Category/post?optionalOwnerId={optionalOwnerId}";


                var response = await client.PostAsync(requestUrl, jsonContent);
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }


        [Theory]
        [MemberData(nameof(TestData.UpdateCategoryValidInputTestData), MemberType = typeof(TestData))]
        public async Task UpdateCategory_ReturnsOk_WhenInputIsValid(
            string username,
            CategoryManageDto categoryManageDto,
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
                    optionalOwnerId = db.Users.First(x => x.UserName.Equals(optionalOwnerUsername)).Id;
                }

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var jsonContent = new StringContent(JsonConvert.SerializeObject(categoryManageDto), Encoding.UTF8, "application/json");

                var requestUrl = optionalOwnerId == null
                    ? "api/Category/put"
                    : $"api/Category/put?optionalOwnerId={optionalOwnerId}";

                var response = await client.PutAsync(requestUrl, jsonContent);


                Assert.Equal(HttpStatusCode.OK, response.StatusCode);


            }
        }

        [Theory]
        [MemberData(nameof(TestData.UpdateCategoryBadRequestTestData), MemberType = typeof(TestData))]
        public async Task UpdateCategory_ReturnsBadRequest_WhenAnotherCategoryUsesTheTitle(
            string username,
            CategoryManageDto categoryManageDto,
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
                    optionalOwnerId = db.Users.First(x => x.UserName.Equals(optionalOwnerUsername)).Id;
                }

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var jsonContent = new StringContent(JsonConvert.SerializeObject(categoryManageDto), Encoding.UTF8, "application/json");

                var requestUrl = optionalOwnerId == null
                    ? "api/Category/put"
                    : $"api/Category/put?optionalOwnerId={optionalOwnerId}";

                var response = await client.PutAsync(requestUrl, jsonContent);


                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }


        [Theory]
        [MemberData(nameof(TestData.UpdateCategoryNotFoundTestData), MemberType = typeof(TestData))]
        public async Task UpdateCategory_ReturnsNotFound_WhenCategoryDoesNotExist(
            string username,
            CategoryManageDto categoryManageDto,
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
                    optionalOwnerId = db.Users.First(x => x.UserName.Equals(optionalOwnerUsername)).Id;
                }

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var jsonContent = new StringContent(JsonConvert.SerializeObject(categoryManageDto), Encoding.UTF8, "application/json");

                var requestUrl = optionalOwnerId == null
                    ? "api/Category/put"
                    : $"api/Category/put?optionalOwnerId={optionalOwnerId}";

                var response = await client.PutAsync(requestUrl, jsonContent);

                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }


        [Theory]
        [MemberData(nameof(TestData.DeleteCategoryValidInputTestData), MemberType = typeof(TestData))]
        public async Task DeleteCategory_ReturnsNoContent_WhenInputIsValid(
            string username,
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
                    optionalOwnerId = db.Users.First(x => x.UserName.Equals(optionalOwnerUsername)).Id;
                }

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var requestUrl = optionalOwnerId == null
                    ? $"api/Category/delete/{categoryId}"
                    : $"api/Category/delete/{categoryId}?optionalOwnerId={optionalOwnerId}";


                var response = await client.DeleteAsync(requestUrl);

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            }
        }

        [Theory]
        [MemberData(nameof(TestData.DeleteCategoryNotFoundTestData), MemberType = typeof(TestData))]
        public async Task DeleteCategory_ReturnsNotFound_WhenInputIsValid(
            string username,
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
                    optionalOwnerId = db.Users.First(x => x.UserName.Equals(optionalOwnerUsername)).Id;
                }

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var requestUrl = optionalOwnerId == null
                    ? $"api/Category/delete/{categoryId}"
                    : $"api/Category/delete/{categoryId}?optionalOwnerId={optionalOwnerId}";


                var response = await client.DeleteAsync(requestUrl);

                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
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
