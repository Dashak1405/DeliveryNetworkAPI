﻿// <auto-generated />
using System;
using DeliveryNetworkAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DeliveryNetworkAPI.Migrations
{
    [DbContext(typeof(DeliveryNetworkDbContext))]
    [Migration("20221212081005_Migration3")]
    partial class Migration3
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("DeliveryNetworkAPI.Models.CompanyInformation", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CompanyName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("NumOfEmployees")
                        .HasColumnType("int");

                    b.Property<int>("NumOfOrders")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.ToTable("companyInformation");
                });

            modelBuilder.Entity("DeliveryNetworkAPI.Models.Customer", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserIDID")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("ID");

                    b.HasIndex("UserIDID");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("DeliveryNetworkAPI.Models.Delivery", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateOfDelivery")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("ProductID")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("ID");

                    b.ToTable("Delivery");
                });

            modelBuilder.Entity("DeliveryNetworkAPI.Models.Manufactor", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ManufactorName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("ProductID")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("ID");

                    b.ToTable("Manufactors");
                });

            modelBuilder.Entity("DeliveryNetworkAPI.Models.Orders", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("CustomerID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DateOfEndOrder")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateOfStartOrder")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("DeliveryID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ExecutorID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("StatusID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("allProducts")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("CustomerID");

                    b.HasIndex("DeliveryID");

                    b.HasIndex("ExecutorID");

                    b.HasIndex("StatusID");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("DeliveryNetworkAPI.Models.Persons", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PassportID")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("postID")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("ID");

                    b.HasIndex("postID");

                    b.ToTable("Persons");
                });

            modelBuilder.Entity("DeliveryNetworkAPI.Models.Posts", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Post")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("DeliveryNetworkAPI.Models.Products", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Count")
                        .HasColumnType("int");

                    b.Property<Guid>("ManufactorID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("OrdersID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ProductName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("ManufactorID");

                    b.HasIndex("OrdersID");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("DeliveryNetworkAPI.Models.Status", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("Status");
                });

            modelBuilder.Entity("DeliveryNetworkAPI.Models.Users", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("PersonID")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("ID");

                    b.HasIndex("PersonID");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("DeliveryNetworkAPI.Models.Customer", b =>
                {
                    b.HasOne("DeliveryNetworkAPI.Models.Users", "UserID")
                        .WithMany()
                        .HasForeignKey("UserIDID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("UserID");
                });

            modelBuilder.Entity("DeliveryNetworkAPI.Models.Orders", b =>
                {
                    b.HasOne("DeliveryNetworkAPI.Models.Customer", "Customer")
                        .WithMany()
                        .HasForeignKey("CustomerID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DeliveryNetworkAPI.Models.Delivery", "Delivery")
                        .WithMany()
                        .HasForeignKey("DeliveryID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DeliveryNetworkAPI.Models.Users", "Executor")
                        .WithMany()
                        .HasForeignKey("ExecutorID");

                    b.HasOne("DeliveryNetworkAPI.Models.Status", "Status")
                        .WithMany()
                        .HasForeignKey("StatusID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Customer");

                    b.Navigation("Delivery");

                    b.Navigation("Executor");

                    b.Navigation("Status");
                });

            modelBuilder.Entity("DeliveryNetworkAPI.Models.Persons", b =>
                {
                    b.HasOne("DeliveryNetworkAPI.Models.Posts", "post")
                        .WithMany()
                        .HasForeignKey("postID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("post");
                });

            modelBuilder.Entity("DeliveryNetworkAPI.Models.Products", b =>
                {
                    b.HasOne("DeliveryNetworkAPI.Models.Manufactor", "Manufactor")
                        .WithMany()
                        .HasForeignKey("ManufactorID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DeliveryNetworkAPI.Models.Orders", null)
                        .WithMany("Products")
                        .HasForeignKey("OrdersID");

                    b.Navigation("Manufactor");
                });

            modelBuilder.Entity("DeliveryNetworkAPI.Models.Users", b =>
                {
                    b.HasOne("DeliveryNetworkAPI.Models.Persons", "Person")
                        .WithMany()
                        .HasForeignKey("PersonID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Person");
                });

            modelBuilder.Entity("DeliveryNetworkAPI.Models.Orders", b =>
                {
                    b.Navigation("Products");
                });
#pragma warning restore 612, 618
        }
    }
}
