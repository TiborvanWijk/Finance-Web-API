using FinanceApi.Data;
using FinanceApi.Test.TestDataHolder;
using FinanceApi.Test.Utils;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net;
using FinanceApi.Data.Dtos;

namespace FinanceApi.Test.IntegrationTests
{
    [Collection("UserIntegrationTests")]
    public class UserEndpointsIntegrationTests : IDisposable
    {

        private CustomWebApplicationFactory factory;
        private HttpClient client;
        public UserEndpointsIntegrationTests()
        {
            factory = new CustomWebApplicationFactory();
            client = factory.CreateClient();
        }

        [Theory]
        [MemberData(nameof(TestData.GetUserValidInputTestData), MemberType = typeof(TestData))]
        public async Task GetUser_ReturnsOk_WhenUserIsLoggedIn(
            string username
            )
        {
            using (var scope = factory.Services.CreateScope())
            {

                var db = scope.ServiceProvider.GetRequiredService<DataContext>();

                var user = db.Users.First(x => x.UserName.Equals(username));
                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var requestUrl = "api/User/current";
                var response = await client.GetAsync(requestUrl);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                var responseData = JsonConvert.DeserializeObject<UserDto>(await response.Content.ReadAsStringAsync());
                Assert.Equal(user.Id, responseData.Id);

            }
        }


        [Fact]
        public async Task GetUser_ReturnsUnauthorized_WhenUserIsNotLoggedIn()
        {
            var requestUrl = "api/User/current";
            var response = await client.GetAsync(requestUrl);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory]
        [MemberData(nameof(TestData.UpdateUsersCurrencyValidInputTestData), MemberType = typeof(TestData))]
        public async Task UpdateUsersCurrency_ReturnsOk_WhenUserIsLoggedInAndInputIsValid(
            string username,
            string currency
            )
        {
            using (var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();

                var user = db.Users.First(x => x.UserName.Equals(username));

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var requestUrl = $"api/user/patch_currency/{currency}";

                var response = await client.PatchAsync(requestUrl, null);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Theory]
        [MemberData(nameof(TestData.UpdateUsersCurrencyBadRequestInputTestData), MemberType = typeof(TestData))]
        public async Task UpdateUsersCurrency_ReturnsBadRequest_WhenUserIsLoggedInAndCurrencyIsNotValid(
            string username,
            string currency
            )
        {
            using (var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();

                var user = db.Users.First(x => x.UserName.Equals(username));

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var requestUrl = $"api/user/patch_currency/{currency}";

                var response = await client.PatchAsync(requestUrl, null);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            }
        }
        [Fact]
        public async Task UpdateUsersCurrency_ReturnsUnauthorized_WhenUserIsNotLoggedIn()
        {
            using (var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();

                var requestUrl = $"api/user/patch_currency/usd";

                var response = await client.PatchAsync(requestUrl, null);

                Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

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
