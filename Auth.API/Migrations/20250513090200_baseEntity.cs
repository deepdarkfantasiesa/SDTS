using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace User.API.Migrations
{
    /// <inheritdoc />
    public partial class baseEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdateAt",
                table: "user_role_permission",
                newName: "update_at");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "user_role_permission",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "CreateAt",
                table: "user_role_permission",
                newName: "create_at");

            migrationBuilder.RenameColumn(
                name: "UpdateAt",
                table: "user_role",
                newName: "update_at");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "user_role",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "CreateAt",
                table: "user_role",
                newName: "create_at");

            migrationBuilder.RenameColumn(
                name: "UpdateAt",
                table: "user",
                newName: "update_at");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "user",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "CreateAt",
                table: "user",
                newName: "create_at");

            migrationBuilder.RenameColumn(
                name: "UpdateAt",
                table: "role_permission",
                newName: "update_at");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "role_permission",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "CreateAt",
                table: "role_permission",
                newName: "create_at");

            migrationBuilder.RenameColumn(
                name: "UpdateAt",
                table: "role",
                newName: "update_at");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "role",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "CreateAt",
                table: "role",
                newName: "create_at");

            migrationBuilder.AlterColumn<bool>(
                name: "is_deleted",
                table: "user_role_permission",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "is_deleted",
                table: "user_role",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "is_deleted",
                table: "user",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "is_deleted",
                table: "role_permission",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "is_deleted",
                table: "role",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "update_at",
                table: "user_role_permission",
                newName: "UpdateAt");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "user_role_permission",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "create_at",
                table: "user_role_permission",
                newName: "CreateAt");

            migrationBuilder.RenameColumn(
                name: "update_at",
                table: "user_role",
                newName: "UpdateAt");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "user_role",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "create_at",
                table: "user_role",
                newName: "CreateAt");

            migrationBuilder.RenameColumn(
                name: "update_at",
                table: "user",
                newName: "UpdateAt");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "user",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "create_at",
                table: "user",
                newName: "CreateAt");

            migrationBuilder.RenameColumn(
                name: "update_at",
                table: "role_permission",
                newName: "UpdateAt");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "role_permission",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "create_at",
                table: "role_permission",
                newName: "CreateAt");

            migrationBuilder.RenameColumn(
                name: "update_at",
                table: "role",
                newName: "UpdateAt");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "role",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "create_at",
                table: "role",
                newName: "CreateAt");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "user_role_permission",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "user_role",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "user",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "role_permission",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "role",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);
        }
    }
}
