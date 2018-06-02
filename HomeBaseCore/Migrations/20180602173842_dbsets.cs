using Microsoft.EntityFrameworkCore.Migrations;

namespace HomeBaseCore.Migrations
{
    public partial class dbsets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "files",
                columns: table => new
                {
                    FileDataID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FolderID = table.Column<int>(nullable: false),
                    OwnerProfileID = table.Column<int>(nullable: false),
                    FilePath = table.Column<string>(nullable: true),
                    FileName = table.Column<string>(nullable: true),
                    FileDescription = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_files", x => x.FileDataID);
                });

            migrationBuilder.CreateTable(
                name: "folders",
                columns: table => new
                {
                    FolderDataID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OwnerProfileID = table.Column<int>(nullable: false),
                    FolderPath = table.Column<string>(nullable: true),
                    FolderName = table.Column<string>(nullable: true),
                    FolderDescription = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_folders", x => x.FolderDataID);
                });

            migrationBuilder.CreateTable(
                name: "profiles",
                columns: table => new
                {
                    ProfileDataID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProfileGuid = table.Column<string>(nullable: true),
                    Username = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_profiles", x => x.ProfileDataID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "files");

            migrationBuilder.DropTable(
                name: "folders");

            migrationBuilder.DropTable(
                name: "profiles");
        }
    }
}
