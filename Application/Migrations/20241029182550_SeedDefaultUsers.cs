using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NoteApp.Migrations
{
    /// <inheritdoc />
    public partial class SeedDefaultUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user1-id",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "58bf7c80-24d4-42d5-a07a-45e17e3aaf98", "AQAAAAIAAYagAAAAEAzUJKbU+bMwz75METVLYCGuOqRnJlOK1PBLBlVQgLCjFC3NXQlN6ZaC8G8Aj/tvOQ==", "bf6f2227-14ed-42ad-bf79-cfda3a45085f" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user2-id",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "9eb21bdb-aa78-4374-8e95-07edc6124b8d", "AQAAAAIAAYagAAAAEJpLy767WxcMbVEVIQsNqh41PaQEXjscC71CECLcQ+NBndy7CnrQrmtRzRxiwwfGGA==", "211f940a-6ed0-47b5-bceb-46e3e599dd66" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user1-id",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "042fd34c-aabc-4cc3-9735-5cc8d9a672ad", "AQAAAAIAAYagAAAAEG6D105sJQ/hjhON2MkNWZHjxVd6F5O8Y3Q4fdyrKuF2PLyoCrzIkzQAtTnvvGUGSQ==", "07e495d5-7583-4112-b633-35ae1876e988" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user2-id",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "eca07448-37f3-4f4e-bf8e-7e2eef762dfb", "AQAAAAIAAYagAAAAEKrqZ54qIrQei/ocOhvb9ym9dfFbRaEDQ7kHSopGugEiVyhYzYlsOb8gQgDRTBkVkg==", "af8ec79a-845b-4219-9068-e3a2d3f7c841" });
        }
    }
}
