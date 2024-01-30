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
using System.Runtime.Serialization;
using System.Security.Claims;
using System.Security.Cryptography;

namespace FinanceApi.Test
{
    public class TestIncomeController
    {
        private readonly Mock<ICategoryRepository> categoryRepoMock = new Mock<ICategoryRepository>();
        private readonly Mock<IAuthorizeService> authServiceMock = new Mock<IAuthorizeService>();
        private readonly Mock<IIncomeRepository> incomeRepoMock = new Mock<IIncomeRepository>();
        private readonly Mock<IUserService> userServiceMock = new Mock<IUserService>();
        private readonly Mock<IAuthorizeRepository> authorizeRepoMock = new Mock<IAuthorizeRepository>();
        private readonly Mock<IAuthorizationInviteRepository> auhtorizationInviteRepoMock = new Mock<IAuthorizationInviteRepository>();
        private readonly Mock<IUserRepository> userRepoMock = new Mock<IUserRepository>();
        public TestIncomeController()
        {

        }


        public static IEnumerable<object[]> GetUsersIncomeValidInputsTestData()
        {
            yield return new object[] { new DateTime(2020, 1, 1), new DateTime(2024, 1, 1), "amount", null, "user123" };
            yield return new object[] { new DateTime(2021, 1, 1), new DateTime(2023, 1, 1), "title", "desc", "user123" };
            yield return new object[] { null, null, null, "desc", null };
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
                        Id = 4,
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
                        Id = 5,
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
                        Id = 6,
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
            var okResult = ((OkObjectResult)result).Value;
            Assert.IsType<List<IncomeDto>>(okResult);

            var incomeDtos = (List<IncomeDto>)okResult;


            var orderedIncomeList = new List<IncomeDto>(incomeDtos);
            if (list_order_by != null || list_dir != null)
            {
                list_order_by = list_order_by ?? "DEFAULT (NO ORDERING)";
                switch (list_order_by.ToLower())
                {
                    case "title":
                        orderedIncomeList = (list_dir != null && list_dir.Equals("desc"))
                            ? orderedIncomeList.OrderByDescending(dto => dto.Title).ToList()
                            : orderedIncomeList.OrderBy(dto => dto.Title).ToList();
                        break;
                    case "amount":
                        orderedIncomeList = (list_dir != null && list_dir.Equals("desc"))
                            ? orderedIncomeList.OrderByDescending(dto => dto.Amount).ToList()
                            : orderedIncomeList.OrderBy(dto => dto.Amount).ToList();
                        break;
                    default:
                        orderedIncomeList = (list_dir != null && list_dir.Equals("desc")) ?
                            orderedIncomeList.OrderByDescending(dto => dto.Date).ToList()
                            : orderedIncomeList.OrderBy(idto => idto.Date).ToList();
                        break;
                }
            }

            Assert.Equal(incomeDtos, orderedIncomeList);
            ResetAllSetups();
        }

        public static IEnumerable<object[]> GetUsersIncomeInvalidInputsTestData()
        {
            yield return new object[] { new DateTime(2020, 1, 1), null, null, null, null };
            yield return new object[] { null, new DateTime(2020, 1, 1), null, null, null };
        }


        [Theory]
        [MemberData(nameof(GetUsersIncomeInvalidInputsTestData))]
        public void GetUsersIncomes_ReturnsBadRequest_WhenUserExistsAndHasInvalidInput(
            DateTime? from,
            DateTime? to,
            string? list_order_by,
            string? list_dir,
            string? optionalOwnerId
            )
        {

            // Arrange

            authServiceMock.Setup(x => x.ValidateUsers(
                It.IsAny<HttpContext>(), It.IsAny<string>(),
                It.IsAny<string?>(), out It.Ref<int>.IsAny, out It.Ref<string>.IsAny
                )).Returns(true);
            incomeRepoMock.Setup(x => x.GetAllOfUser(It.IsAny<string>())).Returns(
                new List<Income>()
                );

            var incomeService = new IncomeService(incomeRepoMock.Object, categoryRepoMock.Object);

            var incomeController = new IncomeController(incomeService, userServiceMock.Object,
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


            var result = incomeController.GetUsersIncomes(from,
                to, list_order_by, list_dir, optionalOwnerId);


            // Assert

            Assert.IsType<BadRequestObjectResult>(result);

            var badRequestResult = (BadRequestObjectResult)result;

            ResetAllSetups();
        }

        [Fact]
        public void GetUsersIncomes_ReturnsUnauthorizedObjectResult_WhenCurrentUserIsNotFound()
        {
            // Arrange

            userRepoMock.Setup(x => x.ExistsById(It.IsAny<string>())).Returns(false);

            var incomeService = new IncomeService(incomeRepoMock.Object, categoryRepoMock.Object);
            var authservice = new AuthorizeService(authorizeRepoMock.Object, auhtorizationInviteRepoMock.Object, userRepoMock.Object);
            var incomeController = new IncomeController(incomeService, userServiceMock.Object, authservice);

            incomeController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]{
                        new Claim(ClaimTypes.NameIdentifier, "user1")
                    }))
                }
            };

            // Act

            var result = incomeController.GetUsersIncomes(null, null, null, null, null);

            // Assert

            Assert.IsType<UnauthorizedObjectResult>(result);
            var test = (UnauthorizedObjectResult)result;
            Assert.Equal(401, test.StatusCode);
            ResetAllSetups();
        }

        [Fact]
        public void GetUsersIncomes_ReturnsNotFoundObjectResult_WhenOptionalOwnerDoesNotExist()
        {
            // Arrange
            var incomeService = new IncomeService(incomeRepoMock.Object, categoryRepoMock.Object);
            var authservice = new AuthorizeService(authorizeRepoMock.Object, auhtorizationInviteRepoMock.Object, userRepoMock.Object);
            var incomeController = new IncomeController(incomeService, userServiceMock.Object, authservice);

            userRepoMock.Setup(x => x.ExistsById(It.Is<string>(s => s.Equals("CURRENT USER"))))
                .Returns(true);

            incomeController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]{
                        new Claim(ClaimTypes.NameIdentifier, "CURRENT USER")
                    }))
                }
            };

            // Act
            var result = incomeController.GetUsersIncomes(null, null, null, null, "THIS USER DOES NOT EXIST");

            // Assert

            Assert.IsType<NotFoundObjectResult>(result);
            var statusCode = ((NotFoundObjectResult)result).StatusCode;

            Assert.Equal(404, statusCode);

            ResetAllSetups();
        }

        [Fact]
        public void GetUsersIncomes_ReturnsForbiddenResult_WhenUsersExistButCurrentUserIsForbiddenToGet()
        {

            // Arrange

            var incomeService = new IncomeService(incomeRepoMock.Object, categoryRepoMock.Object);
            var authservice = new AuthorizeService(authorizeRepoMock.Object, auhtorizationInviteRepoMock.Object, userRepoMock.Object);
            var incomeController = new IncomeController(incomeService, userServiceMock.Object, authservice);


            authorizeRepoMock.Setup(x => x.IsAuthorized(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(false);


            userRepoMock.Setup(x => x.ExistsById(It.IsAny<string>()))
                .Returns(true);

            incomeController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "CURRENT USER")
                    }))
                }
            };

            // Act

            var result = incomeController.GetUsersIncomes(null, null, null, null, "OWNER DID NOT AUTHORIZE CURRENT USER");
    
            // Assert
            
            Assert.IsType<ForbidResult>(result);
            ResetAllSetups();
        }

        public static IEnumerable<object[]> IncomeDtoValidInputsTestData()
        {
            yield return new object[] {
                new IncomeDto() {
                    Id = 1, Title = "Title1", Description = "Desctiption1", Amount = 18, Currency = "EUR", Date = DateTime.Now, DocumentUrl = "URL"
                }, null
                };
        } 

        [Theory]
        [MemberData(nameof(IncomeDtoValidInputsTestData))]
        public void CreateIncome_ReturnsOkResultObject_WhenUserExistsAndIsValidInput(
            IncomeDto incomeDto,
            string? optionalOwnerId
            )
        {
            // Arrange
            authServiceMock.Setup(x => x.ValidateUsers(It.IsAny<HttpContext>(),
                It.IsAny<string>(), It.IsAny<string>(), out It.Ref<int>.IsAny, out It.Ref<string>.IsAny))
                .Returns(true);
            userServiceMock.Setup(x => x.GetById(It.IsAny<string>(), It.IsAny<bool>())).Returns(new User());
            incomeRepoMock.Setup(x => x.Create(It.IsAny<Income>())).Returns(true);

            var incomeService = new IncomeService(incomeRepoMock.Object, categoryRepoMock.Object);

            var incomeController = new IncomeController(incomeService, userServiceMock.Object, authServiceMock.Object);

            incomeController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                        new Claim(ClaimTypes.NameIdentifier, "user1")
                    }))
                }
            };

            // Act
            var result = incomeController.CreateIncome(incomeDto, optionalOwnerId);
            
            // Assert

            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
            ResetAllSetups();
        }

        public static IEnumerable<object[]> IncomeDtoInvalidInputTestData()
        {
            yield return new object[] { new IncomeDto() { Id = 1, Title = "Title-1", Description = "Description-1", Amount = decimal.MinValue, Currency = "USD", Date = DateTime.Now, DocumentUrl = "URL-1" }, null };
            yield return new object[] { new IncomeDto() { Id = 2, Title = "Title-2", Description = "Description-2", Amount = decimal.MaxValue, Currency = "USD", Date = DateTime.Now, DocumentUrl = "URL-2" }, null };
            yield return new object[] { new IncomeDto() { Id = 3, Title = "Title-3", Description = "Description-3", Amount = -1, Currency = "INVALID", Date = DateTime.Now, DocumentUrl = "URL-3" }, null };
            yield return new object[] { new IncomeDto() { Id = 4, Title = "Title-4", Description = "Description-4", Amount = 9, Currency = "INVALID", Date = DateTime.Now, DocumentUrl = "URL-4" }, null };
        }

        [Theory]
        [MemberData(nameof(IncomeDtoInvalidInputTestData))]
        public void CreateIncome_ReturnsBadRequestObjectResult_WhenInputIsInvalid(
            IncomeDto incomeDto,
            string? optionalOwnerId
            )
        {
            // Arrange
            authServiceMock.Setup(x => x.ValidateUsers(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<string>(),
                out It.Ref<int>.IsAny, out It.Ref<string>.IsAny)).Returns(true);

            var incomeService = new IncomeService(incomeRepoMock.Object, categoryRepoMock.Object);

            var incomeController = new IncomeController(incomeService, userServiceMock.Object, authServiceMock.Object);

            incomeController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "CURRENT USER")
                    }))
                }
            };

            // Act

            var result = incomeController.CreateIncome(incomeDto, optionalOwnerId);

            // Assert

            Assert.IsType<BadRequestObjectResult>(result);
            ResetAllSetups();
        }



        [Theory]
        [MemberData(nameof(IncomeDtoValidInputsTestData))]
        public void CreateIncome_ReturnsObjectResult500_WhenCreatingFails(
            IncomeDto incomeDto,
            string? optionalOwnerId
            )
        {
            // Arrange 
            authServiceMock.Setup(x => x.ValidateUsers(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<string>(),
                out It.Ref<int>.IsAny, out It.Ref<string>.IsAny)).Returns(true);

            userServiceMock.Setup(x => x.GetById(It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new User());

            incomeRepoMock.Setup(x => x.Create(It.IsAny<Income>()))
                .Returns(false);

            var incomeService = new IncomeService(incomeRepoMock.Object, categoryRepoMock.Object);

            var incomeController = new IncomeController(incomeService, userServiceMock.Object, authServiceMock.Object);

            incomeController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "CURRENT USER")
                    }))
                }
            };

            // Act
            var result = incomeController.CreateIncome(incomeDto, null);

            // Assert

            Assert.IsType<ObjectResult>(result);
            var objectResult = (ObjectResult)result;
            Assert.Equal(500, objectResult.StatusCode);
            ResetAllSetups();            
        }

        [Theory]
        [MemberData(nameof(IncomeDtoValidInputsTestData))]
        public void UpdateIncome_ReturnsOkObjectResult_WhenUserExistsAndIncomeExistAndIncomeDtoIsValid(
            IncomeDto incomeDto,
            string? optionalOwnerId
            )
        {
            authServiceMock.Setup(x => x.ValidateUsers(It.IsAny<HttpContext>(),
                It.IsAny<string>(), It.IsAny<string>(), out It.Ref<int>.IsAny, out It.Ref<string>.IsAny))
                .Returns(true);
            userServiceMock.Setup(x => x.GetById(It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new User());
            incomeRepoMock.Setup(x => x.Update(It.IsAny<Income>())).Returns(true);
            incomeRepoMock.Setup(x => x.ExistsById(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(true);


            var incomeService = new IncomeService(incomeRepoMock.Object, categoryRepoMock.Object);

            var incomeController = new IncomeController(incomeService, userServiceMock.Object, authServiceMock.Object);

            incomeController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                        new Claim(ClaimTypes.NameIdentifier, "user1")
                    }))
                }
            };

            // Act
            var result = incomeController.UpdateIncome(incomeDto, optionalOwnerId);

            // Assert

            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
            ResetAllSetups();
        }

        [Theory]
        [MemberData(nameof(IncomeDtoInvalidInputTestData))]
        public void UpdateIncome_ReturnsBadRequestObjectResult_WhenInputIsInvalid(
            IncomeDto incomeDto,
            string? optionalOwnerId
            )
        {
            // Arrange
            userServiceMock.Setup(x => x.GetById(It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new User() { Id = "VALID ID" });
            authServiceMock.Setup(x => x.ValidateUsers(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<string>(),
                out It.Ref<int>.IsAny, out It.Ref<string>.IsAny)).Returns(true);
            incomeRepoMock.Setup(x => x.ExistsById(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(true);
            var incomeService = new IncomeService(incomeRepoMock.Object, categoryRepoMock.Object);

            var incomeController = new IncomeController(incomeService, userServiceMock.Object, authServiceMock.Object);

            incomeController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "CURRENT USER")
                    }))
                }
            };

            // Act

            var result = incomeController.UpdateIncome(incomeDto, optionalOwnerId);

            // Assert

            Assert.IsType<BadRequestObjectResult>(result);
            ResetAllSetups();
        }


        [Theory]
        [MemberData(nameof(IncomeDtoValidInputsTestData))]
        public void UpdateIncome_ReturnsNotFoundObjectResult_WhenIncomeDoesNotExist(
            IncomeDto incomeDto,
            string? optionalOwnerId
            )
        {

            userServiceMock.Setup(x => x.GetById(It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new User() { Id = "VALID ID" });
            authServiceMock.Setup(x => x.ValidateUsers(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<string>(),
                out It.Ref<int>.IsAny, out It.Ref<string>.IsAny)).Returns(true);
            incomeRepoMock.Setup(x => x.ExistsById(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(false);
            var incomeService = new IncomeService(incomeRepoMock.Object, categoryRepoMock.Object);

            var incomeController = new IncomeController(incomeService, userServiceMock.Object, authServiceMock.Object);

            incomeController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "CURRENT USER")
                    }))
                }
            };

            // Act

            var result = incomeController.UpdateIncome(incomeDto, optionalOwnerId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<NotFoundObjectResult>(result);
            ResetAllSetups();
        }


        [Theory]
        [MemberData(nameof(IncomeDtoValidInputsTestData))]
        public void UpdateIncome_ReturnsObjectResult500_WhenUpdatingFails(
            IncomeDto incomeDto,
            string? optionalOwnerId
            )
        { 

            authServiceMock.Setup(x => x.ValidateUsers(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<string>(),
                out It.Ref<int>.IsAny, out It.Ref<string>.IsAny)).Returns(true);

            userServiceMock.Setup(x => x.GetById(It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new User());

            incomeRepoMock.Setup(x => x.Update(It.IsAny<Income>()))
                .Returns(false);

            incomeRepoMock.Setup(x => x.ExistsById(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(true);

            var incomeService = new IncomeService(incomeRepoMock.Object, categoryRepoMock.Object);

            var incomeController = new IncomeController(incomeService, userServiceMock.Object, authServiceMock.Object);

            incomeController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "CURRENT USER")
                    }))
                }
            };

            // Act
            var result = incomeController.UpdateIncome(incomeDto, null);

            // Assert

            Assert.IsType<ObjectResult>(result);
            var objectResult = (ObjectResult)result;
            Assert.Equal(500, objectResult.StatusCode);
            ResetAllSetups();
        }


        [Fact]
        public void DeleteIncome_ReturnsOkObjectResult_WhenUserAndIncomeExist()
        {
            userServiceMock.Setup(x => x.GetById(It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new User() { Id = "CURRENT USER " });
            authServiceMock.Setup(x => x.ValidateUsers(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<string>(),
                out It.Ref<int>.IsAny, out It.Ref<string>.IsAny)).Returns(true);
            incomeRepoMock.Setup(x => x.ExistsById(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(true);
            incomeRepoMock.Setup(x => x.Delete(It.IsAny<Income>())).Returns(true);
            incomeRepoMock.Setup(x => x.GetById(It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(new Income() { Id = 1 });

            var incomeService = new IncomeService(incomeRepoMock.Object, categoryRepoMock.Object);

            var incomeController = new IncomeController(incomeService, userServiceMock.Object, authServiceMock.Object);

            incomeController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "CURRENT USER")
                    }))
                }
            };
            var validIncomeId = 1;
            // Act

            var result = incomeController.DeleteIncome(validIncomeId, null);

            // Assert

            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
            ResetAllSetups();
        }

        [Fact]
        public void DeleteIncome_ReturnsNotFoundObjectResult_WhenIncomeDoesNotExist()
        {
            userServiceMock.Setup(x => x.GetById(It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new User() { Id = "CURRENT USER " });
            authServiceMock.Setup(x => x.ValidateUsers(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<string>(),
                out It.Ref<int>.IsAny, out It.Ref<string>.IsAny)).Returns(true);
            incomeRepoMock.Setup(x => x.ExistsById(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(false);

            var incomeService = new IncomeService(incomeRepoMock.Object, categoryRepoMock.Object);

            var incomeController = new IncomeController(incomeService, userServiceMock.Object, authServiceMock.Object);

            incomeController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "CURRENT USER")
                    }))
                }
            };
            var nonExistingIncomeId = 1;
            // Act

            var result = incomeController.DeleteIncome(nonExistingIncomeId, null);

            // Assert

            Assert.NotNull(result);
            Assert.IsType<NotFoundObjectResult>(result);
            ResetAllSetups();
        }



        private void ResetAllSetups()
        {
            incomeRepoMock.Reset();
            categoryRepoMock.Reset();
            userServiceMock.Reset();
            authServiceMock.Reset();
            userRepoMock.Reset();
            authorizeRepoMock.Reset();
            auhtorizationInviteRepoMock.Reset();
        }
    }
}
