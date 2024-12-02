using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NoteApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIdentityConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user1-id",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "00d32224-192c-4ead-be7c-fdc86264b1ce", "AQAAAAIAAYagAAAAEATQGB65x6FBvKHV4bXejpZM6n+lbESuiKVewyq1Q01J/skW4GRTwHp2GMCYQoZNXg==", "bed144bc-3ff3-44e4-b91b-83fe5be17608" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user2-id",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ffe6390c-c557-4d48-8dcb-7179d00b95cb", "AQAAAAIAAYagAAAAEOWeyDmReYb/CdFXdGDzrhTdDs6gVbS2TF4qaLQL3UJTtfjYZl52uBXbRCwBgwHp6A==", "bba80ada-b6ab-45c3-9b9a-3ec65de5d9a5" });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_UserId",
                table: "Comments",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_UserId",
                table: "Comments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_UserId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_UserId",
                table: "Comments");

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
    }
}
