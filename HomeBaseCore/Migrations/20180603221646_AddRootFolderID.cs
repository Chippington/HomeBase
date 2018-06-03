using Microsoft.EntityFrameworkCore.Migrations;

namespace HomeBaseCore.Migrations
{
    public partial class AddRootFolderID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RootFolderID",
                table: "folders",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RootFolderID",
                table: "folders");
        }
    }
}
