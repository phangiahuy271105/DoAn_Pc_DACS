using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DoAn_Pc_DACS.Migrations
{
    /// <inheritdoc />
    public partial class SeedDataPc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name", "Slug" },
                values: new object[,]
                {
                    { 1, "PC GAMING", "pc-gaming" },
                    { 2, "PC WORKSTATION", "pc-workstation" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "Discount", "ImageUrl", "Name", "OldPrice", "Price" },
                values: new object[,]
                {
                    { 1, 1, 3, "https://placehold.co/600x600?text=PC+GAMING+1", "PC TTG GAMING i5 12400F - RTX 5060", 29990000m, 28980000m },
                    { 2, 1, 4, "https://placehold.co/600x600?text=PC+GAMING+2", "PC TTG GAMING ULTRA 5 245KF", 39990000m, 38480000m }
                });

            migrationBuilder.InsertData(
                table: "ComponentSpecs",
                columns: new[] { "Id", "FormFactor", "ProductId", "RamType", "Socket", "Wattage" },
                values: new object[,]
                {
                    { 1, "Micro-ATX", 1, "16GB (2x8GB) DDR4 3200MHz", "Intel LGA 1700", 650 },
                    { 2, "ATX", 2, "32GB (2x16GB) DDR5 6000MHz RGB", "Intel LGA 1700 (Core Ultra)", 850 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ComponentSpecs",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ComponentSpecs",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
