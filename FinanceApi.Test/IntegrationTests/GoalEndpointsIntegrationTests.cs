﻿using FinanceApi.Data.Dtos;
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
    [Collection("GoalIntegrationTests")]
    public class GoalEndpointsIntegrationTests : IDisposable
    {
        private CustomWebApplicationFactory factory;
        private HttpClient client;
        public GoalEndpointsIntegrationTests()
        {
            factory = new CustomWebApplicationFactory();
            client = factory.CreateClient();
        }



        [Theory]
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

            List<GoalDto> responseData = JsonConvert.DeserializeObject<List<GoalDto>>(await response.Content.ReadAsStringAsync());


            var ownerId = optionalOwnerId ?? user.Id;

            List<GoalDto> correctResponse = null;
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
                        .Where(i => i.Date >= x.StartDate && i.Date <= DateTime.Now && i.User.Id.Equals(ownerId) && i.IncomeCategories
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

            Assert.Equivalent(correctResponse, responseData);

        }

        [Fact]
        public async Task GetGoal_ReturnsUnAuthorized_WhenUserIsNotLoggedIn()
        {

            var requestUrl = $"/api/Goal/current";

            var response = await client.GetAsync(requestUrl);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }


        [Theory]
        [MemberData(nameof(TestData.GetGoalsForbiddenTestData), MemberType = typeof(TestData))]
        public async Task GetGoal_ReturnsForbidden_WhenUserIsNotAuthorizedByOtherUser(
            string username,
            string optionalOwnerUsername
            )
        {
            using (var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var user = db.Users.First(x => x.UserName.Equals(username));
                string optionalOwnerId = db.Users.First(x => x.UserName.Equals(optionalOwnerUsername)).Id;

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                var requestUrl = $"/api/Goal/current?optionalOwnerId={optionalOwnerId}";

                var response = await client.GetAsync(requestUrl);

                Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
            }
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

        [Fact]
        public async Task CreateGoal_ReturnsUnAuthorized_WhenUserIsNotLoggedIn()
        {
            var goalManageDto = new GoalManageDto()
            {
                Title = "Title",
                Description = "Description",
                Currency = "usd",
                Amount = 1200,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddYears(3),
            };
            var requestUrl = $"/api/Goal/post";
            var jsonContent = new StringContent(JsonConvert.SerializeObject(goalManageDto), Encoding.UTF8, "application/json");

            var response = await client.PostAsync(requestUrl, jsonContent);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }


        [Theory]
        [MemberData(nameof(TestData.CreateGoalsForbiddenTestData), MemberType = typeof(TestData))]
        public async Task CreateGoal_ReturnsForbidden_WhenUserIsNotAuthorizedByOtherUser(
            string username,
            GoalManageDto goalManageDto,
            string optionalOwnerUsername
            )
        {
            using (var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var user = db.Users.First(x => x.UserName.Equals(username));
                string optionalOwnerId = db.Users.First(x => x.UserName.Equals(optionalOwnerUsername)).Id;

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                var requestUrl = $"/api/Goal/post?optionalOwnerId={optionalOwnerId}";
                var jsonContent = new StringContent(JsonConvert.SerializeObject(goalManageDto), Encoding.UTF8, "application/json");


                var response = await client.PostAsync(requestUrl, jsonContent);

                Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
            }
        }

        [Theory]
        [MemberData(nameof(TestData.UpdateGoalValidInputTestData), MemberType = typeof(TestData))]
        public async Task UpdateGoal_ReturnsOk_WhenGoalExistsAndInputIsValid(
            string username,
            GoalManageDto goalManageDto,
            string? optionalOwnerUsername
            )
        {
            using (var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var user = db.Users.First(x => x.UserName.Equals(username));

                var goalToBeUpdated = db.Goals.AsNoTracking().First(x => x.Id == goalManageDto.Id);

                string? optionalOwnerId = null;
                if (optionalOwnerUsername != null)
                {
                    optionalOwnerId = db.Users.FirstOrDefault(x => x.UserName.Equals(optionalOwnerUsername)).Id;
                }

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var jsonGoalManageDto = new StringContent(JsonConvert.SerializeObject(goalManageDto), Encoding.UTF8, "application/json");

                var requestUrl = optionalOwnerId == null
                    ? $"api/Goal/put"
                    : $"api/Goal/put?optionalOwnerId={optionalOwnerId}";

                try
                {
                    var response = await client.PutAsync(requestUrl, jsonGoalManageDto);
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                }
                finally
                {
                    var isUpdated = db.Goals.AsNoTracking().ToList()
                        .Select(Map.ToGoalManageDto)
                        .Any(x =>
                        {
                            bool isSame =
                                x.Id == goalManageDto.Id &&
                                x.Title.ToLower().Equals(goalManageDto.Title.ToLower()) &&
                                x.Description.ToLower().Equals(goalManageDto.Description.ToLower()) &&
                                x.Currency.ToLower().Equals(goalManageDto.Currency.ToLower()) &&
                                x.StartDate.Equals(goalManageDto.StartDate) &&
                                x.StartDate.Equals(goalManageDto.StartDate) &&
                                x.EndDate.Equals(goalManageDto.EndDate) &&
                                x.Amount == goalManageDto.Amount;

                            return isSame;
                        });

                    Assert.True(isUpdated);

                    if (isUpdated)
                    {
                        db.Goals.Update(goalToBeUpdated);
                        db.SaveChanges();
                    }

                }
            }
        }


        [Theory]
        [MemberData(nameof(TestData.UpdateGoalBadRequestInputTestData), MemberType = typeof(TestData))]
        public async Task UpdateGoal_ReturnsBadRequest_WhenGoalExistsAndInputIsInvalid(
            string username,
            GoalManageDto goalManageDto,
            string? optionalOwnerUsername
            )
        {

            using (var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var user = db.Users.First(x => x.UserName.Equals(username));

                var goalToBeUpdated = db.Goals.AsNoTracking().First(x => x.Id == goalManageDto.Id);

                string? optionalOwnerId = null;
                if (optionalOwnerUsername != null)
                {
                    optionalOwnerId = db.Users.FirstOrDefault(x => x.UserName.Equals(optionalOwnerUsername)).Id;
                }

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var jsonGoalManageDto = new StringContent(JsonConvert.SerializeObject(goalManageDto), Encoding.UTF8, "application/json");

                var requestUrl = optionalOwnerId == null
                    ? $"api/Goal/put"
                    : $"api/Goal/put?optionalOwnerId={optionalOwnerId}";

                try
                {
                    var response = await client.PutAsync(requestUrl, jsonGoalManageDto);
                    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                }
                finally
                {
                    var isUpdated = db.Goals.AsNoTracking().ToList()
                        .Select(Map.ToGoalManageDto)
                        .Any(x =>
                        {
                            bool isSame =
                                x.Id == goalManageDto.Id &&
                                x.Title.ToLower().Equals(goalManageDto.Title.ToLower()) &&
                                x.Description.ToLower().Equals(goalManageDto.Description.ToLower()) &&
                                x.Currency.ToLower().Equals(goalManageDto.Currency.ToLower()) &&
                                x.StartDate.Equals(goalManageDto.StartDate) &&
                                x.StartDate.Equals(goalManageDto.StartDate) &&
                                x.EndDate.Equals(goalManageDto.EndDate) &&
                                x.Amount == goalManageDto.Amount;

                            return isSame;
                        });

                    Assert.False(isUpdated);

                    if (isUpdated)
                    {
                        db.Goals.Update(goalToBeUpdated);
                        db.SaveChanges();
                    }

                }
            }
        }


        [Theory]
        [MemberData(nameof(TestData.UpdateGoalNotFoundRequestInputTestData), MemberType = typeof(TestData))]
        public async Task UpdateGoal_ReturnsNotFound_WhenGoalDoesNotExistOrIsNotUsersGoal(
            string username,
            GoalManageDto goalManageDto,
            string? optionalOwnerUsername
            )
        {
            using (var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var user = db.Users.First(x => x.UserName.Equals(username));

                var goalToBeUpdatedIfExists = db.Goals.AsNoTracking().FirstOrDefault(x => x.Id == goalManageDto.Id);

                string? optionalOwnerId = null;
                if (optionalOwnerUsername != null)
                {
                    optionalOwnerId = db.Users.FirstOrDefault(x => x.UserName.Equals(optionalOwnerUsername)).Id;
                }

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var jsonGoalManageDto = new StringContent(JsonConvert.SerializeObject(goalManageDto), Encoding.UTF8, "application/json");

                var requestUrl = optionalOwnerId == null
                    ? $"api/Goal/put"
                    : $"api/Goal/put?optionalOwnerId={optionalOwnerId}";

                try
                {
                    var response = await client.PutAsync(requestUrl, jsonGoalManageDto);
                    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
                }
                finally
                {
                    if (goalToBeUpdatedIfExists != null)
                    {
                        var isUpdated = db.Goals.AsNoTracking().ToList()
                            .Select(Map.ToGoalManageDto)
                            .Any(x =>
                            {
                                bool isSame =
                                    x.Id == goalManageDto.Id &&
                                    x.Title.ToLower().Equals(goalManageDto.Title.ToLower()) &&
                                    x.Description.ToLower().Equals(goalManageDto.Description.ToLower()) &&
                                    x.Currency.ToLower().Equals(goalManageDto.Currency.ToLower()) &&
                                    x.StartDate.Equals(goalManageDto.StartDate) &&
                                    x.StartDate.Equals(goalManageDto.StartDate) &&
                                    x.EndDate.Equals(goalManageDto.EndDate) &&
                                    x.Amount == goalManageDto.Amount;

                                return isSame;
                            });

                        Assert.False(isUpdated);

                        if (isUpdated)
                        {
                            db.Goals.Update(goalToBeUpdatedIfExists);
                            db.SaveChanges();
                        }
                    }
                }
            }
        }

        [Fact]
        public async Task UpdateGoal_ReturnsUnAuthorized_WhenUserIsNotLoggedIn()
        {
            var goalManageDto = new GoalManageDto()
            {
                Id = 1,
                Title = "Title",
                Description = "Description",
                Currency = "usd",
                Amount = 1200,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddYears(3),
            };
            var requestUrl = $"/api/Goal/post";
            var jsonContent = new StringContent(JsonConvert.SerializeObject(goalManageDto), Encoding.UTF8, "application/json");

            var response = await client.PostAsync(requestUrl, jsonContent);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }


        [Theory]
        [MemberData(nameof(TestData.UpdateGoalsForbiddenTestData), MemberType = typeof(TestData))]
        public async Task UpdateGoal_ReturnsForbidden_WhenUserIsNotAuthorizedByOtherUser(
            string username,
            GoalManageDto goalManageDto,
            string optionalOwnerUsername
            )
        {
            using (var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var user = db.Users.First(x => x.UserName.Equals(username));
                string optionalOwnerId = db.Users.First(x => x.UserName.Equals(optionalOwnerUsername)).Id;

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                var requestUrl = $"/api/Goal/put?optionalOwnerId={optionalOwnerId}";
                var jsonContent = new StringContent(JsonConvert.SerializeObject(goalManageDto), Encoding.UTF8, "application/json");

                var response = await client.PutAsync(requestUrl, jsonContent);

                Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
            }
        }

        [Theory]
        [MemberData(nameof(TestData.DeleteGoalValidTestData), MemberType = typeof(TestData))]
        public async Task DeleteGoal_ReturnsOk_WhenGoalExists(
            string username,
            int goalId,
            string? optionalOwnerUsername
            )
        {
            using (var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var user = db.Users.First(x => x.UserName.Equals(username));

                var goalToBeDeleted = db.Goals.AsNoTracking().First(x => x.Id == goalId);

                string? optionalOwnerId = null;
                if (optionalOwnerUsername != null)
                {
                    optionalOwnerId = db.Users.FirstOrDefault(x => x.UserName.Equals(optionalOwnerUsername)).Id;
                }

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var requestUrl = optionalOwnerId == null
                    ? $"api/Goal/delete/{goalId}"
                    : $"api/Goal/delete/{goalId}?optionalOwnerId={optionalOwnerId}";

                try
                {

                    var response = await client.DeleteAsync(requestUrl);
                    db.SaveChanges();
                    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
                }
                finally
                {
                    bool isDeleted = !db.Goals.Any(x => x.Equals(goalToBeDeleted));
                    Assert.True(isDeleted);

                    if (isDeleted)
                    {
                        goalToBeDeleted.Id = goalId;
                        goalToBeDeleted.User = user;
                        db.Goals.Add(goalToBeDeleted);
                        db.SaveChanges();
                    }
                }

            }
        }

        [Theory]
        [MemberData(nameof(TestData.DeleteGoalNotFoundInputTestData), MemberType = typeof(TestData))]
        public async Task DeleteGoal_ReturnsNotFound_WhenUserDoesNotHaveAGoalWithInputId(
            string username,
            int goalId,
            string? optionalOwnerUsername
            )
        {
            using (var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var user = db.Users.First(x => x.UserName.Equals(username));

                var goalToBeDeleted = db.Goals.AsNoTracking().FirstOrDefault(x => x.Id == goalId);

                string? optionalOwnerId = null;
                if (optionalOwnerUsername != null)
                {
                    optionalOwnerId = db.Users.FirstOrDefault(x => x.UserName.Equals(optionalOwnerUsername)).Id;
                }

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var requestUrl = optionalOwnerId == null
                    ? $"api/Goal/delete/{goalId}"
                    : $"api/Goal/delete/{goalId}?optionalOwnerId={optionalOwnerId}";

                try
                {

                    var response = await client.DeleteAsync(requestUrl);

                    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
                }
                finally
                {
                    bool isDeleted = !db.Goals.Any(x => x.Equals(goalToBeDeleted));
                    Assert.False(isDeleted);

                    if (isDeleted)
                    {
                        Assert.Equal(user, goalToBeDeleted.User);

                        db.Goals.Add(goalToBeDeleted);
                        db.SaveChanges();
                    }
                }

            }
        }

        [Fact]
        public async Task DeleteGoal_ReturnsUnAuthorized_WhenUserIsNotLoggedIn()
        {
            var requestUrl = $"/api/Goal/delete/1";

            var response = await client.DeleteAsync(requestUrl);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }


        [Theory]
        [MemberData(nameof(TestData.DeleteGoalsForbiddenTestData), MemberType = typeof(TestData))]
        public async Task DeleteGoal_ReturnsForbidden_WhenUserIsNotAuthorizedByOtherUser(
            string username,
            int goalId,
            string optionalOwnerUsername
            )
        {
            using (var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var user = db.Users.First(x => x.UserName.Equals(username));
                string optionalOwnerId = db.Users.First(x => x.UserName.Equals(optionalOwnerUsername)).Id;

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                var requestUrl = $"/api/Goal/delete/{goalId}?optionalOwnerId={optionalOwnerId}";

                var response = await client.DeleteAsync(requestUrl);

                Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
            }
        }

        [Theory]
        [MemberData(nameof(TestData.AddCategoryToGoalValidInputTestData), MemberType = typeof(TestData))]
        public async Task AddCategory_ReturnsOk_WhenInputIsValidAndDoesNotExist(
            string username,
            int goalId,
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
                    ? $"api/Goal/associate_categories/{goalId}"
                    : $"api/Goal/associate_categories/{goalId}?optionalOwnerId={optionalOwnerId}";


                try
                {
                    var response = await client.PostAsync(requestUrl, jsonCategoryIds);

                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                }
                finally
                {
                    int amountAdded = db.GoalCategories
                        .Where(x => x.GoalId == goalId && categoryIds.Contains(x.CategoryId)).Count();
                    bool allAdded = categoryIds.Count() == amountAdded;
                    Assert.True(allAdded);

                    if (amountAdded > 0)
                    {
                        for (int i = 0; i < categoryIds.Count; ++i)
                        {
                            var addedGoalCategory = db.GoalCategories.FirstOrDefault(x => x.GoalId == goalId
                            && x.CategoryId == categoryIds[i]);
                            if (addedGoalCategory != null)
                            {
                                db.Remove(addedGoalCategory);
                                db.SaveChanges();
                            }
                        }
                    }

                }



            }
        }

        [Theory]
        [MemberData(nameof(TestData.AddCategoryToGoalBadRequestInputTestData), MemberType = typeof(TestData))]
        public async Task AddCategory_ReturnsBadRequest_WhenInputIsWrongOrGoalCategoryAlreadyExists(
            string username,
            int goalId,
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
                    ? $"api/Goal/associate_categories/{goalId}"
                    : $"api/Goal/associate_categories/{goalId}?optionalOwnerId={optionalOwnerId}";



                var response = await client.PostAsync(requestUrl, jsonCategoryIds);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            }
        }

        [Theory]
        [MemberData(nameof(TestData.AddCategoryToGoalNotFoundInputTestData), MemberType = typeof(TestData))]
        public async Task AddCategory_ReturnsNotFound_WhenInputIsWrongOrGoalCategoryAlreadyExists(
            string username,
            int goalId,
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
                    ? $"api/Goal/associate_categories/{goalId}"
                    : $"api/Goal/associate_categories/{goalId}?optionalOwnerId={optionalOwnerId}";



                var response = await client.PostAsync(requestUrl, jsonCategoryIds);

                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            }
        }

        [Fact]
        public async Task AddCategoryToGoal_ReturnsUnAuthorized_WhenUserIsNotLoggedIn()
        {
            ICollection<int> categoryIds = new List<int>() { 2,3,4 };
            var requestUrl = $"/api/Goal/associate_categories/1";
            var jsonContent = new StringContent(JsonConvert.SerializeObject(categoryIds), Encoding.UTF8, "application/json");

            var response = await client.PostAsync(requestUrl, jsonContent);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }


        [Theory]
        [MemberData(nameof(TestData.AddCategoryToGoalsForbiddenTestData), MemberType = typeof(TestData))]
        public async Task AddCategoryToGoal_ReturnsForbidden_WhenUserIsNotAuthorizedByOtherUser(
            string username,
            int goalId,
            ICollection<int> categoryIds,
            string optionalOwnerUsername
            )
        {
            using (var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var user = db.Users.First(x => x.UserName.Equals(username));
                string optionalOwnerId = db.Users.First(x => x.UserName.Equals(optionalOwnerUsername)).Id;

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                var requestUrl = $"/api/Goal/associate_categories/{goalId}?optionalOwnerId={optionalOwnerId}";
                var jsonContent = new StringContent(JsonConvert.SerializeObject(categoryIds), Encoding.UTF8, "application/json");

                var response = await client.PostAsync(requestUrl, jsonContent);

                Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
            }
        }

        [Theory]
        [MemberData(nameof(TestData.RemoveCategoriesFromGoalValidInputTestData), MemberType = typeof(TestData))]
        public async Task RemoveCategories_ReturnsOk_WhenGoalCategoryExistsAndIsUsers(
            string username,
            int goalId,
            int categoryId,
            string? optionalOwnerUsername
            )
        {
            using (var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();

                var user = db.Users.First(x => x.UserName.Equals(username));

                var goalCategoryToBeDeleted =
                    db.GoalCategories.AsNoTracking().First(x => x.GoalId == goalId && x.CategoryId == categoryId);

                string? optionalOwnerId = null;
                if (optionalOwnerUsername != null)
                {
                    optionalOwnerId = db.Users.FirstOrDefault(x => x.UserName.Equals(optionalOwnerUsername)).Id;
                }


                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var requestUrl = optionalOwnerId == null
                    ? $"api/Goal/remove_category/{goalId}/{categoryId}"
                    : $"api/Goal/remove_category/{goalId}/{categoryId}?optionalOwnerId={optionalOwnerId}";


                try
                {
                    var response = await client.DeleteAsync(requestUrl);

                    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
                }
                finally
                {
                    bool isDeleted = !db.GoalCategories.Any(x => x.Equals(goalCategoryToBeDeleted));
                    Assert.True(isDeleted);
                    if (isDeleted)
                    {
                        db.GoalCategories.Add(goalCategoryToBeDeleted);
                        db.SaveChanges();
                    }
                }
            }
        }


        [Theory]
        [MemberData(nameof(TestData.RemoveCategoriesFromGoalNotFoundInputsTestData), MemberType = typeof(TestData))]
        public async Task RemoveCategories_ReturnsNotFound_WhenGoalOrCategoryDoesNotExist(
            string username,
            int goalId,
            int categoryId,
            string? optionalOwnerUsername
            )
        {
            using (var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();

                var user = db.Users.First(x => x.UserName.Equals(username));

                var goalCategoryToBeDeleted =
                    db.GoalCategories.AsNoTracking().FirstOrDefault(x => x.GoalId == goalId && x.CategoryId == categoryId);

                string? optionalOwnerId = null;
                if (optionalOwnerUsername != null)
                {
                    optionalOwnerId = db.Users.FirstOrDefault(x => x.UserName.Equals(optionalOwnerUsername)).Id;
                }


                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var requestUrl = optionalOwnerId == null
                    ? $"api/Goal/remove_category/{goalId}/{categoryId}"
                    : $"api/Goal/remove_category/{goalId}/{categoryId}?optionalOwnerId={optionalOwnerId}";


                try
                {
                    var response = await client.DeleteAsync(requestUrl);

                    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
                }
                finally
                {
                    if (goalCategoryToBeDeleted != null)
                    {

                        bool isDeleted = !db.GoalCategories.Any(x => x.Equals(goalCategoryToBeDeleted));
                        Assert.True(isDeleted);
                        if (isDeleted)
                        {
                            db.GoalCategories.Add(goalCategoryToBeDeleted);
                            db.SaveChanges();
                        }
                    }
                }
            }
        }


        [Theory]
        [MemberData(nameof(TestData.RemoveCategoriesFromGoalBadRequestInputsTestData), MemberType = typeof(TestData))]
        public async Task RemoveCategories_ReturnsBadRequest_WhenGoalDoesNotHaveACategoryOrACategpry(
            string username,
            int goalId,
            int categoryId,
            string? optionalOwnerUsername
        )
        {
            using (var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();

                var user = db.Users.First(x => x.UserName.Equals(username));

                var goalCategoryToBeDeleted =
                    db.GoalCategories.AsNoTracking().FirstOrDefault(x => x.GoalId == goalId && x.CategoryId == categoryId);

                string? optionalOwnerId = null;
                if (optionalOwnerUsername != null)
                {
                    optionalOwnerId = db.Users.FirstOrDefault(x => x.UserName.Equals(optionalOwnerUsername)).Id;
                }


                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var requestUrl = optionalOwnerId == null
                    ? $"api/Goal/remove_category/{goalId}/{categoryId}"
                    : $"api/Goal/remove_category/{goalId}/{categoryId}?optionalOwnerId={optionalOwnerId}";


                try
                {
                    var response = await client.DeleteAsync(requestUrl);

                    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                }
                finally
                {
                    if (goalCategoryToBeDeleted != null)
                    {

                        bool isDeleted = !db.GoalCategories.Any(x => x.Equals(goalCategoryToBeDeleted));
                        Assert.True(isDeleted);
                        if (isDeleted)
                        {
                            db.GoalCategories.Add(goalCategoryToBeDeleted);
                            db.SaveChanges();
                        }
                    }
                }
            }
        }

        [Fact]
        public async Task RemoveCategoryFromGoal_ReturnsUnAuthorized_WhenUserIsNotLoggedIn()
        {
            ICollection<int> categoryIds = new List<int>() { 2, 3, 4 };
            var requestUrl = $"/api/Goal/associate_categories/1";
            var jsonContent = new StringContent(JsonConvert.SerializeObject(categoryIds), Encoding.UTF8, "application/json");

            var response = await client.PostAsync(requestUrl, jsonContent);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }


        [Theory]
        [MemberData(nameof(TestData.RemoveCategoryFromGoalForbiddenTestData), MemberType = typeof(TestData))]
        public async Task RemoveCategoryFromGoal_ReturnsForbidden_WhenUserIsNotAuthorizedByOtherUser(
            string username,
            int goalId,
            int categoryId,
            string optionalOwnerUsername
            )
        {
            using (var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var user = db.Users.First(x => x.UserName.Equals(username));
                string optionalOwnerId = db.Users.First(x => x.UserName.Equals(optionalOwnerUsername)).Id;

                var authToken = await GetAuthenticationTokenAsync(user.Email, "Password!2");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                var requestUrl = $"/api/Goal/remove_category/{goalId}/{categoryId}?optionalOwnerId={optionalOwnerId}";

                var response = await client.DeleteAsync(requestUrl);

                Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
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
