using DoAn_Pc_DACS.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DoAn_Pc_DACS.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20260915100000_AddBuildPcMetadata")]
public class AddBuildPcMetadata : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>("ComponentType", "Products", type: "nvarchar(20)", maxLength: 20, nullable: true);
        migrationBuilder.AddColumn<string>("BuildSocket", "Products", type: "nvarchar(20)", maxLength: 20, nullable: true);
        migrationBuilder.AddColumn<string>("BuildMemoryType", "Products", type: "nvarchar(20)", maxLength: 20, nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn("ComponentType", "Products");
        migrationBuilder.DropColumn("BuildSocket", "Products");
        migrationBuilder.DropColumn("BuildMemoryType", "Products");
    }
}
