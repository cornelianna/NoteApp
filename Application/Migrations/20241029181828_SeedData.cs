using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NoteApp.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "user1-id", 0, "042fd34c-aabc-4cc3-9735-5cc8d9a672ad", "user1@example.com", true, false, null, "USER1@EXAMPLE.COM", "USER1", "AQAAAAIAAYagAAAAEG6D105sJQ/hjhON2MkNWZHjxVd6F5O8Y3Q4fdyrKuF2PLyoCrzIkzQAtTnvvGUGSQ==", null, false, "07e495d5-7583-4112-b633-35ae1876e988", false, "user1" },
                    { "user2-id", 0, "eca07448-37f3-4f4e-bf8e-7e2eef762dfb", "user2@example.com", true, false, null, "USER2@EXAMPLE.COM", "USER2", "AQAAAAIAAYagAAAAEKrqZ54qIrQei/ocOhvb9ym9dfFbRaEDQ7kHSopGugEiVyhYzYlsOb8gQgDRTBkVkg==", null, false, "af8ec79a-845b-4219-9068-e3a2d3f7c841", false, "user2" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user1-id");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user2-id");
        }
    }
}
