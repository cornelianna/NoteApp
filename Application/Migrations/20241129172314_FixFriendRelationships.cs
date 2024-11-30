using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NoteApp.Migrations
{
    /// <inheritdoc />
    public partial class FixFriendRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user1-id",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "31cb41c7-2605-40cb-8bad-381111ddd1ed", "AQAAAAIAAYagAAAAEPyi5X6LXZAIDkDKUx8fZ3q4F+oHX34MvCQRHlgOqV1wmhYNJlPxm+h1Q5rZHgQ2rA==", "b5953456-8066-4a12-8fe4-d437de1a7833" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user2-id",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c9ea233c-c285-493d-8ed2-8c74c204f658", "AQAAAAIAAYagAAAAECjFcEk/IEgU5vunDfJTjc/QnlSLUCcMmp8Wwz/iFTudakq9MJoNA4vsYms45W4aNg==", "a34aab93-a76c-4d18-bd34-09f41efb8bd1" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
