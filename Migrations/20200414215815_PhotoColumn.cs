using Microsoft.EntityFrameworkCore.Migrations;

namespace datingapp.api.Migrations
{
    public partial class PhotoColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PublicId",
                table: "photos",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "photos");
        }
    }
}
