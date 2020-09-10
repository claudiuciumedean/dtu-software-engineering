using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace LiRAMap.Migrations
{
    public partial class ConditionCoverageNavigation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ways",
                table: "Conditions");

            migrationBuilder.CreateTable(
                name: "ConditionCoverages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Way = table.Column<decimal>(nullable: false),
                    StartMeters = table.Column<int>(nullable: false),
                    EndMeters = table.Column<int>(nullable: true),
                    ConditionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConditionCoverages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConditionCoverages_Conditions_ConditionId",
                        column: x => x.ConditionId,
                        principalTable: "Conditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConditionCoverages_ConditionId",
                table: "ConditionCoverages",
                column: "ConditionId");

            migrationBuilder.CreateIndex(
                name: "IX_ConditionCoverages_Way",
                table: "ConditionCoverages",
                column: "Way");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConditionCoverages");

            migrationBuilder.AddColumn<string>(
                name: "Ways",
                table: "Conditions",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
