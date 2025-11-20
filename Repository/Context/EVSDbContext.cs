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
        public DbSet<CarRentalLocation> CarRentalLocations { get; set; }
        public DbSet<CitizenId> CitizenIds { get; set; }
        public DbSet<DriverLicense> DriverLicenses { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<RentalOrder> RentalOrders { get; set; }
        public DbSet<RentalContact> RentalContacts { get; set; }
        public DbSet<RentalLocation> RentalLocations { get; set; }
        public DbSet<User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Car
            modelBuilder.Entity<Car>(entity =>
            {
                entity.HasMany(e => e.CarRentalLocations)
                    .WithOne(e => e.Car)
                    .HasForeignKey(e => e.CarId)
                    .OnDelete(DeleteBehavior.Cascade);

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
                        BatteryDuration = 350,
                        RentPricePerDay = 1000000,
                        RentPricePerHour = 45000,
                        RentPricePerDayWithDriver = 1400000,
                        RentPricePerHourWithDriver = 60000,
                        ImageUrl = "https://example.com/tesla_model_3.jpg",
                        ImageUrl2 = "https://example.com/tesla_model_3.jpg",
                        ImageUrl3 = "https://example.com/tesla_model_3.jpg",
                        Status = 1,
                        CreatedAt = new DateTime(2025, 10, 11),
                        UpdatedAt = null,
                        IsActive = true,
                        IsDeleted = false
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
                        BatteryDuration = 240,
                        RentPricePerDay = 800000,
                        RentPricePerHour = 35000,
                        RentPricePerDayWithDriver = 1200000,
                        RentPricePerHourWithDriver = 50000,
                        ImageUrl = "https://example.com/nissan_leaf.jpg",
                        ImageUrl2 = "https://example.com/nissan_leaf.jpg",
                        ImageUrl3 = "https://example.com/nissan_leaf.jpg",
                        Status = 1,
                        CreatedAt = new DateTime(2025, 10, 11),
                        UpdatedAt = null,
                        IsActive = true,
                        IsDeleted = false
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
                        BatteryDuration = 259,
                        RentPricePerDay = 900000,
                        RentPricePerHour = 40000,
                        RentPricePerDayWithDriver = 1300000,
                        RentPricePerHourWithDriver = 55000,
                        ImageUrl = "https://example.com/chevrolet_bolt_ev.jpg",
                        ImageUrl2 = "https://example.com/chevrolet_bolt_ev.jpg",
                        ImageUrl3 = "https://example.com/chevrolet_bolt_ev.jpg",
                        Status = 1,
                        CreatedAt = new DateTime(2025, 10, 11),
                        UpdatedAt = null,
                        IsActive = true,
                        IsDeleted = false
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
                        BatteryDuration = 153,
                        RentPricePerDay = 1100000,
                        RentPricePerHour = 50000,
                        RentPricePerDayWithDriver = 1500000,
                        RentPricePerHourWithDriver = 65000,
                        ImageUrl = "https://example.com/bmw_i3.jpg",
                        ImageUrl2 = "https://example.com/bmw_i3.jpg",
                        ImageUrl3 = "https://example.com/bmw_i3.jpg",
                        Status = 1,
                        CreatedAt = new DateTime(2025, 10, 11),
                        UpdatedAt = null,
                        IsActive = true,
                        IsDeleted = false
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
                        BatteryDuration = 222,
                        RentPricePerDay = 1500000,
                        RentPricePerHour = 70000,
                        RentPricePerDayWithDriver = 2000000,
                        RentPricePerHourWithDriver = 90000,
                        ImageUrl = "https://example.com/audi_e_tron.jpg",
                        ImageUrl2 = "https://example.com/audi_e_tron.jpg",
                        ImageUrl3 = "https://example.com/audi_e_tron.jpg",
                        Status = 1,
                        CreatedAt = new DateTime(2025, 10, 11),
                        UpdatedAt = null,
                        IsActive = true,
                        IsDeleted = false
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

            // Configure CarRentalLocation
            modelBuilder.Entity<CarRentalLocation>(entity =>
            {
                entity.HasOne(e => e.Car)
                    .WithMany(e => e.CarRentalLocations)
                    .HasForeignKey(e => e.CarId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Location)
                    .WithMany(e => e.CarRentalLocations)
                    .HasForeignKey(e => e.LocationId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasData(
                    new CarRentalLocation
                    {
                        Id = 1,
                        CarId = 1,
                        LocationId = 1,
                        Quantity = 5
                    },
                    new CarRentalLocation
                    {
                        Id = 2,
                        CarId = 2,
                        LocationId = 1,
                        Quantity = 3
                    },
                    new CarRentalLocation
                    {
                        Id = 3,
                        CarId = 3,
                        LocationId = 2,
                        Quantity = 4
                    },
                    new CarRentalLocation
                    {
                        Id = 4,
                        CarId = 4,
                        LocationId = 3,
                        Quantity = 2
                    },
                    new CarRentalLocation
                    {
                        Id = 5,
                        CarId = 1,
                        LocationId = 2,
                        Quantity = 2
                    },
                    new CarRentalLocation
                    {
                        Id = 6,
                        CarId = 2,
                        LocationId = 3,
                        Quantity = 3
                    },
                    new CarRentalLocation
                    {
                        Id = 7,
                        CarId = 3,
                        LocationId = 1,
                        Quantity = 1
                    },
                    new CarRentalLocation
                    {
                        Id = 8,
                        CarId = 4,
                        LocationId = 2,
                        Quantity = 2
                    },
                    new CarRentalLocation
                    {
                        Id = 9,
                        CarId = 1,
                        LocationId = 3,
                        Quantity = 4
                    },
                    new CarRentalLocation
                    {
                        Id = 10,
                        CarId = 2,
                        LocationId = 2,
                        Quantity = 2
                    },
                    new CarRentalLocation
                    {
                        Id = 11,
                        CarId = 3,
                        LocationId = 3,
                        Quantity = 3
                    },
                    new CarRentalLocation
                    {
                        Id = 12,
                        CarId = 4,
                        LocationId = 1,
                        Quantity = 6
                    },
                    new CarRentalLocation
                    {
                        Id = 13,
                        CarId = 5,
                        LocationId = 1,
                        Quantity = 4
                    },
                    new CarRentalLocation
                    {
                        Id = 14,
                        CarId = 5,
                        LocationId = 2,
                        Quantity = 5
                    },
                    new CarRentalLocation
                    {
                        Id = 15,
                        CarId = 5,
                        LocationId = 3,
                        Quantity = 3
                    },
                    new CarRentalLocation
                    {
                        Id = 16,
                        CarId = 4,
                        LocationId = 4,
                        Quantity = 10
                    }
                    );
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
                entity.HasOne(e => e.RentalOrder)
                    .WithOne(e => e.CitizenIdNavigation)
                    .HasForeignKey<CitizenId>(e => e.RentalOrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            // Configure DriverLicense
            modelBuilder.Entity<DriverLicense>(entity =>
                {
                    entity.HasOne(e => e.RentalOrder)
                        .WithOne(e => e.DriverLicense)
                        .HasForeignKey<DriverLicense>(e => e.RentalOrderId)
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

                // Configure RentalContact
                modelBuilder.Entity<RentalContact>(entity =>
                {
                    entity.HasOne(e => e.RentalOrder)
                        .WithOne(e => e.RentalContact)
                        .HasForeignKey<RentalOrder>(e => e.RentalContactId)
                        .OnDelete(DeleteBehavior.Cascade);
                });

                // Configure RentalLocation
                modelBuilder.Entity<RentalLocation>(entity =>
                {
                    entity.HasMany(e => e.CarRentalLocations)
                        .WithOne(e => e.Location)
                        .HasForeignKey(e => e.LocationId)
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
                            Name = "City Center Rental Location",
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

                    entity.HasOne(e => e.RentalContact)
                        .WithOne(e => e.RentalOrder)
                        .HasForeignKey<RentalOrder>(e => e.RentalContactId)
                        .OnDelete(DeleteBehavior.Cascade);

                    entity.HasMany(e => e.Payments)
                        .WithOne(e => e.RentalOrder)
                        .HasForeignKey(e => e.RentalOrderId)
                        .OnDelete(DeleteBehavior.Cascade);

                    entity.HasOne(e => e.RentalLocation)
                        .WithMany(e => e.RentalOrders)
                        .HasForeignKey(e => e.RentalLocationId)
                        .OnDelete(DeleteBehavior.Restrict);

                    entity.HasOne(e => e.CitizenIdNavigation)
                        .WithOne(e => e.RentalOrder)
                        .HasForeignKey<RentalOrder>(e => e.CitizenId)
                        .OnDelete(DeleteBehavior.Cascade);

                    entity.HasOne(e => e.DriverLicense)
                        .WithOne(e => e.RentalOrder)
                        .HasForeignKey<RentalOrder>(e => e.DriverLicenseId)
                        .OnDelete(DeleteBehavior.Cascade);
                });

                // Configure User
                modelBuilder.Entity<User>(entity =>
                {
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
                    Role = "Customer",
                    ConfirmEmailToken = null,
                    IsEmailConfirmed = true,
                    CreatedAt = new DateTime(2025, 10, 11),
                    UpdatedAt = null,
                    IsActive = true,
                    RentalLocationId = null
                }
                    );
                });

            base.OnModelCreating(modelBuilder);
         
        }
    }
}
//Add-Migration InitMigration -Context EVSDbContext -Project Repository -StartupProject EVStation-basedRentalSystem -OutputDir Context/Migrations