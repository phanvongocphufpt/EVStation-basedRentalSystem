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
                name: "CitizenIds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CitizenIdNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
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
                    UserId = table.Column<int>(type: "int", nullable: false)
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
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentalLocations", x => x.Id);
                });

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
                    DepositOrderAmount = table.Column<double>(type: "float", nullable: false),
                    DepositCarAmount = table.Column<double>(type: "float", nullable: false),
                    BatteryDuration = table.Column<int>(type: "int", nullable: false),
                    RentPricePerDay = table.Column<double>(type: "float", nullable: false),
                    RentPricePer4Hour = table.Column<double>(type: "float", nullable: false),
                    RentPricePer8Hour = table.Column<double>(type: "float", nullable: false),
                    RentPricePerDayWithDriver = table.Column<double>(type: "float", nullable: false),
                    RentPricePer4HourWithDriver = table.Column<double>(type: "float", nullable: false),
                    RentPricePer8HourWithDriver = table.Column<double>(type: "float", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl3 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RentalLocationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cars", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cars_RentalLocations_RentalLocationId",
                        column: x => x.RentalLocationId,
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
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConfirmEmailToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsEmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    ResetPasswordToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResetPasswordTokenExpiry = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RentalLocationId = table.Column<int>(type: "int", nullable: true),
                    CitizenId = table.Column<int>(type: "int", nullable: true),
                    DriverLicenseId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_CitizenIds_CitizenId",
                        column: x => x.CitizenId,
                        principalTable: "CitizenIds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Users_DriverLicenses_DriverLicenseId",
                        column: x => x.DriverLicenseId,
                        principalTable: "DriverLicenses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Users_RentalLocations_RentalLocationId",
                        column: x => x.RentalLocationId,
                        principalTable: "RentalLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RentalOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PickupTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpectedReturnTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualReturnTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SubTotal = table.Column<double>(type: "float", nullable: true),
                    DepositOrder = table.Column<double>(type: "float", nullable: true),
                    DepositCar = table.Column<double>(type: "float", nullable: true),
                    Total = table.Column<double>(type: "float", nullable: true),
                    Discount = table.Column<double>(type: "float", nullable: true),
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
                    ContactImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactImageUrl2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReportNote = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OdometerStart = table.Column<int>(type: "int", nullable: false),
                    BatteryLevelStart = table.Column<int>(type: "int", nullable: false),
                    VehicleConditionStart = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl5 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl6 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    CarId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true)
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
                        name: "FK_CarDeliveryHistories_RentalOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "RentalOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarDeliveryHistories_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CarReturnHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReturnDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OdometerEnd = table.Column<int>(type: "int", nullable: false),
                    BatteryLevelEnd = table.Column<int>(type: "int", nullable: false),
                    VehicleConditionEnd = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl5 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl6 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    CarId = table.Column<int>(type: "int", nullable: false)
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
                        name: "FK_CarReturnHistories_RentalOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "RentalOrders",
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
                    Rating = table.Column<int>(type: "int", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaymentType = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BillingImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TxnRef = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    RentalOrderId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_RentalOrders_RentalOrderId",
                        column: x => x.RentalOrderId,
                        principalTable: "RentalOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Payments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "RentalLocations",
                columns: new[] { "Id", "Address", "Coordinates", "CreatedAt", "IsActive", "IsDeleted", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "209 Nguyễn Văn Tăng, Long Thạnh Mỹ, Thủ Đức, TP.HCM", "10.84274, 106.8198", new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "EVStation Nguyễn Văn Tăng", null },
                    { 2, "447 Lê Văn Việt, Thủ Đức, TP.HCM", "10.84529, 106.7933", new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "EVStation Lê Văn Việt", null },
                    { 3, "39634 Kha Vạn Cân, Linh Chiểu, Thủ Đức, TP.HCM", "10.856468, 106.756518", new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "EVStation Kha Vạn Cân", null },
                    { 4, "190 Võ Văn Ngân, Bình Thọ, Thủ Đức, TP.HCM", "10.850805, 106.763773", new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "EVStation Võ Văn Ngân", null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Address", "BankName", "BankNumber", "CitizenId", "ConfirmEmailToken", "CreatedAt", "DriverLicenseId", "Email", "FullName", "IsActive", "IsEmailConfirmed", "Password", "PasswordHash", "PhoneNumber", "RentalLocationId", "ResetPasswordToken", "ResetPasswordTokenExpiry", "Role", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, null, null, null, null, null, new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "admin@gmail.com", "Admin User", true, true, "1", "$2a$12$z.y2vdQFkt/drkj6yzAXm.6v/rirvWIaw1tXyIgvR7dki1roEfLXm", "0123456789", null, null, null, "Admin", null },
                    { 3, null, null, null, null, null, new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "customer@gmail.com", "Customer User", true, true, "1", "$2a$12$z.y2vdQFkt/drkj6yzAXm.6v/rirvWIaw1tXyIgvR7dki1roEfLXm", "0123456789", null, null, null, "Customer", null }
                });

            migrationBuilder.InsertData(
                table: "Cars",
                columns: new[] { "Id", "BatteryDuration", "BatteryType", "CreatedAt", "DepositCarAmount", "DepositOrderAmount", "ImageUrl", "ImageUrl2", "ImageUrl3", "IsActive", "IsDeleted", "Model", "Name", "RentPricePer4Hour", "RentPricePer4HourWithDriver", "RentPricePer8Hour", "RentPricePer8HourWithDriver", "RentPricePerDay", "RentPricePerDayWithDriver", "RentalLocationId", "Seats", "SizeType", "TrunkCapacity", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 350, "Lithium-Ion", new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), 2000000.0, 200000.0, "https://example.com/tesla_model_3.jpg", "https://example.com/tesla_model_3.jpg", "https://example.com/tesla_model_3.jpg", true, false, "Tesla Model 3", "Model 3", 300000.0, 400000.0, 500000.0, 700000.0, 1000000.0, 1400000.0, 1, 5, "Sedan", 425, null },
                    { 2, 240, "Lithium-Ion", new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), 4500000.0, 160000.0, "https://example.com/nissan_leaf.jpg", "https://example.com/nissan_leaf.jpg", "https://example.com/nissan_leaf.jpg", true, false, "Nissan Leaf", "Leaf", 250000.0, 350000.0, 450000.0, 600000.0, 800000.0, 1200000.0, 2, 5, "Hatchback", 435, null },
                    { 3, 259, "Lithium-Ion", new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), 3400000.0, 180000.0, "https://example.com/chevrolet_bolt_ev.jpg", "https://example.com/chevrolet_bolt_ev.jpg", "https://example.com/chevrolet_bolt_ev.jpg", true, false, "Chevrolet Bolt EV", "Bolt EV", 280000.0, 380000.0, 480000.0, 650000.0, 900000.0, 1300000.0, 3, 5, "Hatchback", 478, null },
                    { 4, 153, "Lithium-Ion", new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), 20000000.0, 110000.0, "https://example.com/bmw_i3.jpg", "https://example.com/bmw_i3.jpg", "https://example.com/bmw_i3.jpg", true, false, "BMW i3", "i3", 350000.0, 450000.0, 550000.0, 750000.0, 1100000.0, 1500000.0, 3, 4, "Hatchback", 260, null },
                    { 5, 222, "Lithium-Ion", new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), 17000000.0, 225000.0, "https://example.com/audi_e_tron.jpg", "https://example.com/audi_e_tron.jpg", "https://example.com/audi_e_tron.jpg", true, false, "Audi e-tron", "e-tron", 450000.0, 600000.0, 750000.0, 900000.0, 1500000.0, 2000000.0, 4, 5, "SUV", 660, null },
                    { 6, 258, "Lithium-Ion", new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), 6000000.0, 175000.0, "https://example.com/hyundai_kona_electric.jpg", "https://example.com/hyundai_kona_electric.jpg", "https://example.com/hyundai_kona_electric.jpg", true, false, "Hyundai Kona Electric", "Kona Electric", 290000.0, 390000.0, 490000.0, 640000.0, 950000.0, 1350000.0, 2, 5, "SUV", 332, null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Address", "BankName", "BankNumber", "CitizenId", "ConfirmEmailToken", "CreatedAt", "DriverLicenseId", "Email", "FullName", "IsActive", "IsEmailConfirmed", "Password", "PasswordHash", "PhoneNumber", "RentalLocationId", "ResetPasswordToken", "ResetPasswordTokenExpiry", "Role", "UpdatedAt" },
                values: new object[,]
                {
                    { 2, null, null, null, null, null, new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "staff@gmail.com", "Staff User", true, true, "1", "$2a$12$z.y2vdQFkt/drkj6yzAXm.6v/rirvWIaw1tXyIgvR7dki1roEfLXm", "0123456789", 1, null, null, "Staff", null },
                    { 4, null, null, null, null, null, new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "staff2@gmail.com", "Staff User", true, true, "1", "$2a$12$z.y2vdQFkt/drkj6yzAXm.6v/rirvWIaw1tXyIgvR7dki1roEfLXm", "0123456789", 2, null, null, "Staff", null },
                    { 5, null, null, null, null, null, new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "staff3@gmail.com", "Staff User", true, true, "1", "$2a$12$z.y2vdQFkt/drkj6yzAXm.6v/rirvWIaw1tXyIgvR7dki1roEfLXm", "0123456789", 3, null, null, "Staff", null },
                    { 6, null, null, null, null, null, new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "staff4@gmail.com", "Staff User", true, true, "1", "$2a$12$z.y2vdQFkt/drkj6yzAXm.6v/rirvWIaw1tXyIgvR7dki1roEfLXm", "0123456789", 4, null, null, "Staff", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarDeliveryHistories_CarId",
                table: "CarDeliveryHistories",
                column: "CarId");

            migrationBuilder.CreateIndex(
                name: "IX_CarDeliveryHistories_OrderId",
                table: "CarDeliveryHistories",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CarDeliveryHistories_UserId",
                table: "CarDeliveryHistories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarReturnHistories_CarId",
                table: "CarReturnHistories",
                column: "CarId");

            migrationBuilder.CreateIndex(
                name: "IX_CarReturnHistories_OrderId",
                table: "CarReturnHistories",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Cars_RentalLocationId",
                table: "Cars",
                column: "RentalLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_RentalOrderId",
                table: "Feedbacks",
                column: "RentalOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_UserId",
                table: "Feedbacks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_RentalOrderId",
                table: "Payments",
                column: "RentalOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_UserId",
                table: "Payments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RentalOrders_CarId",
                table: "RentalOrders",
                column: "CarId");

            migrationBuilder.CreateIndex(
                name: "IX_RentalOrders_RentalLocationId",
                table: "RentalOrders",
                column: "RentalLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_RentalOrders_UserId",
                table: "RentalOrders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CitizenId",
                table: "Users",
                column: "CitizenId",
                unique: true,
                filter: "[CitizenId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DriverLicenseId",
                table: "Users",
                column: "DriverLicenseId",
                unique: true,
                filter: "[DriverLicenseId] IS NOT NULL");

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
                name: "CarReturnHistories");

            migrationBuilder.DropTable(
                name: "Feedbacks");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "RentalOrders");

            migrationBuilder.DropTable(
                name: "Cars");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "CitizenIds");

            migrationBuilder.DropTable(
                name: "DriverLicenses");

            migrationBuilder.DropTable(
                name: "RentalLocations");
        }
    }
}
