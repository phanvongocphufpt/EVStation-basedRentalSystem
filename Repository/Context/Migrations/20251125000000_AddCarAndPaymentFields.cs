using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Context.Migrations
{
    /// <inheritdoc />
    public partial class AddCarAndPaymentFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add Status column to Cars table (if not exists)
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Cars",
                type: "int",
                nullable: false,
                defaultValue: 1);

            // Add DepositPercent column to Cars table
            migrationBuilder.AddColumn<int>(
                name: "DepositPercent",
                table: "Cars",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Add RentalLocationId column to Cars table (nullable)
            migrationBuilder.AddColumn<int>(
                name: "RentalLocationId",
                table: "Cars",
                type: "int",
                nullable: true);

            // Add TxnRef column to Payments table (nullable)
            migrationBuilder.AddColumn<string>(
                name: "TxnRef",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);

            // Add TransactionNo column to Payments table (nullable)
            migrationBuilder.AddColumn<string>(
                name: "TransactionNo",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "DepositPercent",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "RentalLocationId",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "TxnRef",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "TransactionNo",
                table: "Payments");
        }
    }
}

