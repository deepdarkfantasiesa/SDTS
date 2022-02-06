using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SDTS.DataAccess.Migrations
{
    public partial class AddSecureArea : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SecureAreas",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Latitude = table.Column<string>(type: "varchar(MAX)", nullable: true),
                    Longitude = table.Column<string>(type: "varchar(MAX)", nullable: true),
                    creatername = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    createtime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    information = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: false),
                    wardname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    wardaccount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    createraccount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    areaid = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecureAreas", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SecureAreas");
        }
    }
}
