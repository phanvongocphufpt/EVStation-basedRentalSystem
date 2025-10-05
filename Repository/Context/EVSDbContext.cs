using Microsoft.EntityFrameworkCore;
using Repository.Entities;
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
                    .WithOne(e => e.Feedback)
                    .HasForeignKey<Feedback>(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

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
            });

            // Configure RentalContact
            modelBuilder.Entity<RentalContact>(entity =>
            {
                entity.HasOne(e => e.RentalOrder)
                    .WithOne(e => e.RentalContact)
                    .HasForeignKey<RentalOrder>(e => e.RentalContactId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Lessee)
                    .WithMany()
                    .HasForeignKey(e => e.LesseeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Lessor)
                    .WithMany(e => e.RentalContacts)
                    .HasForeignKey(e => e.LessorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure RentalLocation
            modelBuilder.Entity<RentalLocation>(entity =>
            {
                entity.HasMany(e => e.CarRentalLocations)
                    .WithOne(e => e.Location)
                    .HasForeignKey(e => e.LocationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.RentalContacts)
                    .WithOne(e => e.Lessor)
                    .HasForeignKey(e => e.LessorId)
                    .OnDelete(DeleteBehavior.Restrict);
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
            });

            // Configure User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasOne(e => e.DriverLicense)
                    .WithOne(e => e.User)
                    .HasForeignKey<DriverLicense>(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.CitizenIdNavigation)
                    .WithOne(e => e.User)
                    .HasForeignKey<CitizenId>(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Feedback)
                    .WithOne(e => e.User)
                    .HasForeignKey<Feedback>(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

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
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
//Add-Migration InitMigration -Context EVSDbContext -Project Repository -StartupProject EVStation-basedRentalSystem -OutputDir Context/Migrations