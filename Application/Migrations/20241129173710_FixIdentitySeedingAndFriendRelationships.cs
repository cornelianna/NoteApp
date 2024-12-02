using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NoteApp.Migrations
{
    /// <inheritdoc />
    public partial class FixIdentitySeedingAndFriendRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user1-id",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "66bf222e-839a-4fb7-9bd5-5f9e8e9c633c", "AQAAAAIAAYagAAAAEN77G7q07Ns54JmMYfIGFkMG7eJlQvCRIDH+Fhd2jP/t80KfnNLX6Oxy8dx0QP6HKA==", "d8630b28-1adc-4f62-994c-e8ee9bfc1809" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user2-id",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b1dd98dd-d0b2-4b33-8793-a87a32fe7634", "AQAAAAIAAYagAAAAEIF9gCeeyWh1CqbaQJlDrajKImhwah5OxTl2D7YiywQmTjNgubFSMi8+6yyRNUXfLw==", "06f572af-9943-4514-b7f6-86414f2c33a4" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
