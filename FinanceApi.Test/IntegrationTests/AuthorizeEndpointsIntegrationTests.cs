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
        [Fact]
        public async Task AuthorizeUser_ReturnsUnauthorized_WhenUserIsNotLoggedIn()
        {
            var authorizeInvite = new AuthorizeUserInviteDto()
            {
                Title = "This is a title",
                Message = "Hello you have been authorized",
                UserId = "This is a random ID DOES NOT MATTER FOR THE TEST"

            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(authorizeInvite), Encoding.UTF8, "application/json");

            var requestUrl = $"api/Authorize/create_authorize_invite";
            var response = await client.PostAsync(requestUrl, jsonContent);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }


        [Fact]
        public async Task AcceptAuthorization_ReturnsOk_WhenUserIsLoggedInAndHadInvite()
        {
            using (var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();

                var user = db.Users.First(x => x.UserName.Equals("user4@example.com"));
                var incomeingInviteUser = db.Users.First(x => x.UserName.Equals("user1@example.com"));

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);


                var requestUrl = $"api/Authorize/accept_authorize_invite/{incomeingInviteUser.Id}";

                var response = await client.PostAsync(requestUrl, null);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                bool isAdded = db.AuthorizedUsers.Any(x => x.Owner.Id.Equals(incomeingInviteUser.Id)
                        && x.AuthorizedUserId.Equals(user.Id));
                Assert.True(isAdded);
            }
        }


        [Fact]
        public async Task AcceptAuthorization_ReturnsUnAuthorized_WhenUserIsNotLogged()
        {
            var randomId = "THIS VALUE DOES NOT MATTER";
            var requestUrl = $"api/Authorize/accept_authorize_invite/{randomId}";

            var response = await client.PostAsync(requestUrl, null);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory]
        [MemberData(nameof(TestData.GiveEditPermissionValidInputTestData), MemberType = typeof(TestData))]
        public async Task EditPermission_ReturnsOk_WhenUserIsAuthorized(
            string username,
            string authorizedUsername,
            bool canEdit
            )
        {
            using (var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();

                var user = db.Users.First(x => x.UserName.Equals(username));
                var authorizedUser = db.Users.First(x => x.UserName.Equals(authorizedUsername));

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);


                var requestUrl = $"api/Authorize/edit_permission/{authorizedUser.Id}?canEdit={canEdit}";

                var response = await client.PatchAsync(requestUrl, null);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                bool isAdded = db.AuthorizedUsers.Any(x => x.Owner.Id.Equals(user.Id) 
                    && x.AuthorizedUserId.Equals(authorizedUser.Id));
                Assert.True(isAdded);
            }
        }

        [Theory]
        [MemberData(nameof(TestData.GiveEditPermissionBadRequestTestData), MemberType = typeof(TestData))]
        public async Task EditPermission_ReturnsNotFound_WhenUserIsAuthorized(
            string username,
            string authorizedUsername,
            bool canEdit
            )
        {
            using (var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();

                var user = db.Users.First(x => x.UserName.Equals(username));
                var authorizedUser = db.Users.First(x => x.UserName.Equals(authorizedUsername));

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var requestUrl = $"api/Authorize/edit_permission/{authorizedUser.Id}?canEdit={canEdit}";

                var response = await client.PatchAsync(requestUrl, null);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

                bool isAdded = db.AuthorizedUsers.Any(x => x.Owner.Id.Equals(user.Id)
                    && x.AuthorizedUserId.Equals(authorizedUser.Id));
                Assert.False(isAdded);
            }
        }

        [Fact]
        public async Task EditPermission_ReturnsUnauthorized_WhenUserIsNotLoggedIn()
        {
            var authorizedUserId = "This value does not matter";
            bool canEdit = true;
            var requestUrl = $"api/Authorize/edit_permission/{authorizedUserId}?canEdit={canEdit}";

            var response = await client.PatchAsync(requestUrl, null);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }



        [Theory]
        [MemberData(nameof(TestData.DeleteAuthorizationValidTestData), MemberType = typeof(TestData))]
        public async Task DeleteAuthorization_ReturnsOk_WhenUserIsAuthorized(
            string username,
            string authorizedUsername
            )
        {
            using (var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();

                var user = db.Users.First(x => x.UserName.Equals(username));
                var authorizedUser = db.Users.First(x => x.UserName.Equals(authorizedUsername));

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var requestUrl = $"api/Authorize/delete_authorization/{authorizedUser.Id}";

                var response = await client.DeleteAsync(requestUrl);

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                bool isDeleted = !db.AuthorizedUsers.Any(x => x.Owner.Id.Equals(user.Id)
                    && x.AuthorizedUserId.Equals(authorizedUser.Id));
                Assert.True(isDeleted);
            }
        }

        [Fact]
        public async Task DeleteAuthorizationInvite_ReturnsOkWhenInviteIsSent()
        {
            using (var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();

                var user = db.Users.First(x => x.UserName.Equals("user1@example.com"));
                var authorizedUser = db.Users.First(x => x.UserName.Equals("user4@example.com"));

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var requestUrl = $"api/Authorize/delete_authorization_invite/{authorizedUser.Id}";

                var response = await client.DeleteAsync(requestUrl);

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                bool isDeleted = !db.AuthorizeUserInvite.Any(x => x.Owner.Id.Equals(user.Id)
                    && x.AuthorizedUserId.Equals(authorizedUser.Id));
                Assert.True(isDeleted);
            }
        }

        [Fact]
        public async Task DeclineAuthorizationInvite_ReturnsOk_WhenUserHasInvite()
        {
            using (var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();

                var user = db.Users.First(x => x.UserName.Equals("user4@example.com"));
                var invitor = db.Users.First(x => x.UserName.Equals("user1@example.com"));

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var requestUrl = $"api/Authorize/decline_authorization_invite/{invitor.Id}";

                var response = await client.DeleteAsync(requestUrl);

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                bool isDeleted = !db.AuthorizeUserInvite.Any(x => x.Owner.Id.Equals(invitor.Id)
                    && x.AuthorizedUserId.Equals(user.Id));
                Assert.True(isDeleted);
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
