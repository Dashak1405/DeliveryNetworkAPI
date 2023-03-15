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
using Microsoft.AspNetCore.Mvc;

namespace DeliveryNetworkAPI.Controllers.Tests
{
    [TestClass()]
    public class ProductsControllerTests
    {
        private DeliveryNetworkDbContext? _ctx;
        private List<Manufactor> _manufactors = new List<Manufactor> {
            new Manufactor { ID =  Guid.NewGuid(), Address = "Address 1", ManufactorName = "Manufactor 1", ProductID = null },
            new Manufactor { ID =  Guid.NewGuid(), Address = "Address 2", ManufactorName = "Manufactor 2", ProductID = null },
            new Manufactor { ID =  Guid.NewGuid(), Address = "Address 3", ManufactorName = "Manufactor 3", ProductID = null },
        };

        private List<Products> _products;

        [TestInitialize()]
        public async Task Initialize()
        {
            _ctx = DBContextMockHelper.InitDbContextWithPosts();
            var confBuilder = new ConfigurationBuilder().AddJsonFile($"appsettings.json", optional: false);
            var authConf = confBuilder.Build().GetSection("Auth");

            var posts = _ctx.Posts.ToList();
            Assert.AreEqual(3, posts.Count());
            Assert.AreEqual("user", posts[0].Post);
            Assert.AreEqual("admin", posts[1].Post);
            Assert.AreEqual("deliveryman", posts[2].Post);

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

            _ctx.SaveChanges();
        }

        [TestMethod()]
        public async Task GetAllProductsTest()
        {
            var prodController = new ProductsController(_ctx);

            var result = await prodController.GetAllProducts();
            var actualResult = result as OkObjectResult;
            
            Assert.IsNotNull(actualResult);
            Assert.AreEqual(200, actualResult.StatusCode);

            var list = actualResult.Value as List<RequestProducts>;
            Assert.IsNotNull(list);

            Assert.AreEqual(3, list.Count());
            Assert.AreEqual("Product 1", list[0].ProductName);
            Assert.AreEqual("Product 2", list[1].ProductName);
            Assert.AreEqual("Product 3", list[2].ProductName);
        }
    }
}
