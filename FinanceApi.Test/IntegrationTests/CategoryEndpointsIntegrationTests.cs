﻿using Newtonsoft.Json.Linq;
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
