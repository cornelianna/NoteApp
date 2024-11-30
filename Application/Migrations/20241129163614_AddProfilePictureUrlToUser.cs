using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NoteApp.Migrations
{
    /// <inheritdoc />
    public partial class AddProfilePictureUrlToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfilePictureUrl",
                table: "AspNetUsers",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user1-id",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "ProfilePictureUrl", "SecurityStamp" },
                values: new object[] { "ad01879f-2d77-4b9a-b793-940be7f0bd2f", "AQAAAAIAAYagAAAAEOsUBn7KFENh6H8bCfN3F6Um+ZEinEHpWdfdut7y01BC/wN7zD3R7bWLB9Vp5LVqCQ==", null, "ffd18ac0-6f73-41c6-93e3-d0222ccd3fd4" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user2-id",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "ProfilePictureUrl", "SecurityStamp" },
                values: new object[] { "64630560-ab23-4634-acf9-7d7d3abf63ef", "AQAAAAIAAYagAAAAEPmZ0VcBSrcoph+DYSgI2cYiTydUcgVAca9nsG1o4n4z8G6tvgXLwjn+EoNgtAPhZA==", null, "f2cf394e-3efa-4eb0-8aea-756ed5af7e94" });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_UserId",
                table: "Posts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_AspNetUsers_UserId",
                table: "Posts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_AspNetUsers_UserId",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_UserId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "ProfilePictureUrl",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user1-id",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3855e7d0-0ed0-4580-894e-00b8c4f35fce", "AQAAAAIAAYagAAAAEK79u5jVibvUMA24g3F9T8OoWU06YIvSeSJGLaub2KbQhekPiq6JOav4+8TrjFJHhQ==", "f6e9eb9b-542a-4ee2-85d2-69ce32f3bd66" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user2-id",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "557d0f48-fc51-4a8c-a007-7f78b7c5ab2f", "AQAAAAIAAYagAAAAENs9OYEuXdfl2EfbZnLXB91S+JveB7EpRNkpPnOgWyXwB0gargVuHTR8rjbi6oFGMQ==", "a8bf888b-3997-4ffa-811b-b79bc399ee99" });
        }
    }
}
