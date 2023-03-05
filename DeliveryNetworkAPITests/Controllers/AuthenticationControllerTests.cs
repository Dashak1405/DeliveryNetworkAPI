using Microsoft.VisualStudio.TestTools.UnitTesting;
using DeliveryNetworkAPI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeliveryNetworkAPI.Data;
using DeliveryNetworkAPI.Models.RequestModels;
using DeliveryNetworkAPITests;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryNetworkAPI.Controllers.Tests
{
    [TestClass()]
    public class AuthenticationControllerTests
    {

        private DeliveryNetworkDbContext? _ctx;
        private IOptions<AuthOptions> _authOpts;
        private List<CreateUserClass> _creatingUsers = new List<CreateUserClass> {
            new CreateUserClass { login = "log1", password = "password1", name = "name1", surname = "surname1", lastname = "lastname1", passport = "passport1" },
            new CreateUserClass { login = "log2", password = "password2", name = "name2", surname = "surname2", lastname = "lastname2", passport = "passport2" }
        };

        [TestInitialize()]
        public async Task Initialize()
        {
            _ctx = DBContextMockHelper.InitDbContextWithPosts();
            var confBuilder = new ConfigurationBuilder().AddJsonFile($"appsettings.json", optional: false) ;
            var authConf = confBuilder.Build().GetSection("Auth");

            _authOpts = Options.Create<AuthOptions>(new AuthOptions());

            _authOpts.Value.Issuer = authConf["Issuer"];
            _authOpts.Value.Audience = authConf["Audience"];
            _authOpts.Value.Secret = authConf["Secret"];
            _authOpts.Value.TokenLifetime = Int32.Parse(authConf["TokenLifetime"]);
            
            // Удостовериться, что должности добавились
            var posts = _ctx.Posts.ToList();
            Assert.AreEqual(3, posts.Count());
            Assert.AreEqual("user", posts[0].Post);
            Assert.AreEqual("admin", posts[1].Post);
            Assert.AreEqual("deliveryman", posts[2].Post);

            // Добавим пользователей
            var userController = new UsersController(_ctx);
            await userController.AddUser(_creatingUsers[0]);
            await userController.AddUser(_creatingUsers[1]);
        }

        [TestMethod()]
        public void SuccesfullyLogin_ReturnOk200CodeWithJWTTokenTest()
        {
            // Arrangment
            var authController = new AuthenticationController(_ctx, _authOpts);
            List<Login> logins = new List<Login> {
                new Login { username = "log1", password = "password1"},
                new Login { username = "log2", password = "password2"}
            };
            // Act

            var result_1 = authController.Login(logins[0]);
            var result_2 = authController.Login(logins[1]);
            var actualResult_1 = result_1 as OkObjectResult;
            var actualResult_2 = result_2 as OkObjectResult;
            
            // Asserts
            Assert.IsNotNull(actualResult_1);
            Assert.IsNotNull(actualResult_2);
            Assert.AreEqual(200, actualResult_1.StatusCode);
            Assert.AreEqual(200, actualResult_2.StatusCode);
            Assert.IsInstanceOfType(actualResult_1.Value, typeof(JwtTokenResponse));
            Assert.IsInstanceOfType(actualResult_2.Value, typeof(JwtTokenResponse));

        }

        [TestMethod()]
        public void FailLoginExistentUserWithWrongCredentials_ReturnNotFound404CodeWithText_Test()
        {
            // Arrangment
            var authController = new AuthenticationController(_ctx, _authOpts);
            List<Login> logins = new List<Login> {
                new Login { username = "log1", password = "wrongPassword"},
                new Login { username = "log2", password = "wrongPassword"}
            };
            // Act

            var result_1 = authController.Login(logins[0]);
            var result_2 = authController.Login(logins[1]);
            var actualResult_1 = result_1 as NotFoundObjectResult;
            var actualResult_2 = result_2 as NotFoundObjectResult;

            // Asserts
            Assert.IsNotNull(actualResult_1);
            Assert.AreEqual(404, actualResult_1.StatusCode);
            Assert.IsInstanceOfType(actualResult_1.Value, typeof(string));

            Assert.IsNotNull(actualResult_2);
            Assert.AreEqual(404, actualResult_2.StatusCode);
            Assert.IsInstanceOfType(actualResult_2.Value, typeof(string));
        }


        [TestMethod()]
        public void FailLoginNotExistentUser_ReturnNotFound404CodeWithText_Test()
        {
            // Arrangment
            var authController = new AuthenticationController(_ctx, _authOpts);
            var login = new Login { username = "log3", password = "password1" };
            // Act

            var result = authController.Login(login);
            var actualResult = result as NotFoundObjectResult;

            // Asserts
            Assert.IsNotNull(actualResult);
            Assert.AreEqual(404, actualResult.StatusCode);
            Assert.IsInstanceOfType(actualResult.Value, typeof(string));
        }
    }
}