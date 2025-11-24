using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Repository.Entities.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Context
{
    public class EVSDbContext : DbContext
    {
        public EVSDbContext() { }
        public EVSDbContext(DbContextOptions<EVSDbContext> options) : base(options) { }
        public DbSet<Car> Cars { get; set; }
        public DbSet<CarDeliveryHistory> CarDeliveryHistories { get; set; }
        public DbSet<CarReturnHistory> CarReturnHistories { get; set; }
        public DbSet<CitizenId> CitizenIds { get; set; }
        public DbSet<DriverLicense> DriverLicenses { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<RentalOrder> RentalOrders { get; set; }
        public DbSet<RentalLocation> RentalLocations { get; set; }
        public DbSet<User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Car
            modelBuilder.Entity<Car>(entity =>
            {
                entity.HasOne(e => e.RentalLocation)
                    .WithMany()
                    .HasForeignKey(e => e.RentalLocationId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.RentalOrders)
                    .WithOne(e => e.Car)
                    .HasForeignKey(e => e.CarId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasData(
                    new Car
                    {
                        Id = 1,
                        Model = "Tesla Model 3",
                        Name = "Model 3",
                        Seats = 5,
                        SizeType = "Sedan",
                        TrunkCapacity = 425,
                        BatteryType = "Lithium-Ion",
                        DepositPercent = 20,
                        BatteryDuration = 350,
                        RentPricePerDay = 1000000,
                        RentPricePerDayWithDriver = 1400000,
                        ImageUrl = "https://example.com/tesla_model_3.jpg",
                        ImageUrl2 = "https://example.com/tesla_model_3.jpg",
                        ImageUrl3 = "https://example.com/tesla_model_3.jpg",
                        CreatedAt = new DateTime(2025, 10, 11),
                        UpdatedAt = null,
                        IsActive = true,
                        IsDeleted = false,
                        RentalLocationId = 1
                    },
                    new Car
                    {
                        Id = 2,
                        Model = "Nissan Leaf",
                        Name = "Leaf",
                        Seats = 5,
                        SizeType = "Hatchback",
                        TrunkCapacity = 435,
                        BatteryType = "Lithium-Ion",
                        DepositPercent = 20,
                        BatteryDuration = 240,
                        RentPricePerDay = 800000,
                        RentPricePerDayWithDriver = 1200000,
                        ImageUrl = "https://example.com/nissan_leaf.jpg",
                        ImageUrl2 = "https://example.com/nissan_leaf.jpg",
                        ImageUrl3 = "https://example.com/nissan_leaf.jpg",
                        CreatedAt = new DateTime(2025, 10, 11),
                        UpdatedAt = null,
                        IsActive = true,
                        IsDeleted = false,
                        RentalLocationId = 2
                    },
                    new Car
                    {
                        Id = 3,
                        Model = "Chevrolet Bolt EV",
                        Name = "Bolt EV",
                        Seats = 5,
                        SizeType = "Hatchback",
                        TrunkCapacity = 478,
                        BatteryType = "Lithium-Ion",
                        DepositPercent = 20,
                        BatteryDuration = 259,
                        RentPricePerDay = 900000,
                        RentPricePerDayWithDriver = 1300000,
                        ImageUrl = "https://example.com/chevrolet_bolt_ev.jpg",
                        ImageUrl2 = "https://example.com/chevrolet_bolt_ev.jpg",
                        ImageUrl3 = "https://example.com/chevrolet_bolt_ev.jpg",
                        CreatedAt = new DateTime(2025, 10, 11),
                        UpdatedAt = null,
                        IsActive = true,
                        IsDeleted = false,
                        RentalLocationId = 3
                    },
                    new Car
                    {
                        Id = 4,
                        Model = "BMW i3",
                        Name = "i3",
                        Seats = 4,
                        SizeType = "Hatchback",
                        TrunkCapacity = 260,
                        BatteryType = "Lithium-Ion",
                        DepositPercent = 10,
                        BatteryDuration = 153,
                        RentPricePerDay = 1100000,
                        RentPricePerDayWithDriver = 1500000,
                        ImageUrl = "https://example.com/bmw_i3.jpg",
                        ImageUrl2 = "https://example.com/bmw_i3.jpg",
                        ImageUrl3 = "https://example.com/bmw_i3.jpg",
                        CreatedAt = new DateTime(2025, 10, 11),
                        UpdatedAt = null,
                        IsActive = true,
                        IsDeleted = false,
                        RentalLocationId = 3
                    },
                    new Car
                    {
                        Id = 5,
                        Model = "Audi e-tron",
                        Name = "e-tron",
                        Seats = 5,
                        SizeType = "SUV",
                        TrunkCapacity = 660,
                        BatteryType = "Lithium-Ion",
                        DepositPercent = 15,
                        BatteryDuration = 222,
                        RentPricePerDay = 1500000,
                        RentPricePerDayWithDriver = 2000000,
                        ImageUrl = "https://example.com/audi_e_tron.jpg",
                        ImageUrl2 = "https://example.com/audi_e_tron.jpg",
                        ImageUrl3 = "https://example.com/audi_e_tron.jpg",
                        CreatedAt = new DateTime(2025, 10, 11),
                        UpdatedAt = null,
                        IsActive = true,
                        IsDeleted = false,
                        RentalLocationId = 4
                    }
                );
            });

            // Configure CarDeliveryHistory
            modelBuilder.Entity<CarDeliveryHistory>(entity =>
            {
                entity.HasOne(e => e.Order)
                    .WithMany()
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Car)
                    .WithMany()
                    .HasForeignKey(e => e.CarId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure CarReturnHistory
            modelBuilder.Entity<CarReturnHistory>(entity =>
            {
                entity.HasOne(e => e.Order)
                    .WithMany()
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Car)
                    .WithMany()
                    .HasForeignKey(e => e.CarId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure CitizenId
            modelBuilder.Entity<CitizenId>(entity =>
            {
                entity.HasOne(e => e.User)
                    .WithOne(e => e.CitizenIdNavigation)
                    .HasForeignKey<CitizenId>(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            // Configure DriverLicense
            modelBuilder.Entity<DriverLicense>(entity =>
                {
                    entity.HasOne(e => e.User)
                        .WithOne(e => e.DriverLicense)
                        .HasForeignKey<DriverLicense>(e => e.UserId)
                        .OnDelete(DeleteBehavior.Cascade);
                });

                // Configure Feedback
                modelBuilder.Entity<Feedback>(entity =>
                {
                    entity.HasOne(e => e.User)
                        .WithMany(e => e.Feedback)
                        .HasForeignKey(e => e.UserId)
                        .OnDelete(DeleteBehavior.Cascade);

                    entity.HasOne(e => e.RentalOrder)
                        .WithMany()
                        .HasForeignKey(e => e.RentalOrderId)
                        .OnDelete(DeleteBehavior.Restrict);
                });

                // Configure Payment
                modelBuilder.Entity<Payment>(entity =>
                {
                    entity.HasOne(e => e.User)
                        .WithMany(e => e.Payments)
                        .HasForeignKey(e => e.UserId)
                        .OnDelete(DeleteBehavior.Cascade);
                    entity.HasOne(e => e.RentalOrder)
                        .WithMany(e => e.Payments)
                        .HasForeignKey(e => e.RentalOrderId)
                        .OnDelete(DeleteBehavior.Cascade);
                });

                // Configure RentalLocation
                modelBuilder.Entity<RentalLocation>(entity =>
                {
                    entity.HasMany(e => e.Cars)
                        .WithOne(e => e.RentalLocation)
                        .HasForeignKey(e => e.RentalLocationId)
                        .OnDelete(DeleteBehavior.Cascade);

                    entity.HasMany(e => e.RentalOrders)
                        .WithOne(e => e.RentalLocation)
                        .HasForeignKey(e => e.RentalLocationId)
                        .OnDelete(DeleteBehavior.Cascade);

                    entity.HasMany(e => e.Users)
                        .WithOne(e => e.RentalLocation)
                        .HasForeignKey(e => e.RentalLocationId)
                        .OnDelete(DeleteBehavior.Restrict);

                    entity.HasData(
                        new RentalLocation
                        {
                            Id = 1,
                            Name = "EVStation Nguyễn Văn Tăng",
                            Address = "209 Nguyễn Văn Tăng, Long Thạnh Mỹ, Thủ Đức, TP.HCM",
                            Coordinates = "10.84274, 106.8198",
                            CreatedAt = new DateTime(2025, 10, 11),
                            UpdatedAt = null,
                            IsActive = true,
                            IsDeleted = false
                        },
                        new RentalLocation
                        {
                            Id = 2,
                            Name = "EVStation Lê Văn Việt",
                            Address = "447 Lê Văn Việt, Thủ Đức, TP.HCM",
                            Coordinates = "10.84529, 106.7933",
                            CreatedAt = new DateTime(2025, 10, 11),
                            UpdatedAt = null,
                            IsActive = true,
                            IsDeleted = false
                        },
                        new RentalLocation
                        {
                            Id = 3,
                            Name = "EVStation Kha Vạn Cân",
                            Address = "39634 Kha Vạn Cân, Linh Chiểu, Thủ Đức, TP.HCM",
                            Coordinates = "10.856468, 106.756518",
                            CreatedAt = new DateTime(2025, 10, 11),
                            UpdatedAt = null,
                            IsActive = true,
                            IsDeleted = false
                        },
                        new RentalLocation
                        {
                            Id = 4,
                            Name = "EVStation Võ Văn Ngân",
                            Address = "190 Võ Văn Ngân, Bình Thọ, Thủ Đức, TP.HCM",
                            Coordinates = "10.850805, 106.763773",
                            CreatedAt = new DateTime(2025, 10, 11),
                            UpdatedAt = null,
                            IsActive = true,
                            IsDeleted = false
                        }
                        );
                });

                // Configure RentalOrder
                modelBuilder.Entity<RentalOrder>(entity =>
                {
                    entity.HasOne(e => e.User)
                        .WithMany(e => e.RentalOrders)
                        .HasForeignKey(e => e.UserId)
                        .OnDelete(DeleteBehavior.Restrict);

                    entity.HasOne(e => e.Car)
                        .WithMany(e => e.RentalOrders)
                        .HasForeignKey(e => e.CarId)
                        .OnDelete(DeleteBehavior.Restrict);

                    entity.HasMany(e => e.Payments)
                        .WithOne(e => e.RentalOrder)
                        .HasForeignKey(e => e.RentalOrderId)
                        .OnDelete(DeleteBehavior.Cascade);

                    entity.HasOne(e => e.RentalLocation)
                        .WithMany(e => e.RentalOrders)
                        .HasForeignKey(e => e.RentalLocationId)
                        .OnDelete(DeleteBehavior.Restrict);
                });

                // Configure User
                modelBuilder.Entity<User>(entity =>
                {

                    entity.HasOne(e => e.CitizenIdNavigation)
                        .WithOne(e => e.User)
                        .HasForeignKey<User>(e => e.CitizenId)
                        .OnDelete(DeleteBehavior.Cascade);

                    entity.HasOne(e => e.DriverLicense)
                        .WithOne(e => e.User)
                        .HasForeignKey<User>(e => e.DriverLicenseId)
                        .OnDelete(DeleteBehavior.Cascade);

                    entity.HasOne(e => e.RentalLocation)
                        .WithMany(e => e.Users)
                        .HasForeignKey(e => e.RentalLocationId)
                        .OnDelete(DeleteBehavior.Cascade);

                    entity.HasMany(e => e.Feedback)
                        .WithOne(e => e.User)
                        .HasForeignKey(e => e.UserId)
                        .OnDelete(DeleteBehavior.Cascade);

                    entity.HasMany(e => e.RentalOrders)
                        .WithOne(e => e.User)
                        .HasForeignKey(e => e.UserId)
                        .OnDelete(DeleteBehavior.Restrict);

                    entity.HasMany(e => e.Payments)
                        .WithOne(e => e.User)
                        .HasForeignKey(e => e.UserId)
                        .OnDelete(DeleteBehavior.Cascade);

                    entity.HasData(
                new User
                {
                    Id = 1,
                    Email = "admin@gmail.com",
                    Password = "1",
                    PasswordHash = "$2a$12$z.y2vdQFkt/drkj6yzAXm.6v/rirvWIaw1tXyIgvR7dki1roEfLXm",
                    FullName = "Admin User",
                    PhoneNumber = "0123456789",
                    Role = "Admin",
                    ConfirmEmailToken = null,
                    IsEmailConfirmed = true,
                    CreatedAt = new DateTime(2025, 10, 11),
                    UpdatedAt = null,
                    IsActive = true,
                    RentalLocationId = null
                },
                new User
                {
                    Id = 2,
                    Email = "staff@gmail.com",
                    Password = "1",
                    PasswordHash = "$2a$12$z.y2vdQFkt/drkj6yzAXm.6v/rirvWIaw1tXyIgvR7dki1roEfLXm",
                    FullName = "Staff User",
                    PhoneNumber = "0123456789",
                    Role = "Staff",
                    ConfirmEmailToken = null,
                    IsEmailConfirmed = true,
                    CreatedAt = new DateTime(2025, 10, 11),
                    UpdatedAt = null,
                    IsActive = true,
                    RentalLocationId = 1
                },
                new User
                {
                    Id = 3,
                    Email = "customer@gmail.com",
                    Password = "1",
                    PasswordHash = "$2a$12$z.y2vdQFkt/drkj6yzAXm.6v/rirvWIaw1tXyIgvR7dki1roEfLXm",
                    FullName = "Customer User",
                    PhoneNumber = "0123456789",
                    Role = "Customer",
                    ConfirmEmailToken = null,
                    IsEmailConfirmed = true,
                    CreatedAt = new DateTime(2025, 10, 11),
                    UpdatedAt = null,
                    IsActive = true,
                    RentalLocationId = null
                },
                new User
                {
                    Id = 4,
                    Email = "staff2@gmail.com",
                    Password = "1",
                    PasswordHash = "$2a$12$z.y2vdQFkt/drkj6yzAXm.6v/rirvWIaw1tXyIgvR7dki1roEfLXm",
                    FullName = "Staff User",
                    PhoneNumber = "0123456789",
                    Role = "Staff",
                    ConfirmEmailToken = null,
                    IsEmailConfirmed = true,
                    CreatedAt = new DateTime(2025, 10, 11),
                    UpdatedAt = null,
                    IsActive = true,
                    RentalLocationId = 2
                },
                new User
                {
                    Id = 5,
                    Email = "staff3@gmail.com",
                    Password = "1",
                    PasswordHash = "$2a$12$z.y2vdQFkt/drkj6yzAXm.6v/rirvWIaw1tXyIgvR7dki1roEfLXm",
                    FullName = "Staff User",
                    PhoneNumber = "0123456789",
                    Role = "Staff",
                    ConfirmEmailToken = null,
                    IsEmailConfirmed = true,
                    CreatedAt = new DateTime(2025, 10, 11),
                    UpdatedAt = null,
                    IsActive = true,
                    RentalLocationId = 3
                },
                new User
                {
                    Id = 6,
                    Email = "staff4@gmail.com",
                    Password = "1",
                    PasswordHash = "$2a$12$z.y2vdQFkt/drkj6yzAXm.6v/rirvWIaw1tXyIgvR7dki1roEfLXm",
                    FullName = "Staff User",
                    PhoneNumber = "0123456789",
                    Role = "Staff",
                    ConfirmEmailToken = null,
                    IsEmailConfirmed = true,
                    CreatedAt = new DateTime(2025, 10, 11),
                    UpdatedAt = null,
                    IsActive = true,
                    RentalLocationId = 4
                }
                    );
                });
            base.OnModelCreating(modelBuilder);
         
        }
    }
}
//Add-Migration InitMigration -Context EVSDbContext -Project Repository -StartupProject EVStation-basedRentalSystem -OutputDir Context/Migrations