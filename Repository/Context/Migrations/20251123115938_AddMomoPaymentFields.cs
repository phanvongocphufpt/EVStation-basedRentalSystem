using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Context.Migrations
{
    /// <inheritdoc />
    public partial class AddMomoPaymentFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MomoMessage",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MomoOrderId",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MomoPartnerCode",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MomoPayType",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MomoRequestId",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MomoResultCode",
                table: "Payments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MomoSignature",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "MomoTransId",
                table: "Payments",
                type: "bigint",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "RentalLocations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Address", "Coordinates", "Name" },
                values: new object[] { "209 Nguyễn Văn Tăng, Long Thạnh Mỹ, Thủ Đức, TP.HCM", "10.84274, 106.8198", "EVStation Nguyễn Văn Tăng" });

            migrationBuilder.UpdateData(
                table: "RentalLocations",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Address", "Coordinates", "Name" },
                values: new object[] { "447 Lê Văn Việt, Thủ Đức, TP.HCM", "10.84529, 106.7933", "EVStation Lê Văn Việt" });

            migrationBuilder.UpdateData(
                table: "RentalLocations",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Address", "Coordinates", "Name" },
                values: new object[] { "39634 Kha Vạn Cân, Linh Chiểu, Thủ Đức, TP.HCM", "10.856468, 106.756518", "EVStation Kha Vạn Cân" });

            migrationBuilder.UpdateData(
                table: "RentalLocations",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Address", "Coordinates" },
                values: new object[] { "190 Võ Văn Ngân, Bình Thọ, Thủ Đức, TP.HCM", "10.850805, 106.763773" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MomoMessage",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "MomoOrderId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "MomoPartnerCode",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "MomoPayType",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "MomoRequestId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "MomoResultCode",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "MomoSignature",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "MomoTransId",
                table: "Payments");

            migrationBuilder.UpdateData(
                table: "RentalLocations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Address", "Coordinates", "Name" },
                values: new object[] { "123 Tran Hung Dao St, Ho Chi Minh City", "10.7769,106.7009", "Downtown Rental Location" });

            migrationBuilder.UpdateData(
                table: "RentalLocations",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Address", "Coordinates", "Name" },
                values: new object[] { "456 Nguyen Cuu Phuc St, Ho Chi Minh City", "10.7950,106.6540", "Airport Rental Location" });

            migrationBuilder.UpdateData(
                table: "RentalLocations",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Address", "Coordinates", "Name" },
                values: new object[] { "789 Le Van Viet St, Ho Chi Minh City", "10.8500,106.7500", "Suburban Rental Location" });

            migrationBuilder.UpdateData(
                table: "RentalLocations",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Address", "Coordinates" },
                values: new object[] { "101 Nguyen Van Cu St, Ho Chi Minh City", "10.7700,106.6800" });
        }
    }
}
