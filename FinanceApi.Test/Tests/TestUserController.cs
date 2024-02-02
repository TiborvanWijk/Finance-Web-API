using FinanceApi.Controllers;
using FinanceApi.Data;
using FinanceApi.Data.Dtos;
using FinanceApi.Models;
using FinanceApi.Repositories;
using FinanceApi.Repositories.Interfaces;
using FinanceApi.Services;
using FinanceApi.Services.Interfaces;
using FinanceApi.Test.TestDatabase;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace FinanceApi.Test.Tests
{
    public class TestUserController
    {

        private DataContext dataContext;
        public TestUserController()
        {
            var testDatabaseFixture = new TestDatabaseFixture();
            dataContext = testDatabaseFixture.dataContext;
        }

        [Fact]
        public void GetCurrentUser_ReturnsOk_WhenUserExists()
        {
            // Arrange
            var userSerMock = new Mock<IUserService>();

            userSerMock.Setup(x => x.ExistsById(It.IsAny<string>())).Returns(true);
            userSerMock.Setup(x => x.TryGetUserBalance(It.IsAny<string>(),
                out It.Ref<decimal>.IsAny,
                out It.Ref<int>.IsAny,
                out It.Ref<string>.IsAny)).Returns(true);
            userSerMock.Setup(x => x.GetById(It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new User()
                {
                    Id = "Id",
                    UserName = "user123",
                    Email = "email@email.com",
                    Currency = "Eur",
                });

            var controller = new UserController(userSerMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "user123")
                }))
                }
            };

            // Act
            var result = controller.GetCurrentUser();


            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = (OkObjectResult)result;
            Assert.IsType<UserDto>(okResult.Value);
        }

        [Fact]
        public void GetCurrentUser_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(x => x.ExistsById(It.IsAny<string>())).Returns(false);

            var controller = new UserController(userServiceMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "user123")
                }))
                }
            };

            // Act
            var result = controller.GetCurrentUser();

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void UpdateUsersCurrency_ReturnsOk_WhenUpdateIsSuccessful()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(x => x.TryUpdateUserCurrency(It.IsAny<string>(), It.IsAny<string>(), out It.Ref<int>.IsAny, out It.Ref<string>.IsAny))
                .Returns(true);

            var controller = new UserController(userServiceMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "user123")
                }))
                }
            };

            // Act
            var result = controller.UpdateUsersCurrency("USD");

            // Assert
            Assert.IsType<OkObjectResult>(result);

            var okResult = (OkObjectResult)result;
            Assert.Equal("Updated users currency succesfully.", okResult.Value);
        }

        [Fact]
        public void UpdateUsersCurrency_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange


            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(x => x.ExistsById(It.IsAny<string>())).Returns(false);
            var incomeRepoMock = new Mock<IIncomeRepository>();
            var expenseRepoMock = new Mock<IExpenseRepository>();

            var userService = new UserService(userRepoMock.Object,
                expenseRepoMock.Object,
                incomeRepoMock.Object);

            var userController = new UserController(userService);

            userController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "user123")
                    }))
                }
            };

            // Act

            var result = userController.UpdateUsersCurrency("usd");

            // Assert

            Assert.IsType<NotFoundObjectResult>(result);
        }


        [Fact]
        public void UpdateUsersCurrency_ReturnsBadRequest_WhenCurrencyIsoCodeIsInvalid()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            var expenseRepoMock = new Mock<IExpenseRepository>();
            var incomeRepoMock = new Mock<IIncomeRepository>();

            userRepoMock.Setup(x => x.ExistsById(It.IsAny<string>())).Returns(true);

            var userService = new UserService(userRepoMock.Object,
                expenseRepoMock.Object,
                incomeRepoMock.Object);
            var userController = new UserController(userService);

            userController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "user123")
                    }))
                }
            };

            // Act

            var result = userController.UpdateUsersCurrency("WRONG CODE");

            // Assert

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void UpdateUsersCurrency_ReturnsObjectResult500_WhenUpdateFails()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            var expenseRepoMock = new Mock<IExpenseRepository>();
            var incomeRepoMock = new Mock<IIncomeRepository>();

            userRepoMock.Setup(x => x.ExistsById(It.IsAny<string>())).Returns(true);
            userRepoMock.Setup(x => x.Update(It.IsAny<User>())).Returns(false);
            userRepoMock.Setup(x => x.GetById(It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new User()
                {
                    Id = "ID",
                    UserName = "user123",
                    Currency = "EUR",
                    Email = "email@email.com",
                });
            var userService = new UserService(userRepoMock.Object,
                expenseRepoMock.Object,
                incomeRepoMock.Object);
            var userController = new UserController(userService);

            userController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "user123")
                    }))
                }
            };

            // Act
            var result = userController.UpdateUsersCurrency("usd");

            // Assert

            Assert.IsType<ObjectResult>(result);
        }


    }
}