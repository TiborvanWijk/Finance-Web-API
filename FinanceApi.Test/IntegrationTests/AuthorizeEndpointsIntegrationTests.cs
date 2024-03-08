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
using System.Net;
using FinanceApi.Mapper;

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
        [MemberData(nameof(TestData.GetAllAuthorizationInvitesValidTestData), MemberType = typeof(TestData))]
        public async Task GetAllAuthorizationInvites_ReturnsOk_WhenUserIsLoggedIn(
            string username
            )
        {
            using (var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();

                var user = db.Users.First(x => x.UserName.Equals(username));

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                var requestUrl = $"api/authorize/get_all_authorization_invites";


                var response = await client.GetAsync(requestUrl);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var data = JsonConvert.DeserializeObject<List<AuthorizeUserInviteDto>>(await response.Content.ReadAsStringAsync());

                var correctResponse = db.AuthorizeUserInvite.Where(x => x.AuthorizedUserId.Equals(user.Id)).Select(Map.ToAuthorizeUserInviteDto);

                Assert.Equivalent(correctResponse, data);


            }
        }

        [Fact]
        public async Task GetAllAuthorizationInvites_ReturnsUnauthorized_WhenUserIsNotLoggedIn()
        {
            var requestUrl = $"api/authorize/get_all_authorization_invites";

            var response = await client.GetAsync(requestUrl);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }


        [Theory]
        [MemberData(nameof(TestData.GetOutgoingAuthorizationInvitesValidTestData), MemberType = typeof(TestData))]
        public async Task GetOutgoingAuthorizationInvites_ReturnsOk_WhenUserIsLoggedIn(
            string username
            )
        {
            using (var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();

                var user = db.Users.First(x => x.UserName.Equals(username));

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                var requestUrl = $"api/authorize/get_all_outgoing_authorization_invites";


                var response = await client.GetAsync(requestUrl);

                var data = JsonConvert.DeserializeObject<List<AuthorizeUserInviteDto>>(await response.Content.ReadAsStringAsync());

                var correctResponse = db.AuthorizeUserInvite
                    .Where(x => x.OwnerId.Equals(user.Id)).Select(Map.ToAuthorizeUserInviteDto);

                Assert.Equivalent(correctResponse, data);
            }
        }


        [Fact]
        public async Task GetOutgoingAuthorizationInvites_ReturnsUnauthorized_WhenUserIsNotLoggedIn()
        {
            var requestUrl = $"api/authorize/get_all_outgoing_authorization_invites";
            var response = await client.GetAsync(requestUrl);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }


        [Theory]
        [MemberData(nameof(TestData.GetAllAuthorizedUsersValidTestData), MemberType = typeof(TestData))]
        public async Task GetAllAuthorizedUsers_ReturnsOk_WhenUserIsLoggedIn(
            string username
            )
        {
            using (var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();

                var user = db.Users.First(x => x.UserName.Equals(username));

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                var requestUrl = $"api/authorize/get_all_authorized_users";


                var response = await client.GetAsync(requestUrl);

                List<AuthorizedUserDto>? data = JsonConvert.DeserializeObject<List<AuthorizedUserDto>>(await response.Content.ReadAsStringAsync());
                var correctResponse = db.AuthorizedUsers.Where(x => x.OwnerId
                .Equals(user.Id)).Select(x => x.AuthorizedUser);

                Assert.Equivalent(correctResponse.Select(Map.ToAuthorizedUserDto), data);
            }
        }
        [Fact]
        public async Task GetAllAuthorizedUsers_ReturnsUnauthorized_WhenUserIsNotLoggedIn()
        {
            var requestUrl = $"api/authorize/get_all_authorized_users";
            var response = await client.GetAsync(requestUrl);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory]
        [MemberData(nameof(TestData.AuthorizeUserValidTestData), MemberType = typeof(TestData))]
        public async Task AuthorizeUser(
            string username,
            string authorizeUsername
            )
        {
            using (var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();

                var user = db.Users.First(x => x.UserName.Equals(username));
                var authorizedUser = db.Users.First(x => x.UserName.Equals(authorizeUsername));

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var authorizeInvite = new AuthorizeUserInviteDto()
                {
                    Title = "This is a title",
                    Message = "Hello you have been authorized",
                    UserId = authorizedUser.Id
                    
                };

                var jsonContent = new StringContent(JsonConvert.SerializeObject(authorizeInvite), Encoding.UTF8, "application/json");

                var requestUrl = $"api/Authorize/create_authorize_invite";

                var response = await client.PostAsync(requestUrl, jsonContent);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                bool isAdded = db.AuthorizeUserInvite.Any(x => x.Owner.Id.Equals(user.Id)
                        && x.AuthorizedUserId.Equals(authorizedUser.Id));
                Assert.True(isAdded);
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
