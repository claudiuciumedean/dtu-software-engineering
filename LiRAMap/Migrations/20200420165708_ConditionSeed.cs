using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LiRAMap.Migrations
{
    public partial class ConditionSeed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Conditions",
                columns: new[] { "Id", "ConditionType", "Severity", "Timestamp" },
                values: new object[,]
                {
                    { 1, 3, 35f, new DateTime(2020, 4, 20, 18, 0, 7, 915, DateTimeKind.Local).AddTicks(3979) },
                    { 2, 9, 95f, new DateTime(2019, 7, 23, 12, 13, 7, 917, DateTimeKind.Utc).AddTicks(3207) },
                    { 3, 10, 53f, new DateTime(2020, 2, 13, 18, 57, 7, 917, DateTimeKind.Local).AddTicks(3217) },
                    { 4, 0, 100f, new DateTime(2020, 3, 9, 16, 57, 7, 917, DateTimeKind.Utc).AddTicks(3237) }
                });

            migrationBuilder.InsertData(
                table: "ConditionCoverages",
                columns: new[] { "Id", "ConditionId", "EndMeters", "StartMeters", "Way" },
                values: new object[,]
                {
                    { 1, 1, 150, 0, 27294498m },
                    { 2, 1, 250, 0, 26264463m },
                    { 3, 1, 1000, 575, 106151483m },
                    { 4, 2, 78, 52, 131196793m },
                    { 5, 3, null, 60, 131196793m },
                    { 6, 4, null, 10, 25657310m }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ConditionCoverages",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ConditionCoverages",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ConditionCoverages",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ConditionCoverages",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ConditionCoverages",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ConditionCoverages",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Conditions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Conditions",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Conditions",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Conditions",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
