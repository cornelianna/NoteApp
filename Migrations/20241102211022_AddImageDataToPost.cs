using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NoteApp.Migrations
{
    /// <inheritdoc />
    public partial class AddImageDataToPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Posts");

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                table: "Posts",
                type: "BLOB",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Friends",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "FriendId",
                table: "Friends",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user1-id",
                columns: new[] { "ConcurrencyStamp", "NormalizedUserName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "df6eeada-75d6-480a-91da-3cbe22b8f991", "USER1@EXAMPLE.COM", "AQAAAAIAAYagAAAAEHHz+acrYJL+/a2Jd0eigqnh215ft06W+nv3OwIQmS6NhLo57ufafsfDWlR1OrFMsw==", "b05f9c9b-402f-42da-91fb-3237277d804c", "user1@example.com" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user2-id",
                columns: new[] { "ConcurrencyStamp", "NormalizedUserName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "a0b29221-7f31-4ac2-ad06-c7257025726c", "USER2@EXAMPLE.COM", "AQAAAAIAAYagAAAAEKhuVYRPfTTAbj/ND++aCrlKfOHklEw1rMuxG4vJUpZE1mxCARX+mZSsB8ze8yV08g==", "b6c9d28c-ccad-449b-8560-da032ee0cc55", "user1@example.com" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageData",
                table: "Posts");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Posts",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Friends",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FriendId",
                table: "Friends",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user1-id",
                columns: new[] { "ConcurrencyStamp", "NormalizedUserName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "58bf7c80-24d4-42d5-a07a-45e17e3aaf98", "USER1", "AQAAAAIAAYagAAAAEAzUJKbU+bMwz75METVLYCGuOqRnJlOK1PBLBlVQgLCjFC3NXQlN6ZaC8G8Aj/tvOQ==", "bf6f2227-14ed-42ad-bf79-cfda3a45085f", "user1" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user2-id",
                columns: new[] { "ConcurrencyStamp", "NormalizedUserName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "9eb21bdb-aa78-4374-8e95-07edc6124b8d", "USER2", "AQAAAAIAAYagAAAAEJpLy767WxcMbVEVIQsNqh41PaQEXjscC71CECLcQ+NBndy7CnrQrmtRzRxiwwfGGA==", "211f940a-6ed0-47b5-bceb-46e3e599dd66", "user2" });
        }
    }
}
