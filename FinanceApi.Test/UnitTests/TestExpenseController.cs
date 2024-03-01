using FinanceApi.Controllers;
using FinanceApi.Data;
using FinanceApi.Data.Dtos;
using FinanceApi.Enums;
using FinanceApi.Models;
using FinanceApi.Repositories.Interfaces;
using FinanceApi.Services;
using FinanceApi.Services.Interfaces;
using FinanceApi.Test.TestDatabase;
using FinanceApi.Test.TestDataHolder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace FinanceApi.Test.Tests
{
    public class TestExpenseController
    {
        private readonly Mock<ICategoryRepository> categoryRepoMock = new Mock<ICategoryRepository>();
        private readonly Mock<IAuthorizeService> authServiceMock = new Mock<IAuthorizeService>();
        private readonly Mock<IExpenseRepository> expenseRepoMock = new Mock<IExpenseRepository>();
        private readonly Mock<IUserService> userServiceMock = new Mock<IUserService>();
        private readonly Mock<IAuthorizeRepository> authorizeRepoMock = new Mock<IAuthorizeRepository>();
        private readonly Mock<IAuthorizationInviteRepository> auhtorizationInviteRepoMock = new Mock<IAuthorizationInviteRepository>();
        private readonly Mock<IUserRepository> userRepoMock = new Mock<IUserRepository>();

        private DataContext dataContext;
        public TestExpenseController()
        {
            var testDatabaseFixture = new TestDatabaseFixture();
            dataContext = testDatabaseFixture.dataContext;
        }




        [Theory]
        [MemberData(nameof(TestData.GetUsersExpensesValidInputsTestData), MemberType = typeof(TestData))]
        public void GetExpenses_ReturnsOk_When_UserExistsAndHasValidInputs(
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

            var expenses = new List<Expense>()
                {
                    new Expense()
                        {
                            Id = 1,
                            Title = "a-expense",
                            Description = "description",
                            Amount = 100,
                            Currency = "EUR",
                            Date = new DateTime(2021, 1, 1),
                            DocumentUrl = "URL",
                            Urgency = Urgency.Low,
                            User = new User(){ Id = "user1" },
                        },
                    new Expense()
                    {
                        Id = 2,
                        Title = "b-expense",
                        Description = "another description",
                        Amount = 150,
                        Currency = "USD",
                        Date = new DateTime(2021, 2, 1),
                        DocumentUrl = "AnotherURL",
                        Urgency = Urgency.Low,
                        User = new User(){ Id = "user2"},
                    },
                    new Expense()
                    {
                        Id = 3,
                        Title = "c-expense",
                        Description = "yet another description",
                        Amount = 200,
                        Currency = "GBP",
                        Date = new DateTime(2021, 3, 1),
                        DocumentUrl = "YetAnotherURL",
                        Urgency = Urgency.Medium,
                        User = new User(){ Id = "user1" },
                    },
                    new Expense()
                    {
                        Id = 4,
                        Title = "e-expense",
                        Description = "additional description",
                        Amount = 180,
                        Currency = "AUD",
                        Date = new DateTime(2021, 5, 1),
                        DocumentUrl = "AdditionalURL",
                        Urgency = Urgency.Medium,
                        User = new User() { Id = "user2" },
                    },
                    new Expense()
                    {
                        Id = 5,
                        Title = "f-expense",
                        Description = "final description",
                        Amount = 250,
                        Currency = "JPY",
                        Date = new DateTime(2021, 6, 1),
                        DocumentUrl = "FinalURL",
                        Urgency = Urgency.High,
                        User = new User() { Id = "user1" },
                    },
                    new Expense()
                    {
                        Id = 6,
                        Title = "d-expense",
                        Description = "more description",
                        Amount = 120,
                        Currency = "CAD",
                        Date = new DateTime(2021, 4, 1),
                        DocumentUrl = "MoreURL",
                        Urgency = Urgency.High,
                        User = new User() { Id = "user2" },
                    },
                };

            expenseRepoMock.Setup(x => x.GetAllOfUser(It.IsAny<string>()))
                .Returns(expenses.Where(i => i.User.Id.Equals(optionalOwnerId ?? "user1")).ToList());

            IExpenseService expenseService = new ExpenseService(
                expenseRepoMock.Object,
                categoryRepoMock.Object);

            var expenseController = new ExpenseController(
                expenseService,
                userServiceMock.Object,
                authServiceMock.Object);


            expenseController.ControllerContext = new ControllerContext()
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
            var result = expenseController.GetUsersExpenses(
                from, to, list_order_by, list_dir, optionalOwnerId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = ((OkObjectResult)result).Value;
            Assert.IsType<List<ExpenseDto>>(okResult);

            var expenseDtos = (List<ExpenseDto>)okResult;


            var orderedExpenseList = new List<ExpenseDto>(expenseDtos);
            if (list_order_by != null || list_dir != null)
            {
                list_order_by = list_order_by ?? "DEFAULT (NO ORDERING)";
                switch (list_order_by.ToLower())
                {
                    case "title":
                        orderedExpenseList = list_dir != null && list_dir.Equals("desc")
                            ? orderedExpenseList.OrderByDescending(dto => dto.Title).ToList()
                            : orderedExpenseList.OrderBy(dto => dto.Title).ToList();
                        break;
                    case "amount":
                        orderedExpenseList = list_dir != null && list_dir.Equals("desc")
                            ? orderedExpenseList.OrderByDescending(dto => dto.Amount).ToList()
                            : orderedExpenseList.OrderBy(dto => dto.Amount).ToList();
                        break;
                    case "urgency":
                        orderedExpenseList = list_dir != null && list_dir.Equals("desc")
                            ? orderedExpenseList.OrderByDescending(dto => dto.Urgency).ToList()
                            : orderedExpenseList.OrderBy(dto => dto.Urgency).ToList();
                        break;
                    default:
                        orderedExpenseList = list_dir != null && list_dir.Equals("desc") ?
                            orderedExpenseList.OrderByDescending(dto => dto.Date).ToList()
                            : orderedExpenseList.OrderBy(idto => idto.Date).ToList();
                        break;
                }
            }

            Assert.Equal(expenseDtos, orderedExpenseList);
            ResetAllSetups();
        }




        [Theory]
        [MemberData(nameof(TestData.GetUsersExpensesInvalidInputsTestData), MemberType = typeof(TestData))]
        public void GetUsersExpenses_ReturnsBadRequest_WhenUserExistsAndHasInvalidInput(
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
            expenseRepoMock.Setup(x => x.GetAllOfUser(It.IsAny<string>())).Returns(
                new List<Expense>()
                );

            var expenseService = new ExpenseService(expenseRepoMock.Object, categoryRepoMock.Object);

            var expenseController = new ExpenseController(expenseService, userServiceMock.Object,
                authServiceMock.Object);

            expenseController.ControllerContext = new ControllerContext()
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


            var result = expenseController.GetUsersExpenses(from,
                to, list_order_by, list_dir, optionalOwnerId);


            // Assert

            Assert.IsType<BadRequestObjectResult>(result);

            var badRequestResult = (BadRequestObjectResult)result;

            ResetAllSetups();
        }

        [Fact]
        public void GetUsersExpenses_ReturnsUnauthorizedObjectResult_WhenCurrentUserIsNotFound()
        {
            // Arrange

            userRepoMock.Setup(x => x.ExistsById(It.IsAny<string>())).Returns(false);

            var expenseService = new ExpenseService(expenseRepoMock.Object, categoryRepoMock.Object);
            var authservice = new AuthorizeService(authorizeRepoMock.Object, auhtorizationInviteRepoMock.Object, userRepoMock.Object);
            var expenseController = new ExpenseController(expenseService, userServiceMock.Object, authservice);

            expenseController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]{
                        new Claim(ClaimTypes.NameIdentifier, "user1")
                    }))
                }
            };

            // Act

            var result = expenseController.GetUsersExpenses(null, null, null, null, null);

            // Assert

            Assert.IsType<UnauthorizedObjectResult>(result);
            var test = (UnauthorizedObjectResult)result;
            Assert.Equal(401, test.StatusCode);
            ResetAllSetups();
        }

        [Fact]
        public void GetUsersExpenses_ReturnsNotFoundObjectResult_WhenOptionalOwnerDoesNotExist()
        {
            // Arrange
            var expenseService = new ExpenseService(expenseRepoMock.Object, categoryRepoMock.Object);
            var authservice = new AuthorizeService(authorizeRepoMock.Object, auhtorizationInviteRepoMock.Object, userRepoMock.Object);
            var expenseController = new ExpenseController(expenseService, userServiceMock.Object, authservice);

            userRepoMock.Setup(x => x.ExistsById(It.Is<string>(s => s.Equals("CURRENT USER"))))
                .Returns(true);

            expenseController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]{
                        new Claim(ClaimTypes.NameIdentifier, "CURRENT USER")
                    }))
                }
            };

            // Act
            var result = expenseController.GetUsersExpenses(null, null, null, null, "THIS USER DOES NOT EXIST");

            // Assert

            Assert.IsType<NotFoundObjectResult>(result);
            var statusCode = ((NotFoundObjectResult)result).StatusCode;

            Assert.Equal(404, statusCode);

            ResetAllSetups();
        }

        [Fact]
        public void GetUsersExpenses_ReturnsForbiddenResult_WhenUsersExistButCurrentUserIsForbiddenToGet()
        {

            // Arrange

            var expenseService = new ExpenseService(expenseRepoMock.Object, categoryRepoMock.Object);
            var authservice = new AuthorizeService(authorizeRepoMock.Object, auhtorizationInviteRepoMock.Object, userRepoMock.Object);
            var expenseController = new ExpenseController(expenseService, userServiceMock.Object, authservice);


            authorizeRepoMock.Setup(x => x.IsAuthorized(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(false);


            userRepoMock.Setup(x => x.ExistsById(It.IsAny<string>()))
                .Returns(true);

            expenseController.ControllerContext = new ControllerContext()
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

            var result = expenseController.GetUsersExpenses(null, null, null, null, "OWNER DID NOT AUTHORIZE CURRENT USER");

            // Assert

            Assert.IsType<ForbidResult>(result);
            ResetAllSetups();
        }



        [Theory]
        [MemberData(nameof(TestData.ExpenseDtoValidInputsTestData), MemberType = typeof(TestData))]
        public void CreateExpense_ReturnsOkResultObject_WhenUserExistsAndIsValidInput(
            ExpenseDto expenseDto,
            string? optionalOwnerId
            )
        {
            // Arrange
            authServiceMock.Setup(x => x.ValidateUsers(It.IsAny<HttpContext>(),
                It.IsAny<string>(), It.IsAny<string>(), out It.Ref<int>.IsAny, out It.Ref<string>.IsAny))
                .Returns(true);
            userServiceMock.Setup(x => x.GetById(It.IsAny<string>(), It.IsAny<bool>())).Returns(new User());
            expenseRepoMock.Setup(x => x.Create(It.IsAny<Expense>())).Returns(true);

            var expenseService = new ExpenseService(expenseRepoMock.Object, categoryRepoMock.Object);

            var expenseController = new ExpenseController(expenseService, userServiceMock.Object, authServiceMock.Object);

            expenseController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                        new Claim(ClaimTypes.NameIdentifier, "user1")
                    }))
                }
            };

            // Act
            var result = expenseController.CreateExpense(expenseDto, optionalOwnerId);

            // Assert

            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
            ResetAllSetups();
        }



        [Theory]
        [MemberData(nameof(TestData.ExpenseDtoInvalidInputTestData), MemberType = typeof(TestData))]
        public void CreateExpense_ReturnsBadRequestObjectResult_WhenInputIsInvalid(
            ExpenseDto expenseDto,
            string? optionalOwnerId
            )
        {
            // Arrange
            authServiceMock.Setup(x => x.ValidateUsers(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<string>(),
                out It.Ref<int>.IsAny, out It.Ref<string>.IsAny)).Returns(true);

            var expenseService = new ExpenseService(expenseRepoMock.Object, categoryRepoMock.Object);

            var expenseController = new ExpenseController(expenseService, userServiceMock.Object, authServiceMock.Object);

            expenseController.ControllerContext = new ControllerContext()
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

            var result = expenseController.CreateExpense(expenseDto, optionalOwnerId);

            // Assert

            Assert.IsType<BadRequestObjectResult>(result);
            ResetAllSetups();
        }



        [Theory]
        [MemberData(nameof(TestData.ExpenseDtoValidInputsTestData), MemberType = typeof(TestData))]
        public void CreateExpense_ReturnsObjectResult500_WhenCreatingFails(
            ExpenseDto expenseDto,
            string? optionalOwnerId
            )
        {
            // Arrange 
            authServiceMock.Setup(x => x.ValidateUsers(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<string>(),
                out It.Ref<int>.IsAny, out It.Ref<string>.IsAny)).Returns(true);

            userServiceMock.Setup(x => x.GetById(It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new User());

            expenseRepoMock.Setup(x => x.Create(It.IsAny<Expense>()))
                .Returns(false);

            var expenseService = new ExpenseService(expenseRepoMock.Object, categoryRepoMock.Object);

            var expenseController = new ExpenseController(expenseService, userServiceMock.Object, authServiceMock.Object);

            expenseController.ControllerContext = new ControllerContext()
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
            var result = expenseController.CreateExpense(expenseDto, null);

            // Assert

            Assert.IsType<ObjectResult>(result);
            var objectResult = (ObjectResult)result;
            Assert.Equal(500, objectResult.StatusCode);
            ResetAllSetups();
        }





        [Theory]
        [MemberData(nameof(TestData.AddCategoryToExpenseValidInputTestDataUnitTest), MemberType = typeof(TestData))]
        public void AddCategoryToExpense_ReturnsOkObjectResult_WhenExpenseExistsAndCategoryIdsAreValid(
            int expenseId, ICollection<int> categoryIds, string? optionalOwnerId
            )
        {
            // Arrange

            var mockDatabaseExpenseCategories = new List<ExpenseCategory>()
            {
                new ExpenseCategory() { CategoryId = 100, Expense = new Expense(), ExpenseId = expenseId, Category = new Category() }
            };

            authServiceMock.Setup(x => x.ValidateUsers(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<string>(),
                out It.Ref<int>.IsAny, out It.Ref<string>.IsAny)).Returns(true);
            expenseRepoMock.Setup(x => x.ExistsById(It.IsAny<string>(), It.IsAny<int>())).Returns(true);
            expenseRepoMock.Setup(x => x.GetById(It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(new Expense() { Id = 1 });
            categoryRepoMock.Setup(x => x.GetExpenseCategories(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(mockDatabaseExpenseCategories);
            categoryRepoMock.Setup(x => x.ExistsById(It.IsAny<string>(), It.IsAny<int>())).Returns(true);
            expenseRepoMock.Setup(x => x.AddCategory(It.IsAny<ExpenseCategory>())).Returns(true);

            var expenseService = new ExpenseService(expenseRepoMock.Object, categoryRepoMock.Object);

            var expenseController = new ExpenseController(expenseService, userServiceMock.Object, authServiceMock.Object);

            expenseController.ControllerContext = new ControllerContext()
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

            var result = expenseController.AddCategoryToExpense(expenseId, categoryIds, optionalOwnerId);

            // Assert

            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);


            ResetAllSetups();
        }




        [Theory]
        [MemberData(nameof(TestData.ExpenseDtoValidInputsTestData), MemberType = typeof(TestData))]
        public void UpdateExpense_ReturnsOkObjectResult_WhenUserExistsAndExpenseExistAndExpenseDtoIsValid(
            ExpenseDto expenseDto,
            string? optionalOwnerId
            )
        {
            authServiceMock.Setup(x => x.ValidateUsers(It.IsAny<HttpContext>(),
                It.IsAny<string>(), It.IsAny<string>(), out It.Ref<int>.IsAny, out It.Ref<string>.IsAny))
                .Returns(true);
            userServiceMock.Setup(x => x.GetById(It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new User());
            expenseRepoMock.Setup(x => x.Update(It.IsAny<Expense>())).Returns(true);
            expenseRepoMock.Setup(x => x.ExistsById(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(true);


            var expenseService = new ExpenseService(expenseRepoMock.Object, categoryRepoMock.Object);

            var expenseController = new ExpenseController(expenseService, userServiceMock.Object, authServiceMock.Object);

            expenseController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                        new Claim(ClaimTypes.NameIdentifier, "user1")
                    }))
                }
            };

            // Act
            var result = expenseController.UpdateExpense(expenseDto, optionalOwnerId);

            // Assert

            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
            ResetAllSetups();
        }

        [Theory]
        [MemberData(nameof(TestData.ExpenseDtoInvalidInputTestData), MemberType = typeof(TestData))]
        public void UpdateExpense_ReturnsBadRequestObjectResult_WhenInputIsInvalid(
            ExpenseDto expenseDto,
            string? optionalOwnerId
            )
        {
            // Arrange
            userServiceMock.Setup(x => x.GetById(It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new User() { Id = "VALID ID" });
            authServiceMock.Setup(x => x.ValidateUsers(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<string>(),
                out It.Ref<int>.IsAny, out It.Ref<string>.IsAny)).Returns(true);
            expenseRepoMock.Setup(x => x.ExistsById(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(true);
            var expenseService = new ExpenseService(expenseRepoMock.Object, categoryRepoMock.Object);

            var expenseController = new ExpenseController(expenseService, userServiceMock.Object, authServiceMock.Object);

            expenseController.ControllerContext = new ControllerContext()
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

            var result = expenseController.UpdateExpense(expenseDto, optionalOwnerId);

            // Assert

            Assert.IsType<BadRequestObjectResult>(result);
            ResetAllSetups();
        }


        [Theory]
        [MemberData(nameof(TestData.ExpenseDtoValidInputsTestData), MemberType = typeof(TestData))]
        public void UpdateExpense_ReturnsNotFoundObjectResult_WhenExpenseDoesNotExist(
            ExpenseDto expenseDto,
            string? optionalOwnerId
            )
        {

            userServiceMock.Setup(x => x.GetById(It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new User() { Id = "VALID ID" });
            authServiceMock.Setup(x => x.ValidateUsers(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<string>(),
                out It.Ref<int>.IsAny, out It.Ref<string>.IsAny)).Returns(true);
            expenseRepoMock.Setup(x => x.ExistsById(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(false);
            var expenseService = new ExpenseService(expenseRepoMock.Object, categoryRepoMock.Object);

            var expenseController = new ExpenseController(expenseService, userServiceMock.Object, authServiceMock.Object);

            expenseController.ControllerContext = new ControllerContext()
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

            var result = expenseController.UpdateExpense(expenseDto, optionalOwnerId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<NotFoundObjectResult>(result);
            ResetAllSetups();
        }


        [Theory]
        [MemberData(nameof(TestData.ExpenseDtoValidInputsTestData), MemberType = typeof(TestData))]
        public void UpdateExpense_ReturnsObjectResult500_WhenUpdatingFails(
            ExpenseDto expenseDto,
            string? optionalOwnerId
            )
        {

            authServiceMock.Setup(x => x.ValidateUsers(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<string>(),
                out It.Ref<int>.IsAny, out It.Ref<string>.IsAny)).Returns(true);

            userServiceMock.Setup(x => x.GetById(It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new User());

            expenseRepoMock.Setup(x => x.Update(It.IsAny<Expense>()))
                .Returns(false);

            expenseRepoMock.Setup(x => x.ExistsById(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(true);

            var expenseService = new ExpenseService(expenseRepoMock.Object, categoryRepoMock.Object);

            var expenseController = new ExpenseController(expenseService, userServiceMock.Object, authServiceMock.Object);

            expenseController.ControllerContext = new ControllerContext()
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
            var result = expenseController.UpdateExpense(expenseDto, null);

            // Assert

            Assert.IsType<ObjectResult>(result);
            var objectResult = (ObjectResult)result;
            Assert.Equal(500, objectResult.StatusCode);
            ResetAllSetups();
        }


        [Fact]
        public void DeleteExpense_ReturnsOkObjectResult_WhenUserAndExpenseExist()
        {
            userServiceMock.Setup(x => x.GetById(It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new User() { Id = "CURRENT USER " });
            authServiceMock.Setup(x => x.ValidateUsers(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<string>(),
                out It.Ref<int>.IsAny, out It.Ref<string>.IsAny)).Returns(true);
            expenseRepoMock.Setup(x => x.ExistsById(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(true);
            expenseRepoMock.Setup(x => x.Delete(It.IsAny<Expense>())).Returns(true);
            expenseRepoMock.Setup(x => x.GetById(It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(new Expense() { Id = 1 });

            var expenseService = new ExpenseService(expenseRepoMock.Object, categoryRepoMock.Object);

            var expenseController = new ExpenseController(expenseService, userServiceMock.Object, authServiceMock.Object);

            expenseController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "CURRENT USER")
                    }))
                }
            };
            var validExpenseId = 1;
            // Act

            var result = expenseController.DeleteExpense(validExpenseId, null);

            // Assert

            Assert.NotNull(result);
            Assert.IsType<NoContentResult>(result);
            ResetAllSetups();
        }

        [Fact]
        public void DeleteExpense_ReturnsNotFoundObjectResult_WhenExpenseDoesNotExist()
        {
            userServiceMock.Setup(x => x.GetById(It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new User() { Id = "CURRENT USER " });
            authServiceMock.Setup(x => x.ValidateUsers(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<string>(),
                out It.Ref<int>.IsAny, out It.Ref<string>.IsAny)).Returns(true);
            expenseRepoMock.Setup(x => x.ExistsById(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(false);

            var expenseService = new ExpenseService(expenseRepoMock.Object, categoryRepoMock.Object);

            var expenseController = new ExpenseController(expenseService, userServiceMock.Object, authServiceMock.Object);

            expenseController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "CURRENT USER")
                    }))
                }
            };
            var nonExistingExpenseId = 1;
            // Act

            var result = expenseController.DeleteExpense(nonExistingExpenseId, null);

            // Assert

            Assert.NotNull(result);
            Assert.IsType<NotFoundObjectResult>(result);
            ResetAllSetups();
        }




        [Fact]
        public void DeleteExpense_ReturnsObjectResult500_WhenDeletingFails()
        {

            authServiceMock.Setup(x => x.ValidateUsers(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<string>(),
                out It.Ref<int>.IsAny, out It.Ref<string>.IsAny)).Returns(true);
            userServiceMock.Setup(x => x.GetById(It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new User() { Id = "CURRENT USER" });

            expenseRepoMock.Setup(x => x.ExistsById(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(true);
            expenseRepoMock.Setup(x => x.GetById(It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(new Expense() { Id = 1 });
            expenseRepoMock.Setup(x => x.Delete(It.IsAny<Expense>())).Returns(false);

            var expenseService = new ExpenseService(expenseRepoMock.Object, categoryRepoMock.Object);

            var expenseController = new ExpenseController(expenseService, userServiceMock.Object, authServiceMock.Object);

            expenseController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "CURRENT USER")
                    }))
                }
            };
            int validId = 1;

            // Act
            var result = expenseController.DeleteExpense(validId, null);

            // Assert   

            Assert.NotNull(result);
            Assert.IsType<ObjectResult>(result);
            var statusCode = ((ObjectResult)result).StatusCode;
            Assert.Equal(500, statusCode);

            ResetAllSetups();
        }



        private void ResetAllSetups()
        {
            expenseRepoMock.Reset();
            categoryRepoMock.Reset();
            userServiceMock.Reset();
            authServiceMock.Reset();
            userRepoMock.Reset();
            authorizeRepoMock.Reset();
            auhtorizationInviteRepoMock.Reset();
        }
    }
}
