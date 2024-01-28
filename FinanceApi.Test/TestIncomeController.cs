using FinanceApi.Controllers;
using FinanceApi.Data;
using FinanceApi.Data.Dtos;
using FinanceApi.Models;
using FinanceApi.Repositories;
using FinanceApi.Repositories.Interfaces;
using FinanceApi.Services;
using FinanceApi.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;

namespace FinanceApi.Test
{
    public class TestIncomeController
    {


        public static IEnumerable<object[]> GetUsersIncomeValidInputsTestData()
        {
            yield return new object[] { new DateTime(2020, 1, 1), new DateTime(2024, 1, 1), "amount", null, "user123" };
            yield return new object[] { new DateTime(2021, 1, 1), new DateTime(2023, 1, 1), "title", "Desc", "user123" };
            yield return new object[] { null, null, null, "Desc", null };
            yield return new object[] { null, null, "amount", null, "user123" };
            yield return new object[] { null, null, "title", null, null };
        }

        [Theory]
        [MemberData(nameof(GetUsersIncomeValidInputsTestData))]
        public void GetUsersIncomes_ReturnsOk_When_UserExistsAndHasValidInputs(
            DateTime? from,
            DateTime? to,
            string? list_order_by,
            string? list_dir,
            string? optionalOwnerId
            )
        {
            // Arrange

            var categoryRepoMock = new Mock<ICategoryRepository>();
            var authServiceMock = new Mock<IAuthorizeService>();
            var userServiceMock = new Mock<IUserService>();
            var incomeRepoMock = new Mock<IIncomeRepository>();

            authServiceMock.Setup(x => x.ValidateUsers(It.IsAny<HttpContext>(),
                It.IsAny<string>(), It.IsAny<string>(), out It.Ref<int>.IsAny,
                out It.Ref<string>.IsAny)).Returns(true);

            var incomes = new List<Income>()
                {
                    new Income()
                        {
                            Id = 1,
                            Title = "a-income",
                            Description = "description",
                            Amount = 100,
                            Currency = "EUR",
                            Date = new DateTime(2021, 1, 1),
                            DocumentUrl = "URL",
                            User = new User(){ Id = "user1" },
                            IncomeCategories = null,
                        },
                    new Income()
                    {
                        Id = 2,
                        Title = "b-income",
                        Description = "another description",
                        Amount = 150,
                        Currency = "USD",
                        Date = new DateTime(2021, 2, 1),
                        DocumentUrl = "AnotherURL",
                        User = new User(){ Id = "user2"},
                        IncomeCategories = null,
                    },
                    new Income()
                    {
                        Id = 3,
                        Title = "c-income",
                        Description = "yet another description",
                        Amount = 200,
                        Currency = "GBP",
                        Date = new DateTime(2021, 3, 1),
                        DocumentUrl = "YetAnotherURL",
                        User = new User(){ Id = "user1" },
                        IncomeCategories = null,
                    },
                    new Income()
                    {
                        Id = 5,
                        Title = "e-income",
                        Description = "additional description",
                        Amount = 180,
                        Currency = "AUD",
                        Date = new DateTime(2021, 5, 1),
                        DocumentUrl = "AdditionalURL",
                        User = new User() { Id = "user2" },
                        IncomeCategories = null,
                    },
                    new Income()
                    {
                        Id = 6,
                        Title = "f-income",
                        Description = "final description",
                        Amount = 250,
                        Currency = "JPY",
                        Date = new DateTime(2021, 6, 1),
                        DocumentUrl = "FinalURL",
                        User = new User() { Id = "user1" },
                        IncomeCategories = null,
                    },
                    new Income()
                    {
                        Id = 4,
                        Title = "d-income",
                        Description = "more description",
                        Amount = 120,
                        Currency = "CAD",
                        Date = new DateTime(2021, 4, 1),
                        DocumentUrl = "MoreURL",
                        User = new User() { Id = "user2" },
                        IncomeCategories = null,
                    },
                };

            incomeRepoMock.Setup(x => x.GetAllOfUser(It.IsAny<string>()))
                .Returns(incomes.Where(i => i.User.Id.Equals(optionalOwnerId ?? "user1")).ToList());

            IIncomeService incomeService = new IncomeService(
                incomeRepoMock.Object,
                categoryRepoMock.Object);

            var incomeController = new IncomeController(
                incomeService,
                userServiceMock.Object,
                authServiceMock.Object);


            incomeController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.NameIdentifier, "user1")

                    }))
                }
            };

            // Act
            var result = incomeController.GetUsersIncomes(
                from, to, list_order_by, list_dir, optionalOwnerId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = (OkObjectResult)result;
            Assert.IsType<List<IncomeDto>>(okResult.Value);
        }


    }
}
