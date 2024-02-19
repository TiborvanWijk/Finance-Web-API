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

namespace FinanceApi.Test.IntegrationTests
{
    public class GoalEndpointsIntegrationTests : IDisposable
    {
        private CustomWebApplicationFactory factory;
        private HttpClient client;
        public GoalEndpointsIntegrationTests()
        {
            factory = new CustomWebApplicationFactory();
            client = factory.CreateClient();
        }



        //[Theory]
        [Theory(Skip = "Progress of goalDto is not calculated correctly.")]
        [MemberData(nameof(TestData.GetGoalValidInputTestData), MemberType = typeof(TestData))]
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
        [MemberData(nameof(TestData.CreateGoalValidInputTestData), MemberType = typeof(TestData))]
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
                        string ownerId = optionalOwnerId ?? user.Id;

                        db.Goals.Remove(db.Goals.First(g => g.User.Id.Equals(ownerId) 
                        && g.Title.Equals(goalManageDto.Title)));
                        db.SaveChanges();
                    }
                }

            }
        }


        [Theory]
        [MemberData(nameof(TestData.CreateGoalInvalidtestTestData), MemberType = typeof(TestData))]
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
                        string ownerId = optionalOwnerId ?? user.Id;

                        db.Goals.Remove(db.Goals.First(g => g.User.Id.Equals(ownerId) 
                        && g.Title.Equals(goalManageDto.Title)));
                        db.SaveChanges();
                    }
                }

            }
        }

        [Theory(Skip = "Not fully coded up.")]
        //[Theory]
        [MemberData(nameof(TestData.AddCategoryToGoalValidInputTestData), MemberType = typeof(TestData))]
        public async Task AddCategory_ReturnsOk_WhenInputIsValidAndDoesNotExist(
            string username,
            int goalId,
            ICollection<int> categoryIds,
            string? optionalOwnerUsername
            )
        {
            using(var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();

                var user = db.Users.First(x => x.UserName.Equals(username));
                var optionalOwnerId = db.Users.FirstOrDefault(x => x.UserName.Equals(optionalOwnerUsername));

                var jsonCategoryIds = new StringContent(JsonConvert.SerializeObject(categoryIds), Encoding.UTF8, "application/json");

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var requestUrl = optionalOwnerId == null
                    ? $"api/Goal/associate_categories/{goalId}"
                    : $"api/Goal/associate_categories/{goalId}?optionalOwnerId={optionalOwnerId}";


                try
                {
                    var response = await client.PostAsync(requestUrl, jsonCategoryIds);

                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                }
                finally
                {
                    bool isAdded = true;

                    for(int i = 0; i < categoryIds.Count; ++i)
                    {
                        
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
