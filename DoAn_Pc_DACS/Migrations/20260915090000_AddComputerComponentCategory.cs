using DoAn_Pc_DACS.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAn_Pc_DACS.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260915090000_AddComputerComponentCategory")]
    public partial class AddComputerComponentCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF NOT EXISTS (SELECT 1 FROM Categories WHERE Slug = N'linh-kien-may-tinh')
                BEGIN
                    SET IDENTITY_INSERT Categories ON;
                    INSERT INTO Categories (Id, Name, Slug)
                    VALUES (105, N'LINH KIỆN MÁY TÍNH', N'linh-kien-may-tinh');
                    SET IDENTITY_INSERT Categories OFF;
                END
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "DELETE FROM Categories WHERE Id = 105 AND Slug = N'linh-kien-may-tinh' AND NOT EXISTS (SELECT 1 FROM Products WHERE CategoryId = 105)");
        }
    }
}
