using Microsoft.EntityFrameworkCore.Migrations;

namespace SDTS.DataAccess.Migrations
{
    public partial class addAltitudeInEmergencyHelpers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Altitude",
                table: "EmergencyHelpers",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Altitude",
                table: "EmergencyHelpers");
        }
    }
}
