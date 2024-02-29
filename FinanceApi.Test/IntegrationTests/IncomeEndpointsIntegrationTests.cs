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

[assembly: CollectionBehavior(DisableTestParallelization = true)]
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
        public async Task GetIncomes_ReturnsOkObjectResult_WhenUserIsValid2(
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
