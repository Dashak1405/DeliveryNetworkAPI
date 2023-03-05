using Microsoft.VisualStudio.TestTools.UnitTesting;
using DeliveryNetworkAPI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using DeliveryNetworkAPI.Data;
using DeliveryNetworkAPI.Models.RequestModels;
using DeliveryNetworkAPI.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DeliveryNetworkAPITests;
using System.Web.Http.Results;
using System.Net.Sockets;
using ConflictResult = Microsoft.AspNetCore.Mvc.ConflictResult;
using System.Data.Common;
using NotFoundResult = Microsoft.AspNetCore.Mvc.NotFoundResult;

/* https://playwithcsharpdotnet.blogspot.com/2020/04/inject-and-mock-db-context-with-inmemory-options.html
 * 
 */

namespace DeliveryNetworkAPI.Controllers.Tests
{
    [TestClass()]
    public class UsersControllerTests
    {
        private DeliveryNetworkDbContext _ctx;
        private List<CreateUserClass> _creatingUsers = new List<CreateUserClass> {
            new CreateUserClass { login = "log1", password = "password1", name = "name1", surname = "surname1", lastname = "lastname1", passport = "passport1" },
            new CreateUserClass { login = "log2", password = "password2", name = "name2", surname = "surname2", lastname = "lastname2", passport = "passport2" }
        };

        [TestInitialize()]
        public void Initialize() {
            _ctx = DBContextMockHelper.InitDbContextWithPosts();

            // Удостовериться, что должности добавились
            var posts = _ctx.Posts.ToList();
            Assert.AreEqual(3, posts.Count());
            Assert.AreEqual("user", posts[0].Post);
            Assert.AreEqual("admin", posts[1].Post);
            Assert.AreEqual("deliveryman", posts[2].Post);
        }


        [TestMethod()]
        public async Task SuccesfullyAddTwoUsers_ReturnCreated201Code_AsyncTest()
        {
            // Arrangment
            
            var userController = new UsersController(_ctx);
           
            // Act
            var result1 = await userController.AddUser(_creatingUsers[0]);
            var actualResult1 = result1 as CreatedResult;
            var result2 = await userController.AddUser(_creatingUsers[1]);
            var actualResult2 = result2 as CreatedResult;


            // Asserts
            Assert.AreEqual(2, _ctx.Users.Count());

            Assert.IsNotNull(actualResult1);
            Assert.AreEqual(_creatingUsers[0].login, actualResult1.Value);
            Assert.IsNotNull(actualResult2);
            Assert.AreEqual(_creatingUsers[1].login, actualResult2.Value);

            Assert.AreEqual(201, actualResult1.StatusCode);
            Assert.AreEqual(201, actualResult2.StatusCode);
        }

        [TestMethod()]
        public async Task FailAddSimilarUser_ReturnConflict409Code_AsyncTest()
        {
            // Arrangment
            var userController = new UsersController(_ctx);
            await userController.AddUser(_creatingUsers[0]);
            await userController.AddUser(_creatingUsers[1]);

            // Act
            var result1 = await userController.AddUser(_creatingUsers[0]);
            var actualResult1 = result1 as ConflictResult;

            // Asserts
            Assert.AreEqual(2, _ctx.Users.Count());
            Assert.IsNotNull(actualResult1);
            Assert.AreEqual(409, actualResult1.StatusCode);
        }

        [TestMethod()]
        public async Task SuccesfullyGetAllZeroUsers_ReturnOk200Code_AsyncTest()
        {
            // Arrangment
            var userController = new UsersController(_ctx);

            // Act
            var result = await userController.GetAllUsers();
            var actualResult = result as OkObjectResult;
            var value = actualResult.Value as List<RequestUsers>;

            Assert.IsNotNull(actualResult);
            Assert.AreEqual(200, actualResult.StatusCode);
            Assert.AreEqual(0, value.Count());
        }

        [TestMethod()]
        public async Task SuccesfullyGetAllTwoUsers_ReturnOk200Code_AsyncTest()
        {
            // Arrangment
            var userController = new UsersController(_ctx);
            await userController.AddUser(_creatingUsers[0]);
            await userController.AddUser(_creatingUsers[1]);

            // Act
            var result = await userController.GetAllUsers();

            // Asserts
            var actualResult = result as OkObjectResult;
            Assert.IsNotNull(actualResult);
            var value = actualResult.Value as List<RequestUsers>;
            Assert.IsNotNull(value);
            Assert.AreEqual(200, actualResult.StatusCode);
            Assert.AreEqual(2, value.Count);
        }

        [TestMethod()]
        public async Task SuccesfullyGetExistingUser_ReturnOk200Code_AsyncTest()
        {
            // Arrangment
            var userController = new UsersController(_ctx);
            await userController.AddUser(_creatingUsers[0]);
            var user = _ctx.Users.ToList()[0];
            // Act
            var result = await userController.GetEditUser(user.ID);
            var actualResult = result as OkObjectResult;
            var value = actualResult.Value as RequestUsers;

            // Asserts
            Assert.IsNotNull(actualResult);
            Assert.AreEqual(200, actualResult.StatusCode);
            Assert.AreEqual(user.ID.ToString(), value.Id.ToString());
            Assert.AreEqual(user.Login, value.login);

        }

        [TestMethod()]
        public async Task FialGettingNotExistingUser_ReturnNotFound404Code_AsyncTest()
        {
            // Arrangment
            var userController = new UsersController(_ctx);
            await userController.AddUser(_creatingUsers[0]);
            await userController.AddUser(_creatingUsers[1]);
            // Act
            var result = await userController.GetEditUser(Guid.Empty);
            var actualResult = result as NotFoundResult;
            
            // Asserts
            Assert.IsNotNull(actualResult);
            Assert.AreEqual(404, actualResult.StatusCode);
        }

        [TestMethod()]
        public async Task SuccesfullyUpdateAllowedFieldOfExistingUser_ReturnOk200Code_AsyncTest()
        {
           // Arrangment
            var userController = new UsersController(_ctx);
            await userController.AddUser(_creatingUsers[0]);
            var user = _ctx.Users.First();

            Assert.AreEqual("log1", user.Login);

            var reqUser = new RequestUsers { fio ="A B C", Id = user.ID, login="changedLogin", role="admin" };
            // Act
            var result = await userController.UpdateUser(user.ID, reqUser);
            var actualResult = result as OkObjectResult;
            
            // Asserts
            Assert.IsNotNull(actualResult);
            Assert.AreEqual(200, actualResult.StatusCode);
            Assert.AreEqual(user.Person.post.Post, reqUser.role);
            Assert.AreEqual(user.Person.Surname, reqUser.fio.Split(' ')[0]);
            Assert.AreEqual(user.Person.Name, reqUser.fio.Split(' ')[1]);
            Assert.AreEqual(user.Person.LastName, reqUser.fio.Split(' ')[2]);
            Assert.AreEqual(user.Login, reqUser.login);
        }

        [TestMethod()]
        public async Task WillNotUpdateNotChangeableFieldsOfExsistingUser_ReturnOk200Code_AsyncTestAsync()
        {
            // Arrangment
            var userController = new UsersController(_ctx);
            await userController.AddUser(_creatingUsers[0]);
            var user = _ctx.Users.First();
            var oldGuid = user.ID;
            Assert.AreEqual("log1", user.Login);
            var fio = String.Join(' ', new String[3] { user.Person.Surname, user.Person.Name, user.Person.LastName });
            var reqUser = new RequestUsers { fio = fio, Id = Guid.NewGuid(), login = user.Login, role = user.Person.post.Post };
            
            // Act
            var result = await userController.UpdateUser(user.ID, reqUser);
            var actualResult = result as OkObjectResult;

            // Asserts
            Assert.IsNotNull(actualResult);
            Assert.AreEqual(200, actualResult.StatusCode);
            Assert.IsTrue(oldGuid.Equals(user.ID));
        }

        [TestMethod()]
        public async Task FailUpdateExistingUserWithLoginConflict_ReturnConflict409Code_AsyncTestAsync()
        {
            // Arrangment
            var userController = new UsersController(_ctx);
            await userController.AddUser(_creatingUsers[0]);
            await userController.AddUser(_creatingUsers[1]);
            var users = _ctx.Users.ToList();
            Assert.AreEqual("log1", users[0].Login);
            Assert.AreEqual("log2", users[1].Login);

            var fio = String.Join(' ', new String[3] { users[0].Person.Surname, users[0].Person.Name, users[0].Person.LastName });
            var reqUser = new RequestUsers { fio = fio, Id = users[0].ID, login = users[1].Login, role = users[0].Person.post.Post };

            // Act
            var result = await userController.UpdateUser(users[0].ID, reqUser);
            var actualResult = result as ConflictResult;
            
            // Asserts
            Assert.IsNotNull(actualResult);
            Assert.AreEqual(409, actualResult.StatusCode);
            Assert.AreEqual("log1", users[0].Login);
        }

        [TestMethod()]
        public async Task FailUpdateNotExistingUser_ReturnNotFound404Code_AsyncTestAsync()
        {
            // Arrangment
            var userController = new UsersController(_ctx);
            var reqUser = new RequestUsers { fio = "A B C", Id = Guid.NewGuid(), login = "changedLogin", role = "admin" };
            // Act
            var result = await userController.UpdateUser(Guid.Empty, reqUser);
            var actualResult = result as NotFoundResult;

            // Asserts
            Assert.IsNotNull(actualResult);
            Assert.AreEqual(404, actualResult.StatusCode);
            Assert.AreEqual(0, _ctx.Users.Count());
        }

        [TestMethod()]
        public async Task SuccesfullyDeleteUser_ReturnOk200Code_AsyncTestAsync()
        {
             // Arrangment
            var userController = new UsersController(_ctx);
            await userController.AddUser(_creatingUsers[0]);
            await userController.AddUser(_creatingUsers[1]);
            var users = _ctx.Users.ToList();
            
            // Act 1
            var result1 = await userController.DeleteUser(users[0].ID);
            var actualResult1 = result1 as OkObjectResult;
            
            // Asserts 1
            Assert.IsNotNull(actualResult1);
            Assert.AreEqual(200, actualResult1.StatusCode);
            Assert.AreEqual(1, _ctx.Users.Count());

            // Act 2
            var result2 = await userController.DeleteUser(users[1].ID);
            var actualResult2 = result1 as OkObjectResult;
            
            // Asserts 2
            Assert.IsNotNull(actualResult2);
            Assert.AreEqual(200, actualResult2.StatusCode);
            Assert.AreEqual(0, _ctx.Users.Count());
        }

        [TestMethod()]
        public async Task FailDeleteNotExistingUser_ReturnNotFound404Code_AsyncTestAsync()
        {
            // Arrangment
            var userController = new UsersController(_ctx);
            await userController.AddUser(_creatingUsers[0]);
            await userController.AddUser(_creatingUsers[1]);

            // Act 
            var result1 = await userController.DeleteUser(Guid.Empty);
            var actualResult1 = result1 as NotFoundResult;

            // Asserts 
            Assert.IsNotNull(actualResult1);
            Assert.AreEqual(404, actualResult1.StatusCode);
            Assert.AreEqual(2, _ctx.Users.Count());
        }
    }
}