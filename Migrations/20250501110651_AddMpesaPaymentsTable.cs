using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JC_Ecommerce.Migrations
{
    /// <inheritdoc />
    public partial class AddMpesaPaymentsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MpesaPayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CheckoutId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MerchantRequestId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResultCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResultDesc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MpesaReceiptNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MpesaPayments", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MpesaPayments");
        }
    }
}
