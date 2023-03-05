using DeliveryNetworkAPI.Data;
using DeliveryNetworkAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace DeliveryNetworkAPITests
{
    public static class DBContextMockHelper
    {
        public static DeliveryNetworkDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<DeliveryNetworkDbContext>()
                            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                            .Options;
            return new DeliveryNetworkDbContext(options);
        }
        public static Mock<DeliveryNetworkDbContext> GetMockDbContext() {
            var options = new DbContextOptionsBuilder<DeliveryNetworkDbContext>()
                            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                            .Options;
            return new Mock<DeliveryNetworkDbContext>(options);
        }

        public static DbContextOptions GetDBOptions()
        {
            return new DbContextOptionsBuilder<DeliveryNetworkDbContext>()
                            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                            .Options;
        }

        
        public static Mock<DbSet<T>> GetQueryableMockDbSet<T>(params T[] sourceList) where T : class
        {
            var queryable = sourceList.AsQueryable();
            var dbSet = new Mock<DbSet<T>>();
            dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

            return dbSet;
        }

        public static Mock<DbSet<T>> GetQueryableMockEmptyDbSet<T>() where T : class
        {
            var queryable = new List<T> { }.AsQueryable();

            var dbSet = new Mock<DbSet<T>>();
            dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

            return dbSet;
        }

        public static Mock<DbSet<Posts>> GetMockPosts(Mock<DeliveryNetworkDbContext> mockContext, List<Posts> posts) 
        {
            var mockPosts = GetQueryableMockDbSet(
                posts[0], posts[1], posts[2]
            );
            mockContext.Setup(m => m.Posts).Returns(mockPosts.Object);
            return mockPosts;
        }

        public static List<Posts> GetPosts() {
            var posts = new List<Posts> {
                new Posts { ID = Guid.Parse("719D0342-155A-4A70-BF59-3DD53AD9F58E"), Post = Posts.USER }
              , new Posts { ID = Guid.Parse("2C6515CC-2A4E-4CD3-A067-54E51974D7D0"), Post = Posts.ADMIN }
              , new Posts { ID = Guid.Parse("95579B5D-839A-4C12-A33F-EF5236888431"), Post = Posts.DELIVERYMAN }
            };
            return posts;
        }

        /** Инифициализация фиктивного списка должностей в текущем контексте баз данных
         */
        public static void  InitMockPosts(Mock<DeliveryNetworkDbContext> mctx) {
            var mSet = GetMockPosts(mctx, GetPosts());
            mctx.Object.Posts = mSet.Object;
        }

        public static DeliveryNetworkDbContext InitDbContextWithPosts() {
            var ctx = GetDbContext();
            foreach (var p in GetPosts()) {
                ctx.Posts.Add(p);
            }
            ctx.SaveChanges();
            return ctx;
        }
    }
}
