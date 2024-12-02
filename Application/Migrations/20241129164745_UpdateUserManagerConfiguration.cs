using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NoteApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserManagerConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user1-id",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "59b443aa-fb64-430e-abcf-aba185101156", "AQAAAAIAAYagAAAAEHQqh87hGecECZECELOq/sbBm95dPS0mRKBy6XqTMKBHSjUKm+Jonb/97EaBRl6HVA==", "3cc60e58-9dfb-4620-a258-8121c14908c0" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user2-id",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b9ad8ce2-03fc-4932-b59e-fe933856dcb4", "AQAAAAIAAYagAAAAEOf51Il8EMX5DtywDx+UuXLUV2h4E96bdnlp+/WFkdrjXooU2nd53TrRUk4lqnNBMA==", "aa2548bd-bdf0-4058-94cf-d727a52c6d2e" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user1-id",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ad01879f-2d77-4b9a-b793-940be7f0bd2f", "AQAAAAIAAYagAAAAEOsUBn7KFENh6H8bCfN3F6Um+ZEinEHpWdfdut7y01BC/wN7zD3R7bWLB9Vp5LVqCQ==", "ffd18ac0-6f73-41c6-93e3-d0222ccd3fd4" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user2-id",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "64630560-ab23-4634-acf9-7d7d3abf63ef", "AQAAAAIAAYagAAAAEPmZ0VcBSrcoph+DYSgI2cYiTydUcgVAca9nsG1o4n4z8G6tvgXLwjn+EoNgtAPhZA==", "f2cf394e-3efa-4eb0-8aea-756ed5af7e94" });
        }
    }
}
