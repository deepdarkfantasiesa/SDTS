using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace User.API.Migrations
{
    /// <inheritdoc />
    public partial class update_user_role_per : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_role_permission_user_role_RoleId",
                table: "user_role_permission");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "user_role_permission",
                newName: "role_id");

            migrationBuilder.RenameIndex(
                name: "IX_user_role_permission_RoleId",
                table: "user_role_permission",
                newName: "IX_user_role_permission_role_id");

            migrationBuilder.AddForeignKey(
                name: "FK_user_role_permission_user_role_role_id",
                table: "user_role_permission",
                column: "role_id",
                principalTable: "user_role",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_role_permission_user_role_role_id",
                table: "user_role_permission");

            migrationBuilder.RenameColumn(
                name: "role_id",
                table: "user_role_permission",
                newName: "RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_user_role_permission_role_id",
                table: "user_role_permission",
                newName: "IX_user_role_permission_RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_user_role_permission_user_role_RoleId",
                table: "user_role_permission",
                column: "RoleId",
                principalTable: "user_role",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
