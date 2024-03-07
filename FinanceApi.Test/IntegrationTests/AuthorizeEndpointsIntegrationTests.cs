using FinanceApi.Test.Utils;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using FinanceApi.Data;
using FinanceApi.Test.TestDataHolder;
using System.Net.Http.Headers;
using FinanceApi.Data.Dtos;

namespace FinanceApi.Test.IntegrationTests
{
    public class AuthorizeEndpointsIntegrationTests : IDisposable
    {

        private CustomWebApplicationFactory factory;
        private HttpClient client;

        public AuthorizeEndpointsIntegrationTests()
        {
            factory = new CustomWebApplicationFactory();
            client = factory.CreateClient();
        }

        [Theory]
        [MemberData(nameof(TestData.GetAllAuthorizationInvites), MemberType = typeof(TestData))]
        public async Task GetAllAuthorizationInvites_ReturnsOk_WhenUserIsLoggedIn(
            string username
            )
        {
            using (var scope = factory.Services.CreateScope()) 
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();

                var user = db.Users.First();

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                var requestUrl = $"api/authorize/get_all_authorization_invites";


                var response = await client.GetAsync(requestUrl);

                var data = JsonConvert.DeserializeObject<List<AuthorizeUserInviteDto>>(await response.Content.ReadAsStringAsync());

                var correctResponse = db.AuthorizeUserInvite.Where(x => x.AuthorizedUserId.Equals(user.Id));

                Assert.Equivalent(correctResponse, data);


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
