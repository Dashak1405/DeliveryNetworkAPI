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
using DeliveryNetworkAPI.Models;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;

namespace DeliveryNetworkAPI.Controllers.Tests
{
    [TestClass()]
    public class OrdersControllerTests
    {
        private DeliveryNetworkDbContext _ctx;
        private List<CreateUserClass> _creatingUsers = new List<CreateUserClass> {
            new CreateUserClass { login = "simpleUser", password = "password1", name = "name1", surname = "surname1", lastname = "lastname1", passport = "passport1" }
          , new CreateUserClass { login = "deliveryUser", password = "password2", name = "name2", surname = "surname2", lastname = "lastname2", passport = "passport2" }
          , new CreateUserClass { login = "adminUser", password = "password3", name = "name3", surname = "surname3", lastname = "lastname3", passport = "passport3" }
          , new CreateUserClass { login = "simpleUser2", password = "password4", name = "name4", surname = "surname4", lastname = "lastname4", passport = "passport4" }
        };
        private List<Manufactor> _manufactors = new List<Manufactor> {
            new Manufactor { ID =  Guid.NewGuid(), Address = "Address 1", ManufactorName = "Manufactor 1", ProductID = null }
          , new Manufactor { ID =  Guid.NewGuid(), Address = "Address 2", ManufactorName = "Manufactor 2", ProductID = null }
          , new Manufactor { ID =  Guid.NewGuid(), Address = "Address 3", ManufactorName = "Manufactor 3", ProductID = null }
        };
        private List<Status> _statuses = new List<Status> {
            new Status { ID = Guid.Parse("1F5F5982-C3AC-4E24-8D77-08C841D62796"), status = Status.IN_PROGRESS }
          , new Status { ID = Guid.Parse("6A36632F-6DA8-4A40-AA57-1688D9F2DF6F"), status = Status.COMPLETED }
          , new Status { ID = Guid.Parse("0087713E-1BE2-4ACF-9C77-D898EAD9841E"), status = Status.NEW }
        };
        private List<Products> _products;

        private string MakePersonFullName(Persons p)
        {
            return p.Surname + ' ' + p.Name + ' ' + p.LastName;
        }

        [TestInitialize()]
        public async Task Initialize()
        {
            _ctx = DBContextMockHelper.InitDbContextWithPosts();

            var posts = _ctx.Posts.ToList();
            Assert.AreEqual(3, posts.Count());
            Assert.AreEqual("user", posts[0].Post);
            Assert.AreEqual("admin", posts[1].Post);
            Assert.AreEqual("deliveryman", posts[2].Post);

            var userController = new UsersController(_ctx);
            await userController.AddUser(_creatingUsers[0]);
            await userController.AddUser(_creatingUsers[1]);
            await userController.AddUser(_creatingUsers[2]);
            await userController.AddUser(_creatingUsers[3]);

            var users = _ctx.Users.ToList();
            Assert.IsNotNull(users);

            var admin = users[1];
            var adminReq = new RequestUsers
            {
                Id = admin.ID,
                role = "admin",
                login = admin.Login,
                fio = MakePersonFullName(admin.Person)
            };
            await userController.UpdateUser(admin.ID, adminReq);
            var deliveryman = users[2];
            var deliverymanReq = new RequestUsers
            {
                Id = deliveryman.ID,
                role = "deliveryman",
                login = deliveryman.Login,
                fio = MakePersonFullName(deliveryman.Person)
            };
            await userController.UpdateUser(deliveryman.ID, deliverymanReq);

            _products = new List<Products> {
                new Products { ID =  Guid.NewGuid(), Count = 0, ProductName =  "Product 1", Manufactor = _manufactors[0] },
                new Products { ID =  Guid.NewGuid(), Count = 10, ProductName =  "Product 2", Manufactor = _manufactors[1] },
                new Products { ID =  Guid.NewGuid(), Count = 20, ProductName =  "Product 3", Manufactor = _manufactors[2] },
            };

            _ctx.Manufactors.Add(_manufactors[0]);
            _ctx.Manufactors.Add(_manufactors[1]);
            _ctx.Manufactors.Add(_manufactors[2]);

            _ctx.Products.Add(_products[0]);
            _ctx.Products.Add(_products[1]);
            _ctx.Products.Add(_products[2]);

            _ctx.Status.Add(_statuses[0]);
            _ctx.Status.Add(_statuses[1]);
            _ctx.Status.Add(_statuses[2]);


            _ctx.SaveChanges();
        }

        [TestMethod()]
        public async Task SuccessfulyAddOrderForAuthorizedUser_ReturnOk200Code()
        {

            var createOrder = new CreateOrderClass
            {
                address = "Addres 1",
                products = "Product 1;Product 2",
                status = "",
                dateOfStart = "",
                dateOfEnd = "",
                customer = "simpleUser",
                executor = ""
            };
            var orderController = new OrdersController(_ctx);
            var dateStart = DateTime.Now;
            var result = await orderController.AddOrder(createOrder);

            var actualResult = result as OkObjectResult;
            Assert.IsNotNull(actualResult);
            var order = actualResult.Value as Orders;
            Assert.IsNotNull(order);
            Assert.AreEqual(createOrder.address, order.Address);
            Assert.IsTrue(
                dateStart < order.DateOfStartOrder &&
                order.DateOfStartOrder < DateTime.Now
            );
        }

        [TestMethod()]
        public async Task FailedAddOrderForAnonimUser_ReturnForbidden403Code()
        {
            // Подготовка
            var createOrder = new CreateOrderClass
            {
                address = "Addres 1",
                products = "Product 1;Product 2",
                status = "",
                dateOfStart = "",
                dateOfEnd = "",
                customer = "",
                executor = ""
            };
            var orderController = new OrdersController(_ctx);
            var result = await orderController.AddOrder(createOrder);
            var actualResult = result as ForbidResult;
            Assert.IsNotNull(actualResult);
        }

        [TestMethod()]
        public async Task FailedAddOrderForNotExistentsUser_ReturnForbidden403Code()
        {

            var createOrder = new CreateOrderClass
            {
                address = "Addres 1",
                products = "Product 1;Product 2",
                status = "",
                dateOfStart = "",
                dateOfEnd = "",
                customer = "WrongUser",
                executor = ""
            };
            var orderController = new OrdersController(_ctx);
            var result = await orderController.AddOrder(createOrder);
            var actualResult = result as ForbidResult;
            Assert.IsNotNull(actualResult);
        }

        [TestMethod()]
        public async Task SuccessfullyListAllOrders_ReturnOk200CodeWithList()
        {
            var createOrders = new List<CreateOrderClass>
            {
                new CreateOrderClass
                {
                    address = "Addres 1",
                    products = "Product 1;Product 2",
                    status = "",
                    dateOfStart = "",
                    dateOfEnd = "",
                    customer = "simpleUser",
                    executor = ""
                },
                new CreateOrderClass
                {
                    address = "Addres 1",
                    products = "Product 3",
                    status = "",
                    dateOfStart = "",
                    dateOfEnd = "",
                    customer = "simpleUser",
                    executor = ""
                },
                new CreateOrderClass
                {
                    address = "Addres 2",
                    products = "Product 3;Product 2",
                    status = "",
                    dateOfStart = "",
                    dateOfEnd = "",
                    customer = "simpleUser2",
                    executor = ""
                }
            };

            var orderController = new OrdersController(_ctx);
            
            foreach (var createOrder in createOrders) {
                await orderController.AddOrder(createOrder);
            }

            var result = await orderController.GetAllOrders();

            var actualResult = result as OkObjectResult;
            Assert.IsNotNull(actualResult);
            var orederResultlist = actualResult.Value as List<RequestOrders>;
            Assert.IsNotNull(orederResultlist);
            Assert.AreEqual(3, orederResultlist.Count());
            for (var i = 0; i < orederResultlist.Count(); i++)
            {
                Assert.AreEqual(orederResultlist[i].Address, createOrders[i].address);
                Assert.AreEqual(orederResultlist[i].Products, createOrders[i].products);
            }
        }


        [TestMethod()]
        public async Task SuccessfullyListOrdersOfUser_ReturnOk200CodeWithList()
        {
            var createOrders = new List<CreateOrderClass>
            {
                new CreateOrderClass
                {
                    address = "Addres 1",
                    products = "Product 1;Product 2",
                    status = "",
                    dateOfStart = "",
                    dateOfEnd = "",
                    customer = "simpleUser",
                    executor = ""
                },
                new CreateOrderClass
                {
                    address = "Addres 1",
                    products = "Product 3",
                    status = "",
                    dateOfStart = "",
                    dateOfEnd = "",
                    customer = "simpleUser",
                    executor = ""
                },
                new CreateOrderClass
                {
                    address = "Addres 2",
                    products = "Product 3;Product 2",
                    status = "",
                    dateOfStart = "",
                    dateOfEnd = "",
                    customer = "simpleUser2",
                    executor = ""
                }
            };
            var orderController = new OrdersController(_ctx);
            var user = _ctx.Users.First(u => u.Login == "simpleUser");
            Assert.IsNotNull(user);
            await orderController.AddOrder(createOrders[0]);
            await orderController.AddOrder(createOrders[1]);

            var result = await orderController.GetOrdersForUser(user.ID);

            var actualResult = result as OkObjectResult;
            Assert.IsNotNull(actualResult);
            var list = actualResult.Value as List<RequestOrders>;
            Assert.IsNotNull(list);
            Assert.AreEqual(2, list.Count());
            Assert.AreEqual(list[0].Customer, MakePersonFullName(user.Person));
            Assert.AreEqual(list[1].Customer, MakePersonFullName(user.Person));
        }

        [TestMethod()]
        public async Task SuccessfullyListOrdersOfDeliveryman_ReturnOk200CodeWithList()
        {
            var orderController = new OrdersController(_ctx);
            var deliveryman = _ctx.Users.First(u => u.Person.post.Post == Posts.DELIVERYMAN);
            var users = _ctx.Users.Where(u => u.Person.post.Post == Posts.USER).ToList();
            var customer1 = new Customer { ID = Guid.NewGuid(), Address = "Address 1", UserID = users[0] };
            var customer2 = new Customer { ID = Guid.NewGuid(), Address = "Address 2", UserID = users[1] };


            var orders = new List<Orders>
            {
                new Orders
                {
                    Executor = deliveryman,
                    ExecutorID = deliveryman.ID,
                    Customer = customer1,
                    Address = "OrderAddress1",
                    allProducts = "Product 2;Product 3",
                    Products = new List<Products> { _products[1], _products[2] },
                    Status = _ctx.Status.First(s => s.status == Status.NEW),
                    DateOfStartOrder = DateTime.Now,
                    ID = Guid.NewGuid(),
                    Delivery = new Delivery { Address = "DeliveryAddress1", ID = Guid.NewGuid() }
                },
                new Orders
                {
                    Executor = deliveryman,
                    ExecutorID = deliveryman.ID,
                    Customer = customer2,
                    Address = "OrderAddress1",
                    allProducts = "Product 2;Product 1",
                    Products = new List<Products> { _products[1], _products[0] },
                    Status = _ctx.Status.First(s => s.status == Status.IN_PROGRESS),
                    DateOfStartOrder = DateTime.Now,
                    ID = Guid.NewGuid(),
                    Delivery = new Delivery { Address = "DeliveryAddress1", ID = Guid.NewGuid() }
                }
            };
            foreach (var order in orders)
            {
                _ctx.Orders.Add(order);
            }
            _ctx.SaveChanges();

            var result = await orderController.GetOrdersForDeliveryMan(deliveryman.ID);

            var actualResult = result as OkObjectResult;
            Assert.IsNotNull(actualResult);
            Assert.AreEqual(200, actualResult.StatusCode);
            
            var list = actualResult.Value as List<RequestOrders>;
            Assert.IsNotNull(list);
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(list[0].Executor, deliveryman.Login);
            Assert.AreEqual(list[1].Executor, deliveryman.Login);
        }

        [TestMethod()]
        public async Task SuccessfullyAddExecturoToOrder_ReturnOk200Code()
        {
            var deliveryman = _ctx.Users.First(u => u.Person.post.Post == Posts.DELIVERYMAN);
            Assert.IsNotNull(deliveryman);

            var users = _ctx.Users.Where(u => u.Person.post.Post == Posts.USER).ToList();
            var customer = new Customer { ID = Guid.NewGuid(), Address = "Address 1", UserID = users[0] };

            var order = new Orders
            {
                ID = Guid.NewGuid(),
                Address = "OrderAddress1",
                allProducts = "Product 2;Product 3",
                Products = new List<Products> { _products[1], _products[2] },
                Status = _ctx.Status.First(s => s.status == Status.NEW),
                Customer = customer,
                Delivery = new Delivery { Address = "DeliveryAddress1", ID = Guid.NewGuid() }
            };

            _ctx.Orders.Add(order);
            _ctx.SaveChanges();
            var reqOrder = new RequestOrders { Executor = deliveryman.Login };
            var orderController = new OrdersController(_ctx);
            var result = await orderController.AddExecutor(order.ID, reqOrder);

            var actualResult = result as OkObjectResult;
            Assert.IsNotNull(actualResult);
            Assert.AreEqual(200, actualResult.StatusCode);
            Assert.AreEqual(Status.IN_PROGRESS, order.Status.status);
            Assert.AreSame(order.Executor, deliveryman);
        }

        [TestMethod()]
        public async Task SuccessfullyGetAddresForDeliveryman_ReturnOk200Code()
        {
            var deliveryman = _ctx.Users.First(u => u.Person.post.Post == Posts.DELIVERYMAN);
            Assert.IsNotNull(deliveryman);

            var users = _ctx.Users.Where(u => u.Person.post.Post == Posts.USER).ToList();
            var customer = new Customer { ID = Guid.NewGuid(), Address = "Address 1", UserID = users[0] };

            var order = new Orders
            {
                ID = Guid.NewGuid(),
                Address = "OrderAddress1",
                allProducts = "Product 2;Product 3",
                Status = _ctx.Status.First(s => s.status == Status.NEW),
                Products = new List<Products> { _products[1], _products[2] },
                Customer = customer,
                Delivery = new Delivery { Address = "DeliveryAddress1", ID = Guid.NewGuid() },
            };

            _ctx.Orders.Add(order);
            _ctx.SaveChanges();
            var reqOrder = new RequestOrders { 
                ID = Guid.NewGuid(),    
                Executor = deliveryman.Login, 
                Customer = customer.ID.ToString(), 
                Address = "RequestOrderAddress",  
                Status = Status.IN_PROGRESS,
                Products = String.Join(';', _products[1], _products[2])
            };
            var orderController = new OrdersController(_ctx);

            await orderController.AddExecutor(order.ID, reqOrder);

            var result = await orderController.GetAddressForDeliveryMan(deliveryman.ID);

            var actualResult = result as OkObjectResult;
            Assert.IsNotNull(actualResult);
            Assert.AreEqual(200, actualResult.StatusCode);
            
            List<string> value = actualResult.Value as List<string>;
            Assert.IsNotNull(value);
            Assert.AreEqual(1, value.Count);
            Assert.AreEqual(order.Address, value[0]);
        }

        [TestMethod()]
        public async Task SuccessfultCompleteOrder_Return200OkCode()
        {
            var deliveryman = _ctx.Users.First(u => u.Person.post.Post == Posts.DELIVERYMAN);
            Assert.IsNotNull(deliveryman);

            var users = _ctx.Users.Where(u => u.Person.post.Post == Posts.USER).ToList();
            var customer = new Customer { ID = Guid.NewGuid(), Address = "Address 1", UserID = users[0] };

            var order = new Orders
            {
                ID = Guid.NewGuid(),
                Address = "OrderAddress1",
                allProducts = "Product 2;Product 3",
                Status = _ctx.Status.First(s => s.status == Status.NEW),
                Products = new List<Products> { _products[1], _products[2] },
                Customer = customer,
                Delivery = new Delivery { Address = "DeliveryAddress1", ID = Guid.NewGuid() }
            };

            _ctx.Orders.Add(order);
            _ctx.SaveChanges();
            var reqOrder = new RequestOrders { };
            var orderController = new OrdersController(_ctx);
            await orderController.AddExecutor(order.ID, reqOrder);

            var result = await orderController.CompleteOrder(order.ID, reqOrder);

            var actualResult = result as OkObjectResult;
            Assert.IsNotNull(actualResult);
            Assert.AreEqual(200, actualResult.StatusCode);
            Assert.AreEqual(Status.COMPLETED, order.Status.status);
        }
    }
}
