using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompanyEmployees.Migrations
{
    public partial class CreateSQLDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd81ed77-8218-4d5e-a418-d4849f399061");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f1a24314-229f-47d3-ab8e-31e0016b2540");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "1d15cde9-a103-4ebc-8db5-320338872416", "0fb8a77a-418c-49a0-932b-37e121d7c79a", "Manager", "MANAGER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "21338029-d4a6-42ad-addd-997e73a9d9a6", "76aad488-7b9c-4700-bd1e-f87b8541b430", "Administrator", "ADMINISTRATOR" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1d15cde9-a103-4ebc-8db5-320338872416");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "21338029-d4a6-42ad-addd-997e73a9d9a6");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "cd81ed77-8218-4d5e-a418-d4849f399061", "3b56e68b-a7f9-4aaf-bd26-12aa72393c0c", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "f1a24314-229f-47d3-ab8e-31e0016b2540", "fc3e76ce-420d-4653-a7da-ad82d9b2c8bc", "Manager", "MANAGER" });
        }
    }
}
