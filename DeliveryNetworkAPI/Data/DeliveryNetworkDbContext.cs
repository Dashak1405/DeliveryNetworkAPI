using DeliveryNetworkAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DeliveryNetworkAPI.Data
{
    public class DeliveryNetworkDbContext : DbContext
    {
        public DeliveryNetworkDbContext(DbContextOptions options) : base(options)
        {

        }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<Status> Status { get; set; }
        public virtual DbSet<Products> Products { get; set; }
        public virtual DbSet<Posts> Posts { get; set; }
        public virtual DbSet<Persons> Persons { get; set; }
        public virtual DbSet<Orders> Orders { get; set; }
        public virtual DbSet<Manufactor> Manufactors { get; set; }
        public virtual DbSet<Delivery> Delivery { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<CompanyInformation> companyInformation { get; set; }
    }
}
