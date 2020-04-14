using Microsoft.EntityFrameworkCore.Migrations;

namespace datingapp.api.Migrations
{
    public partial class photoediting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PubliccId",
                table: "photos",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PubliccId",
                table: "photos");
        }
    }
}
