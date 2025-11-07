using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Repository.Context.Migrations
{
    /// <inheritdoc />
    public partial class InitMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cars",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Seats = table.Column<int>(type: "int", nullable: false),
                    SizeType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrunkCapacity = table.Column<int>(type: "int", nullable: false),
                    BatteryType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BatteryDuration = table.Column<int>(type: "int", nullable: false),
                    RentPricePerDay = table.Column<double>(type: "float", nullable: false),
                    RentPricePerHour = table.Column<double>(type: "float", nullable: false),
                    RentPricePerDayWithDriver = table.Column<double>(type: "float", nullable: false),
                    RentPricePerHourWithDriver = table.Column<double>(type: "float", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl3 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cars", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CitizenIds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CitizenIdNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RentalOrderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CitizenIds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DriverLicenses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LicenseNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RentalOrderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriverLicenses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RentalLocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Coordinates = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentalLocations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarRentalLocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    CarId = table.Column<int>(type: "int", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarRentalLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarRentalLocations_Cars_CarId",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarRentalLocations_RentalLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "RentalLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConfirmEmailToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsEmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    ResetPasswordToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResetPasswordTokenExpiry = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RentalLocationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_RentalLocations_RentalLocationId",
                        column: x => x.RentalLocationId,
                        principalTable: "RentalLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    RentalOrderId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RentalContacts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RentalDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RentalPeriod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReturnDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TerminationClause = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RentalOrderId = table.Column<int>(type: "int", nullable: true),
                    LesseeId = table.Column<int>(type: "int", nullable: false),
                    LessorId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentalContacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RentalContacts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RentalOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PickupTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpectedReturnTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualReturnTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SubTotal = table.Column<double>(type: "float", nullable: true),
                    Total = table.Column<double>(type: "float", nullable: true),
                    Discount = table.Column<int>(type: "int", nullable: true),
                    ExtraFee = table.Column<double>(type: "float", nullable: true),
                    DamageFee = table.Column<double>(type: "float", nullable: true),
                    DamageNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WithDriver = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RentalLocationId = table.Column<int>(type: "int", nullable: false),
                    CarId = table.Column<int>(type: "int", nullable: false),
                    RentalContactId = table.Column<int>(type: "int", nullable: true),
                    CitizenId = table.Column<int>(type: "int", nullable: true),
                    DriverLicenseId = table.Column<int>(type: "int", nullable: true),
                    PaymentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentalOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RentalOrders_Cars_CarId",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RentalOrders_CitizenIds_CitizenId",
                        column: x => x.CitizenId,
                        principalTable: "CitizenIds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RentalOrders_DriverLicenses_DriverLicenseId",
                        column: x => x.DriverLicenseId,
                        principalTable: "DriverLicenses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RentalOrders_Payments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RentalOrders_RentalContacts_RentalContactId",
                        column: x => x.RentalContactId,
                        principalTable: "RentalContacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RentalOrders_RentalLocations_RentalLocationId",
                        column: x => x.RentalLocationId,
                        principalTable: "RentalLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RentalOrders_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarDeliveryHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OdometerStart = table.Column<int>(type: "int", nullable: false),
                    BatteryLevelStart = table.Column<int>(type: "int", nullable: false),
                    VehicleConditionStart = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    StaffId = table.Column<int>(type: "int", nullable: false),
                    CarId = table.Column<int>(type: "int", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarDeliveryHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarDeliveryHistories_Cars_CarId",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarDeliveryHistories_RentalLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "RentalLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarDeliveryHistories_RentalOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "RentalOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarDeliveryHistories_Users_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarDeliveryHistories_Users_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarReturnHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReturnDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OdometerEnd = table.Column<int>(type: "int", nullable: false),
                    BatteryLevelEnd = table.Column<int>(type: "int", nullable: false),
                    VehicleConditionEnd = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    StaffId = table.Column<int>(type: "int", nullable: false),
                    CarId = table.Column<int>(type: "int", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarReturnHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarReturnHistories_Cars_CarId",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarReturnHistories_RentalLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "RentalLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarReturnHistories_RentalOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "RentalOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarReturnHistories_Users_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarReturnHistories_Users_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Feedbacks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RentalOrderId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedbacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Feedbacks_RentalOrders_RentalOrderId",
                        column: x => x.RentalOrderId,
                        principalTable: "RentalOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Feedbacks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Cars",
                columns: new[] { "Id", "BatteryDuration", "BatteryType", "CreatedAt", "ImageUrl", "ImageUrl2", "ImageUrl3", "IsActive", "IsDeleted", "Model", "Name", "RentPricePerDay", "RentPricePerDayWithDriver", "RentPricePerHour", "RentPricePerHourWithDriver", "Seats", "SizeType", "Status", "TrunkCapacity", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 350, "Lithium-Ion", new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://example.com/tesla_model_3.jpg", "https://example.com/tesla_model_3.jpg", "https://example.com/tesla_model_3.jpg", true, false, "Tesla Model 3", "Model 3", 1000000.0, 1400000.0, 45000.0, 60000.0, 5, "Sedan", 1, 425, null },
                    { 2, 240, "Lithium-Ion", new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://example.com/nissan_leaf.jpg", "https://example.com/nissan_leaf.jpg", "https://example.com/nissan_leaf.jpg", true, false, "Nissan Leaf", "Leaf", 800000.0, 1200000.0, 35000.0, 50000.0, 5, "Hatchback", 1, 435, null },
                    { 3, 259, "Lithium-Ion", new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://example.com/chevrolet_bolt_ev.jpg", "https://example.com/chevrolet_bolt_ev.jpg", "https://example.com/chevrolet_bolt_ev.jpg", true, false, "Chevrolet Bolt EV", "Bolt EV", 900000.0, 1300000.0, 40000.0, 55000.0, 5, "Hatchback", 1, 478, null },
                    { 4, 305, "Lithium-Ion", new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://example.com/hyundai_kona.jpg", "https://example.com/hyundai_kona.jpg", "https://example.com/hyundai_kona.jpg", true, false, "Hyundai Kona Electric", "Kona Electric", 900000.0, 1300000.0, 40000.0, 55000.0, 5, "SUV", 1, 332, null },
                    { 5, 510, "Lithium-Ion", new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://example.com/kia_ev6.jpg", "https://example.com/kia_ev6.jpg", "https://example.com/kia_ev6.jpg", true, false, "Kia EV6", "EV6", 1200000.0, 1600000.0, 55000.0, 75000.0, 5, "Crossover", 1, 480, null },
                    { 6, 285, "Lithium-Ion", new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://example.com/vf_e34.jpg", "https://example.com/vf_e34.jpg", "https://example.com/vf_e34.jpg", true, false, "VinFast VF e34", "VF e34", 850000.0, 1150000.0, 38000.0, 52000.0, 5, "SUV", 1, 290, null },
                    { 7, 200, "Lithium-Ion", new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://example.com/bmw_i3.jpg", "https://example.com/bmw_i3.jpg", "https://example.com/bmw_i3.jpg", true, false, "BMW i3", "i3", 1100000.0, 1450000.0, 50000.0, 68000.0, 4, "Hatchback", 1, 260, null },
                    { 8, 450, "Lithium-Ion", new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://example.com/porsche_taycan.jpg", "https://example.com/porsche_taycan.jpg", "https://example.com/porsche_taycan.jpg", true, false, "Porsche Taycan", "Taycan", 2500000.0, 3000000.0, 100000.0, 130000.0, 4, "Sedan", 1, 366, null },
                    { 9, 410, "Lithium-Ion", new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://example.com/mercedes_eqc.jpg", "https://example.com/mercedes_eqc.jpg", "https://example.com/mercedes_eqc.jpg", true, false, "Mercedes EQC", "EQC", 1800000.0, 2300000.0, 85000.0, 110000.0, 5, "SUV", 1, 500, null },
                    { 10, 430, "Lithium-Ion", new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://example.com/audi_etron.jpg", "https://example.com/audi_etron.jpg", "https://example.com/audi_etron.jpg", true, false, "Audi e-tron", "e-tron", 1700000.0, 2200000.0, 80000.0, 100000.0, 5, "SUV", 1, 555, null }
                });

            migrationBuilder.InsertData(
                table: "RentalContacts",
                columns: new[] { "Id", "IsDeleted", "LesseeId", "LessorId", "RentalDate", "RentalOrderId", "RentalPeriod", "ReturnDate", "Status", "TerminationClause", "UserId" },
                values: new object[,]
                {
                    { 1, false, 1, 2, new DateTime(2025, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "3 ngày", new DateTime(2025, 10, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Chấm dứt hợp đồng sớm sẽ chịu phí 10%.", null },
                    { 2, false, 2, 3, new DateTime(2025, 10, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "7 ngày", new DateTime(2025, 10, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, "Phí chấm dứt sớm: 15%.", null },
                    { 3, false, 3, 1, new DateTime(2025, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "5 ngày", new DateTime(2025, 10, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Phải thông báo trước 24h để chấm dứt hợp đồng.", null },
                    { 4, false, 4, 5, new DateTime(2025, 10, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, "2 ngày", new DateTime(2025, 10, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Không hoàn tiền nếu hủy trong vòng 12h.", null },
                    { 5, false, 5, 6, new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, "10 ngày", new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, "Phí hủy hợp đồng: 20% tổng giá trị.", null },
                    { 6, false, 6, 7, new DateTime(2025, 10, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, "1 ngày", new DateTime(2025, 10, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, "Không hoàn tiền nếu hủy trong ngày thuê.", null },
                    { 7, false, 7, 8, new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, "4 ngày", new DateTime(2025, 10, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, "Có thể gia hạn thêm tối đa 2 ngày.", null },
                    { 8, false, 8, 9, new DateTime(2025, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 8, "6 ngày", new DateTime(2025, 10, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Không áp dụng hoàn tiền nếu xe bị hư hại.", null },
                    { 9, false, 9, 10, new DateTime(2025, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 9, "8 ngày", new DateTime(2025, 11, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, "Có thể kết thúc sớm nhưng không hoàn phí thuê.", null },
                    { 10, false, 10, 1, new DateTime(2025, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 10, "3 ngày", new DateTime(2025, 11, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Hợp đồng sẽ tự động hết hạn sau ngày trả xe.", null }
                });

            migrationBuilder.InsertData(
                table: "RentalLocations",
                columns: new[] { "Id", "Address", "Coordinates", "CreatedAt", "IsActive", "IsDeleted", "LocationId", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "123 Tran Hung Dao St, Ho Chi Minh City", "10.7769,106.7009", new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, 0, "Downtown Rental Location", null },
                    { 2, "456 Nguyen Cuu Phuc St, Ho Chi Minh City", "10.7950,106.6540", new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, 0, "Airport Rental Location", null },
                    { 3, "12 Le Loi St, District 1, Ho Chi Minh City", "10.7760,106.7030", new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, 0, "District 1 Rental Location", null },
                    { 4, "89 Nguyen Van Linh St, District 7, Ho Chi Minh City", "10.7298,106.7219", new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, 0, "District 7 Rental Location", null },
                    { 5, "27 Dinh Bo Linh St, Binh Thanh District, Ho Chi Minh City", "10.8123,106.7098", new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, 0, "Binh Thanh Rental Location", null },
                    { 6, "101 Vo Van Ngan St, Thu Duc City", "10.8502,106.7549", new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, 0, "Thu Duc Rental Location", null },
                    { 7, "35 Hoang Van Thu St, Tan Binh District, Ho Chi Minh City", "10.8015,106.6521", new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, 0, "Tan Binh Rental Location", null },
                    { 8, "58 Phan Xich Long St, Phu Nhuan District, Ho Chi Minh City", "10.7998,106.6825", new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, 0, "Phu Nhuan Rental Location", null },
                    { 9, "245 Quang Trung St, Go Vap District, Ho Chi Minh City", "10.8412,106.6647", new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, 0, "Go Vap Rental Location", null },
                    { 10, "500 Kinh Duong Vuong St, Binh Tan District, Ho Chi Minh City", "10.7487,106.6032", new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, 0, "Binh Tan Rental Location", null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Address", "ConfirmEmailToken", "CreatedAt", "Email", "FullName", "IsActive", "IsEmailConfirmed", "Password", "PasswordHash", "PhoneNumber", "RentalLocationId", "ResetPasswordToken", "ResetPasswordTokenExpiry", "Role", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, null, null, new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@gmail.com", "Admin User", true, true, "1", "$2a$12$z.y2vdQFkt/drkj6yzAXm.6v/rirvWIaw1tXyIgvR7dki1roEfLXm", null, null, null, null, "Admin", null },
                    { 3, null, null, new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "customer@gmail.com", "Customer User", true, true, "1", "$2a$12$z.y2vdQFkt/drkj6yzAXm.6v/rirvWIaw1tXyIgvR7dki1roEfLXm", null, null, null, null, "Customer", null }
                });

            migrationBuilder.InsertData(
                table: "CarRentalLocations",
                columns: new[] { "Id", "CarId", "IsDeleted", "LocationId", "Quantity" },
                values: new object[,]
                {
                    { 1, 1, false, 1, 5 },
                    { 2, 2, false, 1, 3 },
                    { 3, 3, false, 2, 4 },
                    { 4, 4, false, 2, 2 },
                    { 5, 5, false, 3, 6 },
                    { 6, 6, false, 3, 3 },
                    { 7, 7, false, 4, 5 },
                    { 8, 8, false, 4, 2 },
                    { 9, 9, false, 5, 4 },
                    { 10, 10, false, 5, 3 },
                    { 11, 1, false, 6, 2 }
                });

            migrationBuilder.InsertData(
                table: "Feedbacks",
                columns: new[] { "Id", "Content", "CreatedAt", "IsDeleted", "RentalOrderId", "Title", "UpdatedAt", "UserId" },
                values: new object[,]
                {
                    { 1, "Xe mới, sạch sẽ và nhân viên hỗ trợ rất nhiệt tình.", new DateTime(2025, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, "Dịch vụ tuyệt vời", null, 1 },
                    { 3, "Chỉ mất vài phút để hoàn thành thủ tục thuê xe.", new DateTime(2025, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 3, "Thủ tục nhanh chóng", null, 3 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Address", "ConfirmEmailToken", "CreatedAt", "Email", "FullName", "IsActive", "IsEmailConfirmed", "Password", "PasswordHash", "PhoneNumber", "RentalLocationId", "ResetPasswordToken", "ResetPasswordTokenExpiry", "Role", "UpdatedAt" },
                values: new object[,]
                {
                    { 2, null, null, new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "staff@gmail.com", "Staff User", true, true, "1", "$2a$12$z.y2vdQFkt/drkj6yzAXm.6v/rirvWIaw1tXyIgvR7dki1roEfLXm", null, 1, null, null, "Staff", null },
                    { 4, "123 Tran Hung Dao St, Ho Chi Minh City", null, new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "duongduy12314@gmail.com", "Customer User 1", true, true, "1", "$2a$12$examplehash4", "0945353500", 1, null, null, "Admin", null },
                    { 5, "456 Nguyen Cuu Phuc St, Ho Chi Minh City", null, new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "customer2@gmail.com", "Customer User 2", true, true, "1", "$2a$12$examplehash5", "0901000005", 2, null, null, "Customer", null },
                    { 6, "789 Le Lai St, Ho Chi Minh City", null, new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "staff1@gmail.com", "Staff User 1", true, true, "1", "$2a$12$examplehash6", "0901000006", 1, null, null, "Staff", null },
                    { 7, "101 Nguyen Van Linh St, District 7, Ho Chi Minh City", null, new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "staff2@gmail.com", "Staff User 2", true, true, "1", "$2a$12$examplehash7", "0901000007", 2, null, null, "Staff", null },
                    { 8, "202 Le Loi St, District 1, Ho Chi Minh City", null, new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "customer3@gmail.com", "Customer User 3", true, true, "1", "$2a$12$examplehash8", "0901000008", 3, null, null, "Customer", null },
                    { 9, "303 Vo Van Tan St, District 3, Ho Chi Minh City", null, new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "staff3@gmail.com", "Staff User 3", true, true, "1", "$2a$12$examplehash9", "0901000009", 3, null, null, "Staff", null },
                    { 10, "404 Nguyen Trai St, District 5, Ho Chi Minh City", null, new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "customer4@gmail.com", "Customer User 4", true, true, "1", "$2a$12$examplehash10", "0901000010", 4, null, null, "Customer", null }
                });

            migrationBuilder.InsertData(
                table: "Feedbacks",
                columns: new[] { "Id", "Content", "CreatedAt", "IsDeleted", "RentalOrderId", "Title", "UpdatedAt", "UserId" },
                values: new object[,]
                {
                    { 2, "Giá thuê xe phù hợp, chất lượng xe tốt, pin đầy đủ.", new DateTime(2025, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 2, "Giá hợp lý", null, 2 },
                    { 4, "Nhân viên tư vấn rất chu đáo, hướng dẫn chi tiết khi nhận xe.", new DateTime(2025, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 4, "Nhân viên thân thiện", null, 4 },
                    { 5, "Xe điện chạy rất êm, tiết kiệm và thân thiện môi trường.", new DateTime(2025, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 5, "Xe chạy êm", null, 5 },
                    { 6, "Mọi thứ ổn, nhưng mong có thêm nhiều trạm sạc hơn ở trung tâm.", new DateTime(2025, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 6, "Cần thêm trạm sạc", null, 6 },
                    { 7, "Dịch vụ chuyên nghiệp, hệ thống đặt xe dễ dùng.", new DateTime(2025, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 7, "Trải nghiệm tốt", null, 7 },
                    { 8, "Tôi gặp vấn đề nhỏ về pin, nhân viên đã hỗ trợ rất nhanh.", new DateTime(2025, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 8, "Hỗ trợ nhanh", null, 8 },
                    { 9, "Xe mới, thiết kế hiện đại, rất hài lòng với chuyến đi.", new DateTime(2025, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 9, "Xe đẹp", null, 9 },
                    { 10, "Tôi sẽ tiếp tục thuê xe ở đây trong các chuyến công tác sau.", new DateTime(2025, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 10, "Rất hài lòng", null, 10 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarDeliveryHistories_CarId",
                table: "CarDeliveryHistories",
                column: "CarId");

            migrationBuilder.CreateIndex(
                name: "IX_CarDeliveryHistories_CustomerId",
                table: "CarDeliveryHistories",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CarDeliveryHistories_LocationId",
                table: "CarDeliveryHistories",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarDeliveryHistories_OrderId",
                table: "CarDeliveryHistories",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CarDeliveryHistories_StaffId",
                table: "CarDeliveryHistories",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_CarRentalLocations_CarId",
                table: "CarRentalLocations",
                column: "CarId");

            migrationBuilder.CreateIndex(
                name: "IX_CarRentalLocations_LocationId",
                table: "CarRentalLocations",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarReturnHistories_CarId",
                table: "CarReturnHistories",
                column: "CarId");

            migrationBuilder.CreateIndex(
                name: "IX_CarReturnHistories_CustomerId",
                table: "CarReturnHistories",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CarReturnHistories_LocationId",
                table: "CarReturnHistories",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarReturnHistories_OrderId",
                table: "CarReturnHistories",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CarReturnHistories_StaffId",
                table: "CarReturnHistories",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_RentalOrderId",
                table: "Feedbacks",
                column: "RentalOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_UserId",
                table: "Feedbacks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_UserId",
                table: "Payments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RentalContacts_UserId",
                table: "RentalContacts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RentalOrders_CarId",
                table: "RentalOrders",
                column: "CarId");

            migrationBuilder.CreateIndex(
                name: "IX_RentalOrders_CitizenId",
                table: "RentalOrders",
                column: "CitizenId",
                unique: true,
                filter: "[CitizenId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RentalOrders_DriverLicenseId",
                table: "RentalOrders",
                column: "DriverLicenseId",
                unique: true,
                filter: "[DriverLicenseId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RentalOrders_PaymentId",
                table: "RentalOrders",
                column: "PaymentId",
                unique: true,
                filter: "[PaymentId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RentalOrders_RentalContactId",
                table: "RentalOrders",
                column: "RentalContactId",
                unique: true,
                filter: "[RentalContactId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RentalOrders_RentalLocationId",
                table: "RentalOrders",
                column: "RentalLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_RentalOrders_UserId",
                table: "RentalOrders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RentalLocationId",
                table: "Users",
                column: "RentalLocationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarDeliveryHistories");

            migrationBuilder.DropTable(
                name: "CarRentalLocations");

            migrationBuilder.DropTable(
                name: "CarReturnHistories");

            migrationBuilder.DropTable(
                name: "Feedbacks");

            migrationBuilder.DropTable(
                name: "RentalOrders");

            migrationBuilder.DropTable(
                name: "Cars");

            migrationBuilder.DropTable(
                name: "CitizenIds");

            migrationBuilder.DropTable(
                name: "DriverLicenses");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "RentalContacts");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "RentalLocations");
        }
    }
}
