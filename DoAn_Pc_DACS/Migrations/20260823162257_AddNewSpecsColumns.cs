using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAn_Pc_DACS.Migrations
{
    /// <inheritdoc />
    public partial class AddNewSpecsColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Vga",
                table: "ComponentSpecs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Storage",
                table: "ComponentSpecs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Socket",
                table: "ComponentSpecs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RamType",
                table: "ComponentSpecs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Mainboard",
                table: "ComponentSpecs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FormFactor",
                table: "ComponentSpecs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cooler",
                table: "ComponentSpecs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CoolerBh",
                table: "ComponentSpecs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CoolerSl",
                table: "ComponentSpecs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "FormFactorBh",
                table: "ComponentSpecs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FormFactorSl",
                table: "ComponentSpecs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "MainboardBh",
                table: "ComponentSpecs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MainboardSl",
                table: "ComponentSpecs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PowerSupply",
                table: "ComponentSpecs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PowerSupplyBh",
                table: "ComponentSpecs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PowerSupplySl",
                table: "ComponentSpecs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RamBh",
                table: "ComponentSpecs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RamSl",
                table: "ComponentSpecs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SocketBh",
                table: "ComponentSpecs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SocketSl",
                table: "ComponentSpecs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "StorageBh",
                table: "ComponentSpecs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StorageSl",
                table: "ComponentSpecs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "VgaBh",
                table: "ComponentSpecs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VgaSl",
                table: "ComponentSpecs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "ComponentSpecs",
                keyColumn: "Id",
                keyValue: 99,
                columns: new[] { "Cooler", "CoolerBh", "CoolerSl", "FormFactorBh", "FormFactorSl", "MainboardBh", "MainboardSl", "PowerSupply", "PowerSupplyBh", "PowerSupplySl", "RamBh", "RamSl", "SocketBh", "SocketSl", "StorageBh", "StorageSl", "VgaBh", "VgaSl" },
                values: new object[] { null, "12 Tháng", 1, "12 Tháng", 1, "36 Tháng", 1, null, "36 Tháng", 1, "36 Tháng", 1, "36 Tháng", 1, "36 Tháng", 1, "36 Tháng", 1 });

            migrationBuilder.UpdateData(
                table: "ComponentSpecs",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "Cooler", "CoolerBh", "CoolerSl", "FormFactorBh", "FormFactorSl", "MainboardBh", "MainboardSl", "PowerSupply", "PowerSupplyBh", "PowerSupplySl", "RamBh", "RamSl", "SocketBh", "SocketSl", "StorageBh", "StorageSl", "VgaBh", "VgaSl" },
                values: new object[] { null, "12 Tháng", 1, "12 Tháng", 1, "36 Tháng", 1, null, "36 Tháng", 1, "36 Tháng", 1, "36 Tháng", 1, "36 Tháng", 1, "36 Tháng", 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cooler",
                table: "ComponentSpecs");

            migrationBuilder.DropColumn(
                name: "CoolerBh",
                table: "ComponentSpecs");

            migrationBuilder.DropColumn(
                name: "CoolerSl",
                table: "ComponentSpecs");

            migrationBuilder.DropColumn(
                name: "FormFactorBh",
                table: "ComponentSpecs");

            migrationBuilder.DropColumn(
                name: "FormFactorSl",
                table: "ComponentSpecs");

            migrationBuilder.DropColumn(
                name: "MainboardBh",
                table: "ComponentSpecs");

            migrationBuilder.DropColumn(
                name: "MainboardSl",
                table: "ComponentSpecs");

            migrationBuilder.DropColumn(
                name: "PowerSupply",
                table: "ComponentSpecs");

            migrationBuilder.DropColumn(
                name: "PowerSupplyBh",
                table: "ComponentSpecs");

            migrationBuilder.DropColumn(
                name: "PowerSupplySl",
                table: "ComponentSpecs");

            migrationBuilder.DropColumn(
                name: "RamBh",
                table: "ComponentSpecs");

            migrationBuilder.DropColumn(
                name: "RamSl",
                table: "ComponentSpecs");

            migrationBuilder.DropColumn(
                name: "SocketBh",
                table: "ComponentSpecs");

            migrationBuilder.DropColumn(
                name: "SocketSl",
                table: "ComponentSpecs");

            migrationBuilder.DropColumn(
                name: "StorageBh",
                table: "ComponentSpecs");

            migrationBuilder.DropColumn(
                name: "StorageSl",
                table: "ComponentSpecs");

            migrationBuilder.DropColumn(
                name: "VgaBh",
                table: "ComponentSpecs");

            migrationBuilder.DropColumn(
                name: "VgaSl",
                table: "ComponentSpecs");

            migrationBuilder.AlterColumn<string>(
                name: "Vga",
                table: "ComponentSpecs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Storage",
                table: "ComponentSpecs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Socket",
                table: "ComponentSpecs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RamType",
                table: "ComponentSpecs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Mainboard",
                table: "ComponentSpecs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FormFactor",
                table: "ComponentSpecs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
