using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class colors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ColorCode", "ColorName" },
                values: new object[] { "#000000", "Black" });

            migrationBuilder.UpdateData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ColorCode", "ColorName" },
                values: new object[] { "#ffffff", "White" });

            migrationBuilder.UpdateData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 3,
                column: "ColorName",
                value: "Red");

            migrationBuilder.UpdateData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ColorCode", "ColorName" },
                values: new object[] { "#4169e1", "Blue" });

            migrationBuilder.InsertData(
                table: "Colors",
                columns: new[] { "Id", "ColorCode", "ColorName" },
                values: new object[,]
                {
                    { 5, "#7ea122", "Avocado" },
                    { 6, "#ede8d0", "Beige" },
                    { 7, "#964B00", "Brown" },
                    { 8, "#152238", "MidNight" },
                    { 9, "#808080", "Grey" },
                    { 10, "#003200", "DarkGreen" },
                    { 11, "#d1e5f4", "BabyBlue" },
                    { 12, "#ff69b4", "Pink" },
                    { 13, "#fffff2", "OffWhite" }
                });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "MenT-Shirt");

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "MenHoddie");

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "MenShort");

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "WomenT-Shirt");

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "WomenHoddie");

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 7,
                column: "Name",
                value: "UnisexHoddie");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.UpdateData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ColorCode", "ColorName" },
                values: new object[] { "#000", "black" });

            migrationBuilder.UpdateData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ColorCode", "ColorName" },
                values: new object[] { "#fff", "white" });

            migrationBuilder.UpdateData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 3,
                column: "ColorName",
                value: "red");

            migrationBuilder.UpdateData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ColorCode", "ColorName" },
                values: new object[] { "#0000FF", "blue" });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "T-Shirt");

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Hoddie");

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Short");

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "T-Shirt");

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "Hoddie");

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 7,
                column: "Name",
                value: "Hoddie");
        }
    }
}
