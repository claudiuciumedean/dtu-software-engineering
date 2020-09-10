using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LiRAMap.Migrations
{
    public partial class RenameSeverityToValue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Severity",
                table: "Conditions");

            migrationBuilder.AddColumn<float>(
                name: "Value",
                table: "Conditions",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.UpdateData(
                table: "Conditions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Timestamp", "Value" },
                values: new object[] { new DateTime(2020, 5, 11, 21, 32, 12, 825, DateTimeKind.Local).AddTicks(6739), 35f });

            migrationBuilder.UpdateData(
                table: "Conditions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Timestamp", "Value" },
                values: new object[] { new DateTime(2020, 5, 11, 19, 32, 12, 827, DateTimeKind.Utc).AddTicks(6663), 95f });

            migrationBuilder.UpdateData(
                table: "Conditions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Timestamp", "Value" },
                values: new object[] { new DateTime(2020, 5, 11, 21, 32, 12, 827, DateTimeKind.Local).AddTicks(6674), 53f });

            migrationBuilder.UpdateData(
                table: "Conditions",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Timestamp", "Value" },
                values: new object[] { new DateTime(2020, 5, 11, 19, 32, 12, 827, DateTimeKind.Utc).AddTicks(6697), 100f });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Value",
                table: "Conditions");

            migrationBuilder.AddColumn<float>(
                name: "Severity",
                table: "Conditions",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.UpdateData(
                table: "Conditions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Severity", "Timestamp" },
                values: new object[] { 35f, new DateTime(2020, 4, 20, 18, 57, 7, 915, DateTimeKind.Local).AddTicks(3979) });

            migrationBuilder.UpdateData(
                table: "Conditions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Severity", "Timestamp" },
                values: new object[] { 95f, new DateTime(2020, 4, 20, 16, 57, 7, 917, DateTimeKind.Utc).AddTicks(3207) });

            migrationBuilder.UpdateData(
                table: "Conditions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Severity", "Timestamp" },
                values: new object[] { 53f, new DateTime(2020, 4, 20, 18, 57, 7, 917, DateTimeKind.Local).AddTicks(3217) });

            migrationBuilder.UpdateData(
                table: "Conditions",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Severity", "Timestamp" },
                values: new object[] { 100f, new DateTime(2020, 4, 20, 16, 57, 7, 917, DateTimeKind.Utc).AddTicks(3237) });
        }
    }
}
