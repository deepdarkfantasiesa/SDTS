using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace User.API.Migrations
{
    /// <inheritdoc />
    public partial class update_user_aggregate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Address_User_UserId",
                table: "Address");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRole_User_UserId",
                table: "UserRole");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRolePermission_UserRole_RoleId",
                table: "UserRolePermission");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User",
                table: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRolePermission",
                table: "UserRolePermission");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRole",
                table: "UserRole");

            migrationBuilder.RenameTable(
                name: "User",
                newName: "user");

            migrationBuilder.RenameTable(
                name: "UserRolePermission",
                newName: "user_role_permission");

            migrationBuilder.RenameTable(
                name: "UserRole",
                newName: "user_role");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "user",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "user",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Url",
                table: "user_role_permission",
                newName: "url");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "user_role_permission",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "user_role_permission",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "user_role_permission",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_UserRolePermission_RoleId",
                table: "user_role_permission",
                newName: "IX_user_role_permission_RoleId");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "user_role",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "user_role",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "user_role",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "user_role",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "IX_UserRole_UserId",
                table: "user_role",
                newName: "IX_user_role_user_id");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "user",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user",
                table: "user",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_role_permission",
                table: "user_role_permission",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_role",
                table: "user_role",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Address_user_UserId",
                table: "Address",
                column: "UserId",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_role_user_user_id",
                table: "user_role",
                column: "user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_role_permission_user_role_RoleId",
                table: "user_role_permission",
                column: "RoleId",
                principalTable: "user_role",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Address_user_UserId",
                table: "Address");

            migrationBuilder.DropForeignKey(
                name: "FK_user_role_user_user_id",
                table: "user_role");

            migrationBuilder.DropForeignKey(
                name: "FK_user_role_permission_user_role_RoleId",
                table: "user_role_permission");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user",
                table: "user");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user_role_permission",
                table: "user_role_permission");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user_role",
                table: "user_role");

            migrationBuilder.RenameTable(
                name: "user",
                newName: "User");

            migrationBuilder.RenameTable(
                name: "user_role_permission",
                newName: "UserRolePermission");

            migrationBuilder.RenameTable(
                name: "user_role",
                newName: "UserRole");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "User",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "User",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "url",
                table: "UserRolePermission",
                newName: "Url");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "UserRolePermission",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "UserRolePermission",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "UserRolePermission",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_user_role_permission_RoleId",
                table: "UserRolePermission",
                newName: "IX_UserRolePermission_RoleId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "UserRole",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "UserRole",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "UserRole",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "UserRole",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_user_role_user_id",
                table: "UserRole",
                newName: "IX_UserRole_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "User",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                table: "User",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRolePermission",
                table: "UserRolePermission",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRole",
                table: "UserRole",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Address_User_UserId",
                table: "Address",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRole_User_UserId",
                table: "UserRole",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRolePermission_UserRole_RoleId",
                table: "UserRolePermission",
                column: "RoleId",
                principalTable: "UserRole",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
