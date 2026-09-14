using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAn_Pc_DACS.Migrations
{
    /// <inheritdoc />
    public partial class MigrateAccessorySpecifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE product
                SET TechnicalSpecifications = NULLIF(CONCAT_WS(CHAR(13) + CHAR(10),
                    NULLIF(LTRIM(RTRIM(specification.Socket)), N''),
                    NULLIF(LTRIM(RTRIM(specification.Mainboard)), N''),
                    NULLIF(LTRIM(RTRIM(specification.RamType)), N''),
                    NULLIF(LTRIM(RTRIM(specification.Storage)), N''),
                    NULLIF(LTRIM(RTRIM(specification.PowerSupply)), N''),
                    NULLIF(LTRIM(RTRIM(specification.Vga)), N''),
                    NULLIF(LTRIM(RTRIM(specification.FormFactor)), N''),
                    NULLIF(LTRIM(RTRIM(specification.Cooler)), N''),
                    CASE WHEN specification.Wattage > 0
                         THEN CONCAT(N'Công suất: ', specification.Wattage, N'W')
                         ELSE NULL END), N'')
                FROM Products AS product
                INNER JOIN Categories AS category ON category.Id = product.CategoryId
                INNER JOIN ComponentSpecs AS specification ON specification.ProductId = product.Id
                WHERE category.Slug NOT IN (N'pc-gaming', N'pc-workstation', N'pc-workstation-2d-3d')
                  AND NULLIF(LTRIM(RTRIM(product.TechnicalSpecifications)), N'') IS NULL;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Giữ nguyên dữ liệu đã chuyển để tránh làm mất thông số người dùng nhập.
        }
    }
}
