using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class sizekey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SizeKey",
                table: "Sizes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Sizes",
                keyColumn: "Id",
                keyValue: 1,
                column: "SizeKey",
                value: "S");

            migrationBuilder.UpdateData(
                table: "Sizes",
                keyColumn: "Id",
                keyValue: 2,
                column: "SizeKey",
                value: "M");

            migrationBuilder.UpdateData(
                table: "Sizes",
                keyColumn: "Id",
                keyValue: 3,
                column: "SizeKey",
                value: "L");

            migrationBuilder.UpdateData(
                table: "Sizes",
                keyColumn: "Id",
                keyValue: 4,
                column: "SizeKey",
                value: "XL");

            migrationBuilder.UpdateData(
                table: "Sizes",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "SizeKey", "SizeName" },
                values: new object[] { "XXL", "XLarge" });

            migrationBuilder.UpdateData(
                table: "Sizes",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "SizeKey", "SizeName" },
                values: new object[] { "32", "Size32" });

            migrationBuilder.UpdateData(
                table: "Sizes",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "SizeKey", "SizeName" },
                values: new object[] { "34", "Size34" });

            migrationBuilder.UpdateData(
                table: "Sizes",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "SizeKey", "SizeName" },
                values: new object[] { "36", "Size36" });

            migrationBuilder.UpdateData(
                table: "Sizes",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "SizeKey", "SizeName" },
                values: new object[] { "38", "Size38" });

            migrationBuilder.InsertData(
                table: "Sizes",
                columns: new[] { "Id", "Indicator", "SizeKey", "SizeName" },
                values: new object[] { 10, 0, "40", "Size40" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DropColumn(
                name: "SizeKey",
                table: "Sizes");

            migrationBuilder.UpdateData(
                table: "Sizes",
                keyColumn: "Id",
                keyValue: 5,
                column: "SizeName",
                value: "Size32");

            migrationBuilder.UpdateData(
                table: "Sizes",
                keyColumn: "Id",
                keyValue: 6,
                column: "SizeName",
                value: "Size34");

            migrationBuilder.UpdateData(
                table: "Sizes",
                keyColumn: "Id",
                keyValue: 7,
                column: "SizeName",
                value: "Size36");

            migrationBuilder.UpdateData(
                table: "Sizes",
                keyColumn: "Id",
                keyValue: 8,
                column: "SizeName",
                value: "Size38");

            migrationBuilder.UpdateData(
                table: "Sizes",
                keyColumn: "Id",
                keyValue: 9,
                column: "SizeName",
                value: "Size40");
        }
    }
}
