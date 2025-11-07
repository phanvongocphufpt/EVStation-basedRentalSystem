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
                        Model = "Hyundai Kona Electric",
                        Name = "Kona Electric",
                        Seats = 5,
                        SizeType = "SUV",
                        TrunkCapacity = 332,
                        BatteryType = "Lithium-Ion",
                        BatteryDuration = 305,
                        RentPricePerDay = 900000,
                        RentPricePerHour = 40000,
                        RentPricePerDayWithDriver = 1300000,
                        RentPricePerHourWithDriver = 55000,
                        ImageUrl = "https://example.com/hyundai_kona.jpg",
                        ImageUrl2 = "https://example.com/hyundai_kona.jpg",
                        ImageUrl3 = "https://example.com/hyundai_kona.jpg",
                        Status = 1,
                        CreatedAt = new DateTime(2025, 10, 11),
                        UpdatedAt = null,
                        IsActive = true,
                        IsDeleted = false
                    },
new Car
{
    Id = 5,
    Model = "Kia EV6",
    Name = "EV6",
    Seats = 5,
    SizeType = "Crossover",
    TrunkCapacity = 480,
    BatteryType = "Lithium-Ion",
    BatteryDuration = 510,
    RentPricePerDay = 1200000,
    RentPricePerHour = 55000,
    RentPricePerDayWithDriver = 1600000,
    RentPricePerHourWithDriver = 75000,
    ImageUrl = "https://example.com/kia_ev6.jpg",
    ImageUrl2 = "https://example.com/kia_ev6.jpg",
    ImageUrl3 = "https://example.com/kia_ev6.jpg",
    Status = 1,
    CreatedAt = new DateTime(2025, 10, 11),
    UpdatedAt = null,
    IsActive = true,
    IsDeleted = false
},
new Car
{
    Id = 6,
    Model = "VinFast VF e34",
    Name = "VF e34",
    Seats = 5,
    SizeType = "SUV",
    TrunkCapacity = 290,
    BatteryType = "Lithium-Ion",
    BatteryDuration = 285,
    RentPricePerDay = 850000,
    RentPricePerHour = 38000,
    RentPricePerDayWithDriver = 1150000,
    RentPricePerHourWithDriver = 52000,
    ImageUrl = "https://example.com/vf_e34.jpg",
    ImageUrl2 = "https://example.com/vf_e34.jpg",
    ImageUrl3 = "https://example.com/vf_e34.jpg",
    Status = 1,
    CreatedAt = new DateTime(2025, 10, 11),
    UpdatedAt = null,
    IsActive = true,
    IsDeleted = false
},
new Car
{
    Id = 7,
    Model = "BMW i3",
    Name = "i3",
    Seats = 4,
    SizeType = "Hatchback",
    TrunkCapacity = 260,
    BatteryType = "Lithium-Ion",
    BatteryDuration = 200,
    RentPricePerDay = 1100000,
    RentPricePerHour = 50000,
    RentPricePerDayWithDriver = 1450000,
    RentPricePerHourWithDriver = 68000,
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
    Id = 8,
    Model = "Porsche Taycan",
    Name = "Taycan",
    Seats = 4,
    SizeType = "Sedan",
    TrunkCapacity = 366,
    BatteryType = "Lithium-Ion",
    BatteryDuration = 450,
    RentPricePerDay = 2500000,
    RentPricePerHour = 100000,
    RentPricePerDayWithDriver = 3000000,
    RentPricePerHourWithDriver = 130000,
    ImageUrl = "https://example.com/porsche_taycan.jpg",
    ImageUrl2 = "https://example.com/porsche_taycan.jpg",
    ImageUrl3 = "https://example.com/porsche_taycan.jpg",
    Status = 1,
    CreatedAt = new DateTime(2025, 10, 11),
    UpdatedAt = null,
    IsActive = true,
    IsDeleted = false
},
new Car
{
    Id = 9,
    Model = "Mercedes EQC",
    Name = "EQC",
    Seats = 5,
    SizeType = "SUV",
    TrunkCapacity = 500,
    BatteryType = "Lithium-Ion",
    BatteryDuration = 410,
    RentPricePerDay = 1800000,
    RentPricePerHour = 85000,
    RentPricePerDayWithDriver = 2300000,
    RentPricePerHourWithDriver = 110000,
    ImageUrl = "https://example.com/mercedes_eqc.jpg",
    ImageUrl2 = "https://example.com/mercedes_eqc.jpg",
    ImageUrl3 = "https://example.com/mercedes_eqc.jpg",
    Status = 1,
    CreatedAt = new DateTime(2025, 10, 11),
    UpdatedAt = null,
    IsActive = true,
    IsDeleted = false
},
new Car
{
    Id = 10,
    Model = "Audi e-tron",
    Name = "e-tron",
    Seats = 5,
    SizeType = "SUV",
    TrunkCapacity = 555,
    BatteryType = "Lithium-Ion",
    BatteryDuration = 430,
    RentPricePerDay = 1700000,
    RentPricePerHour = 80000,
    RentPricePerDayWithDriver = 2200000,
    RentPricePerHourWithDriver = 100000,
    ImageUrl = "https://example.com/audi_etron.jpg",
    ImageUrl2 = "https://example.com/audi_etron.jpg",
    ImageUrl3 = "https://example.com/audi_etron.jpg",
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

                entity.HasOne(e => e.Customer)
                    .WithMany(e => e.CarDeliveryHistories)
                    .HasForeignKey(e => e.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Staff)
                    .WithMany()
                    .HasForeignKey(e => e.StaffId)
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
                    }, new CarRentalLocation
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
    LocationId = 2,
    Quantity = 2
},
new CarRentalLocation
{
    Id = 5,
    CarId = 5,
    LocationId = 3,
    Quantity = 6
},
new CarRentalLocation
{
    Id = 6,
    CarId = 6,
    LocationId = 3,
    Quantity = 3
},
new CarRentalLocation
{
    Id = 7,
    CarId = 7,
    LocationId = 4,
    Quantity = 5
},
new CarRentalLocation
{
    Id = 8,
    CarId = 8,
    LocationId = 4,
    Quantity = 2
},
new CarRentalLocation
{
    Id = 9,
    CarId = 9,
    LocationId = 5,
    Quantity = 4
},
new CarRentalLocation
{
    Id = 10,
    CarId = 10,
    LocationId = 5,
    Quantity = 3
},
new CarRentalLocation
{
    Id = 11,
    CarId = 1,
    LocationId = 6,
    Quantity = 2
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

                entity.HasOne(e => e.Customer)
                    .WithMany()
                    .HasForeignKey(e => e.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Staff)
                    .WithMany()
                    .HasForeignKey(e => e.StaffId)
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
                modelBuilder.Entity<Feedback>().HasData(
        new Feedback
        {
            Id = 1,
            Title = "Dịch vụ tuyệt vời",
            Content = "Xe mới, sạch sẽ và nhân viên hỗ trợ rất nhiệt tình.",
            CreatedAt = new DateTime(2025, 11, 7),
            UpdatedAt = null,
            UserId = 1,
            RentalOrderId = 1,
            IsDeleted = false
        },
        new Feedback
        {
            Id = 2,
            Title = "Giá hợp lý",
            Content = "Giá thuê xe phù hợp, chất lượng xe tốt, pin đầy đủ.",
            CreatedAt = new DateTime(2025, 11, 7),
            UpdatedAt = null,
            UserId = 2,
            RentalOrderId = 2,
            IsDeleted = false
        },
        new Feedback
        {
            Id = 3,
            Title = "Thủ tục nhanh chóng",
            Content = "Chỉ mất vài phút để hoàn thành thủ tục thuê xe.",
            CreatedAt = new DateTime(2025, 11, 7),
            UpdatedAt = null,
            UserId = 3,
            RentalOrderId = 3,
            IsDeleted = false
        },
        new Feedback
        {
            Id = 4,
            Title = "Nhân viên thân thiện",
            Content = "Nhân viên tư vấn rất chu đáo, hướng dẫn chi tiết khi nhận xe.",
            CreatedAt = new DateTime(2025, 11, 7),
            UpdatedAt = null,
            UserId = 4,
            RentalOrderId = 4,
            IsDeleted = false
        },
        new Feedback
        {
            Id = 5,
            Title = "Xe chạy êm",
            Content = "Xe điện chạy rất êm, tiết kiệm và thân thiện môi trường.",
            CreatedAt = new DateTime(2025, 11, 7),
            UpdatedAt = null,
            UserId = 5,
            RentalOrderId = 5,
            IsDeleted = false
        },
        new Feedback
        {
            Id = 6,
            Title = "Cần thêm trạm sạc",
            Content = "Mọi thứ ổn, nhưng mong có thêm nhiều trạm sạc hơn ở trung tâm.",
            CreatedAt = new DateTime(2025, 11, 7),
            UpdatedAt = null,
            UserId = 6,
            RentalOrderId = 6,
            IsDeleted = false
        },
        new Feedback
        {
            Id = 7,
            Title = "Trải nghiệm tốt",
            Content = "Dịch vụ chuyên nghiệp, hệ thống đặt xe dễ dùng.",
            CreatedAt = new DateTime(2025, 11, 7),
            UpdatedAt = null,
            UserId = 7,
            RentalOrderId = 7,
            IsDeleted = false
        },
        new Feedback
        {
            Id = 8,
            Title = "Hỗ trợ nhanh",
            Content = "Tôi gặp vấn đề nhỏ về pin, nhân viên đã hỗ trợ rất nhanh.",
            CreatedAt = new DateTime(2025, 11, 7),
            UpdatedAt = null,
            UserId = 8,
            RentalOrderId = 8,
            IsDeleted = false
        },
        new Feedback
        {
            Id = 9,
            Title = "Xe đẹp",
            Content = "Xe mới, thiết kế hiện đại, rất hài lòng với chuyến đi.",
            CreatedAt = new DateTime(2025, 11, 7),
            UpdatedAt = null,
            UserId = 9,
            RentalOrderId = 9,
            IsDeleted = false
        },
        new Feedback
        {
            Id = 10,
            Title = "Rất hài lòng",
            Content = "Tôi sẽ tiếp tục thuê xe ở đây trong các chuyến công tác sau.",
            CreatedAt = new DateTime(2025, 11, 7),
            UpdatedAt = null,
            UserId = 10,
            RentalOrderId = 10,
            IsDeleted = false
        }
    );

            });

            // Configure Payment
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasOne(e => e.User)
                    .WithMany(e => e.Payments)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.RentalOrder)
                    .WithOne(e => e.Payment)
                    .HasForeignKey<Payment>(e => e.RentalOrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure RentalContact
            modelBuilder.Entity<RentalContact>(entity =>
            {
                entity.HasOne(e => e.RentalOrder)
                    .WithOne(e => e.RentalContact)
                    .HasForeignKey<RentalOrder>(e => e.RentalContactId)
                    .OnDelete(DeleteBehavior.Cascade);

                modelBuilder.Entity<RentalContact>().HasData(
    new RentalContact
    {
        Id = 1,
        RentalDate = new DateTime(2025, 10, 1),
        RentalPeriod = "3 ngày",
        ReturnDate = new DateTime(2025, 10, 4),
        TerminationClause = "Chấm dứt hợp đồng sớm sẽ chịu phí 10%.",
        Status = DocumentStatus.Approved,
        RentalOrderId = 1,
        LesseeId = 1,
        LessorId = 2,
        IsDeleted = false
    },
    new RentalContact
    {
        Id = 2,
        RentalDate = new DateTime(2025, 10, 5),
        RentalPeriod = "7 ngày",
        ReturnDate = new DateTime(2025, 10, 12),
        TerminationClause = "Phí chấm dứt sớm: 15%.",
        Status = DocumentStatus.Pending,
        RentalOrderId = 2,
        LesseeId = 2,
        LessorId = 3,
        IsDeleted = false
    },
    new RentalContact
    {
        Id = 3,
        RentalDate = new DateTime(2025, 10, 10),
        RentalPeriod = "5 ngày",
        ReturnDate = new DateTime(2025, 10, 15),
        TerminationClause = "Phải thông báo trước 24h để chấm dứt hợp đồng.",
        Status = DocumentStatus.Rejected,
        RentalOrderId = 3,
        LesseeId = 3,
        LessorId = 1,
        IsDeleted = false
    },
    new RentalContact
    {
        Id = 4,
        RentalDate = new DateTime(2025, 10, 15),
        RentalPeriod = "2 ngày",
        ReturnDate = new DateTime(2025, 10, 17),
        TerminationClause = "Không hoàn tiền nếu hủy trong vòng 12h.",
        Status = DocumentStatus.Approved,
        RentalOrderId = 4,
        LesseeId = 4,
        LessorId = 5,
        IsDeleted = false
    },
    new RentalContact
    {
        Id = 5,
        RentalDate = new DateTime(2025, 10, 18),
        RentalPeriod = "10 ngày",
        ReturnDate = new DateTime(2025, 10, 28),
        TerminationClause = "Phí hủy hợp đồng: 20% tổng giá trị.",
        Status = DocumentStatus.Pending,
        RentalOrderId = 5,
        LesseeId = 5,
        LessorId = 6,
        IsDeleted = false
    },
    new RentalContact
    {
        Id = 6,
        RentalDate = new DateTime(2025, 10, 20),
        RentalPeriod = "1 ngày",
        ReturnDate = new DateTime(2025, 10, 21),
        TerminationClause = "Không hoàn tiền nếu hủy trong ngày thuê.",
        Status = DocumentStatus.Pending,
        RentalOrderId = 6,
        LesseeId = 6,
        LessorId = 7,
        IsDeleted = false
    },
    new RentalContact
    {
        Id = 7,
        RentalDate = new DateTime(2025, 10, 22),
        RentalPeriod = "4 ngày",
        ReturnDate = new DateTime(2025, 10, 26),
        TerminationClause = "Có thể gia hạn thêm tối đa 2 ngày.",
        Status = DocumentStatus.Pending,
        RentalOrderId = 7,
        LesseeId = 7,
        LessorId = 8,
        IsDeleted = false
    },
    new RentalContact
    {
        Id = 8,
        RentalDate = new DateTime(2025, 10, 25),
        RentalPeriod = "6 ngày",
        ReturnDate = new DateTime(2025, 10, 31),
        TerminationClause = "Không áp dụng hoàn tiền nếu xe bị hư hại.",
        Status = DocumentStatus.Rejected,
        RentalOrderId = 8,
        LesseeId = 8,
        LessorId = 9,
        IsDeleted = false
    },
    new RentalContact
    {
        Id = 9,
        RentalDate = new DateTime(2025, 11, 1),
        RentalPeriod = "8 ngày",
        ReturnDate = new DateTime(2025, 11, 9),
        TerminationClause = "Có thể kết thúc sớm nhưng không hoàn phí thuê.",
        Status = DocumentStatus.Pending,
        RentalOrderId = 9,
        LesseeId = 9,
        LessorId = 10,
        IsDeleted = false
    },
    new RentalContact
    {
        Id = 10,
        RentalDate = new DateTime(2025, 11, 5),
        RentalPeriod = "3 ngày",
        ReturnDate = new DateTime(2025, 11, 8),
        TerminationClause = "Hợp đồng sẽ tự động hết hạn sau ngày trả xe.",
        Status = DocumentStatus.Approved,
        RentalOrderId = 10,
        LesseeId = 10,
        LessorId = 1,
        IsDeleted = false
    }
);

            }
            );

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
                        Name = "Downtown Rental Location",
                        Address = "123 Tran Hung Dao St, Ho Chi Minh City",
                        Coordinates = "10.7769,106.7009",
                        CreatedAt = new DateTime(2025, 10, 11),
                        UpdatedAt = null,
                        IsActive = true,
                        IsDeleted = false
                    },
                    new RentalLocation
                    {
                        Id = 2,
                        Name = "Airport Rental Location",
                        Address = "456 Nguyen Cuu Phuc St, Ho Chi Minh City",
                        Coordinates = "10.7950,106.6540",
                        CreatedAt = new DateTime(2025, 10, 11),
                        UpdatedAt = null,
                        IsActive = true,
                        IsDeleted = false
                    }, new RentalLocation
                    {
                        Id = 3,
                        Name = "District 1 Rental Location",
                        Address = "12 Le Loi St, District 1, Ho Chi Minh City",
                        Coordinates = "10.7760,106.7030",
                        CreatedAt = new DateTime(2025, 10, 11),
                        UpdatedAt = null,
                        IsActive = true,
                        IsDeleted = false
                    },
new RentalLocation
{
    Id = 4,
    Name = "District 7 Rental Location",
    Address = "89 Nguyen Van Linh St, District 7, Ho Chi Minh City",
    Coordinates = "10.7298,106.7219",
    CreatedAt = new DateTime(2025, 10, 11),
    UpdatedAt = null,
    IsActive = true,
    IsDeleted = false
},
new RentalLocation
{
    Id = 5,
    Name = "Binh Thanh Rental Location",
    Address = "27 Dinh Bo Linh St, Binh Thanh District, Ho Chi Minh City",
    Coordinates = "10.8123,106.7098",
    CreatedAt = new DateTime(2025, 10, 11),
    UpdatedAt = null,
    IsActive = true,
    IsDeleted = false
},
new RentalLocation
{
    Id = 6,
    Name = "Thu Duc Rental Location",
    Address = "101 Vo Van Ngan St, Thu Duc City",
    Coordinates = "10.8502,106.7549",
    CreatedAt = new DateTime(2025, 10, 11),
    UpdatedAt = null,
    IsActive = true,
    IsDeleted = false
},
new RentalLocation
{
    Id = 7,
    Name = "Tan Binh Rental Location",
    Address = "35 Hoang Van Thu St, Tan Binh District, Ho Chi Minh City",
    Coordinates = "10.8015,106.6521",
    CreatedAt = new DateTime(2025, 10, 11),
    UpdatedAt = null,
    IsActive = true,
    IsDeleted = false
},
new RentalLocation
{
    Id = 8,
    Name = "Phu Nhuan Rental Location",
    Address = "58 Phan Xich Long St, Phu Nhuan District, Ho Chi Minh City",
    Coordinates = "10.7998,106.6825",
    CreatedAt = new DateTime(2025, 10, 11),
    UpdatedAt = null,
    IsActive = true,
    IsDeleted = false
},
new RentalLocation
{
    Id = 9,
    Name = "Go Vap Rental Location",
    Address = "245 Quang Trung St, Go Vap District, Ho Chi Minh City",
    Coordinates = "10.8412,106.6647",
    CreatedAt = new DateTime(2025, 10, 11),
    UpdatedAt = null,
    IsActive = true,
    IsDeleted = false
},
new RentalLocation
{
    Id = 10,
    Name = "Binh Tan Rental Location",
    Address = "500 Kinh Duong Vuong St, Binh Tan District, Ho Chi Minh City",
    Coordinates = "10.7487,106.6032",
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

                entity.HasOne(e => e.Payment)
                    .WithOne(e => e.RentalOrder)
                    .HasForeignKey<RentalOrder>(e => e.PaymentId)
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

                entity.HasMany(e => e.CarDeliveryHistories)
                    .WithOne(e => e.Customer)
                    .HasForeignKey(e => e.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Note: Ignored ICollection<CarRentalLocation> as no corresponding FK in CarRentalLocation
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
            }, new User
            {
                Id = 4,
                Email = "duongduy12314@gmail.com",
                Password = "1",
                PasswordHash = "$2a$12$examplehash4",
                FullName = "Customer User 1",
                Role = "Admin",
                ConfirmEmailToken = null,
                IsEmailConfirmed = true,
                CreatedAt = new DateTime(2025, 10, 11),
                UpdatedAt = null,
                IsActive = true,
                RentalLocationId = 1,
                PhoneNumber = "0945353500",
                Address = "123 Tran Hung Dao St, Ho Chi Minh City"
            },
new User
{
    Id = 5,
    Email = "customer2@gmail.com",
    Password = "1",
    PasswordHash = "$2a$12$examplehash5",
    FullName = "Customer User 2",
    Role = "Customer",
    ConfirmEmailToken = null,
    IsEmailConfirmed = true,
    CreatedAt = new DateTime(2025, 10, 11),
    UpdatedAt = null,
    IsActive = true,
    RentalLocationId = 2,
    PhoneNumber = "0901000005",
    Address = "456 Nguyen Cuu Phuc St, Ho Chi Minh City"
},
new User
{
    Id = 6,
    Email = "staff1@gmail.com",
    Password = "1",
    PasswordHash = "$2a$12$examplehash6",
    FullName = "Staff User 1",
    Role = "Staff",
    ConfirmEmailToken = null,
    IsEmailConfirmed = true,
    CreatedAt = new DateTime(2025, 10, 11),
    UpdatedAt = null,
    IsActive = true,
    RentalLocationId = 1,
    PhoneNumber = "0901000006",
    Address = "789 Le Lai St, Ho Chi Minh City"
},
new User
{
    Id = 7,
    Email = "staff2@gmail.com",
    Password = "1",
    PasswordHash = "$2a$12$examplehash7",
    FullName = "Staff User 2",
    Role = "Staff",
    ConfirmEmailToken = null,
    IsEmailConfirmed = true,
    CreatedAt = new DateTime(2025, 10, 11),
    UpdatedAt = null,
    IsActive = true,
    RentalLocationId = 2,
    PhoneNumber = "0901000007",
    Address = "101 Nguyen Van Linh St, District 7, Ho Chi Minh City"
},
new User
{
    Id = 8,
    Email = "customer3@gmail.com",
    Password = "1",
    PasswordHash = "$2a$12$examplehash8",
    FullName = "Customer User 3",
    Role = "Customer",
    ConfirmEmailToken = null,
    IsEmailConfirmed = true,
    CreatedAt = new DateTime(2025, 10, 11),
    UpdatedAt = null,
    IsActive = true,
    RentalLocationId = 3,
    PhoneNumber = "0901000008",
    Address = "202 Le Loi St, District 1, Ho Chi Minh City"
},
new User
{
    Id = 9,
    Email = "staff3@gmail.com",
    Password = "1",
    PasswordHash = "$2a$12$examplehash9",
    FullName = "Staff User 3",
    Role = "Staff",
    ConfirmEmailToken = null,
    IsEmailConfirmed = true,
    CreatedAt = new DateTime(2025, 10, 11),
    UpdatedAt = null,
    IsActive = true,
    RentalLocationId = 3,
    PhoneNumber = "0901000009",
    Address = "303 Vo Van Tan St, District 3, Ho Chi Minh City"
},
new User
{
    Id = 10,
    Email = "customer4@gmail.com",
    Password = "1",
    PasswordHash = "$2a$12$examplehash10",
    FullName = "Customer User 4",
    Role = "Customer",
    ConfirmEmailToken = null,
    IsEmailConfirmed = true,
    CreatedAt = new DateTime(2025, 10, 11),
    UpdatedAt = null,
    IsActive = true,
    RentalLocationId = 4,
    PhoneNumber = "0901000010",
    Address = "404 Nguyen Trai St, District 5, Ho Chi Minh City"
}

                );
            });

            base.OnModelCreating(modelBuilder);

        }
    }
}
//Add-Migration InitMigration -Context EVSDbContext -Project Repository -StartupProject EVStation-basedRentalSystem -OutputDir Context/Migrations