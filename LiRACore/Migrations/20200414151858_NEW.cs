using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace LiRACore.Migrations
{
    public partial class NEW : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MeasurementTypes",
                columns: table => new
                {
                    MeasurementTypeId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type = table.Column<string>(nullable: true),
                    Created_Date = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasurementTypes", x => x.MeasurementTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Roads",
                columns: table => new
                {
                    Road_Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoadName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roads", x => x.Road_Id);
                });

            migrationBuilder.CreateTable(
                name: "SourceTypes",
                columns: table => new
                {
                    SourceTypeId = table.Column<Guid>(nullable: false),
                    Created_Date = table.Column<DateTimeOffset>(nullable: false),
                    Updated_Date = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SourceTypes", x => x.SourceTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Sections",
                columns: table => new
                {
                    OSMWayPointId = table.Column<Guid>(nullable: false),
                    FK_Road = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sections", x => x.OSMWayPointId);
                    table.ForeignKey(
                        name: "FK_Sections_Roads_FK_Road",
                        column: x => x.FK_Road,
                        principalTable: "Roads",
                        principalColumn: "Road_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    DeviceId = table.Column<Guid>(nullable: false),
                    Created_Date = table.Column<DateTimeOffset>(nullable: false),
                    Updated_Date = table.Column<DateTimeOffset>(nullable: false),
                    FK_SourceType = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.DeviceId);
                    table.ForeignKey(
                        name: "FK_Devices_SourceTypes_FK_SourceType",
                        column: x => x.FK_SourceType,
                        principalTable: "SourceTypes",
                        principalColumn: "SourceTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Nodes",
                columns: table => new
                {
                    NodeId = table.Column<Guid>(nullable: false),
                    lat = table.Column<float>(nullable: false),
                    lon = table.Column<float>(nullable: false),
                    FK_Section = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nodes", x => x.NodeId);
                    table.ForeignKey(
                        name: "FK_Nodes_Sections_FK_Section",
                        column: x => x.FK_Section,
                        principalTable: "Sections",
                        principalColumn: "OSMWayPointId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Trips",
                columns: table => new
                {
                    TripId = table.Column<Guid>(nullable: false),
                    StartTimeUtc = table.Column<DateTimeOffset>(nullable: true),
                    EndTimeUtc = table.Column<DateTimeOffset>(nullable: true),
                    StartPositionLat = table.Column<string>(nullable: true),
                    StartPositionLng = table.Column<string>(nullable: true),
                    StartPositionDisplay = table.Column<string>(nullable: true),
                    EndPositionLat = table.Column<string>(nullable: true),
                    EndPositionLng = table.Column<string>(nullable: true),
                    EndPositionDisplay = table.Column<string>(nullable: true),
                    Duration = table.Column<DateTimeOffset>(nullable: true),
                    DistanceKm = table.Column<double>(nullable: true),
                    FK_Device = table.Column<Guid>(nullable: false),
                    Created_Date = table.Column<DateTimeOffset>(nullable: false),
                    Updated_Date = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trips", x => x.TripId);
                    table.ForeignKey(
                        name: "FK_Trips_Devices_FK_Device",
                        column: x => x.FK_Device,
                        principalTable: "Devices",
                        principalColumn: "DeviceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DRDMeasurements",
                columns: table => new
                {
                    DRDMeasurementId = table.Column<Guid>(nullable: false),
                    TS_or_Distance = table.Column<string>(nullable: true),
                    T = table.Column<string>(nullable: true),
                    lat = table.Column<float>(nullable: true),
                    lon = table.Column<float>(nullable: true),
                    message = table.Column<string>(nullable: true),
                    isComputed = table.Column<bool>(nullable: false),
                    FK_Trip = table.Column<Guid>(nullable: false),
                    FK_MeasurementType = table.Column<int>(nullable: false),
                    Created_Date = table.Column<DateTimeOffset>(nullable: false),
                    Updated_Date = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DRDMeasurements", x => x.DRDMeasurementId);
                    table.ForeignKey(
                        name: "FK_DRDMeasurements_MeasurementTypes_FK_MeasurementType",
                        column: x => x.FK_MeasurementType,
                        principalTable: "MeasurementTypes",
                        principalColumn: "MeasurementTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DRDMeasurements_Trips_FK_Trip",
                        column: x => x.FK_Trip,
                        principalTable: "Trips",
                        principalColumn: "TripId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Measurements",
                columns: table => new
                {
                    MeasurementId = table.Column<Guid>(nullable: false),
                    TS_or_Distance = table.Column<DateTimeOffset>(nullable: false),
                    T = table.Column<string>(nullable: true),
                    lat = table.Column<float>(nullable: true),
                    lon = table.Column<float>(nullable: true),
                    message = table.Column<string>(nullable: true),
                    isComputed = table.Column<bool>(nullable: false),
                    FK_Trip = table.Column<Guid>(nullable: false),
                    FK_MeasurementType = table.Column<int>(nullable: false),
                    Created_Date = table.Column<DateTimeOffset>(nullable: false),
                    Updated_Date = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Measurements", x => x.MeasurementId);
                    table.ForeignKey(
                        name: "FK_Measurements_MeasurementTypes_FK_MeasurementType",
                        column: x => x.FK_MeasurementType,
                        principalTable: "MeasurementTypes",
                        principalColumn: "MeasurementTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Measurements_Trips_FK_Trip",
                        column: x => x.FK_Trip,
                        principalTable: "Trips",
                        principalColumn: "TripId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DRDMapReferences",
                columns: table => new
                {
                    DRDMapReferenceId = table.Column<Guid>(nullable: false),
                    lat_MapMatched = table.Column<float>(nullable: true),
                    lon_MapMatched = table.Column<float>(nullable: true),
                    wayPointName = table.Column<string>(nullable: true),
                    legSummary_MapMatched = table.Column<string>(nullable: true),
                    legDistance_MapMatched = table.Column<string>(nullable: true),
                    nodeId_MapMatched = table.Column<string>(nullable: true),
                    offset = table.Column<string>(nullable: true),
                    lane = table.Column<string>(nullable: true),
                    direction = table.Column<string>(nullable: true),
                    FK_DRDMeasurementId = table.Column<Guid>(nullable: false),
                    FK_OSMWayPointId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DRDMapReferences", x => x.DRDMapReferenceId);
                    table.ForeignKey(
                        name: "FK_DRDMapReferences_DRDMeasurements_FK_DRDMeasurementId",
                        column: x => x.FK_DRDMeasurementId,
                        principalTable: "DRDMeasurements",
                        principalColumn: "DRDMeasurementId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DRDMapReferences_Sections_FK_OSMWayPointId",
                        column: x => x.FK_OSMWayPointId,
                        principalTable: "Sections",
                        principalColumn: "OSMWayPointId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MapReferences",
                columns: table => new
                {
                    MapReferenceId = table.Column<Guid>(nullable: false),
                    lat_MapMatched = table.Column<float>(nullable: true),
                    lon_MapMatched = table.Column<float>(nullable: true),
                    wayPointName = table.Column<string>(nullable: true),
                    legSummary_MapMatched = table.Column<string>(nullable: true),
                    legDistance_MapMatched = table.Column<string>(nullable: true),
                    nodeId_MapMatched = table.Column<string>(nullable: true),
                    offset = table.Column<string>(nullable: true),
                    lane = table.Column<string>(nullable: true),
                    direction = table.Column<string>(nullable: true),
                    FK_MeasurementId = table.Column<Guid>(nullable: false),
                    FK_OSMWayPointId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapReferences", x => x.MapReferenceId);
                    table.ForeignKey(
                        name: "FK_MapReferences_Measurements_FK_MeasurementId",
                        column: x => x.FK_MeasurementId,
                        principalTable: "Measurements",
                        principalColumn: "MeasurementId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MapReferences_Sections_FK_OSMWayPointId",
                        column: x => x.FK_OSMWayPointId,
                        principalTable: "Sections",
                        principalColumn: "OSMWayPointId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Devices_FK_SourceType",
                table: "Devices",
                column: "FK_SourceType");

            migrationBuilder.CreateIndex(
                name: "IX_DRDMapReferences_FK_DRDMeasurementId",
                table: "DRDMapReferences",
                column: "FK_DRDMeasurementId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DRDMapReferences_FK_OSMWayPointId",
                table: "DRDMapReferences",
                column: "FK_OSMWayPointId");

            migrationBuilder.CreateIndex(
                name: "IX_DRDMeasurements_FK_MeasurementType",
                table: "DRDMeasurements",
                column: "FK_MeasurementType");

            migrationBuilder.CreateIndex(
                name: "IX_DRDMeasurements_FK_Trip",
                table: "DRDMeasurements",
                column: "FK_Trip");

            migrationBuilder.CreateIndex(
                name: "IX_MapReferences_FK_MeasurementId",
                table: "MapReferences",
                column: "FK_MeasurementId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MapReferences_FK_OSMWayPointId",
                table: "MapReferences",
                column: "FK_OSMWayPointId");

            migrationBuilder.CreateIndex(
                name: "IX_Measurements_FK_MeasurementType",
                table: "Measurements",
                column: "FK_MeasurementType");

            migrationBuilder.CreateIndex(
                name: "IX_Measurements_FK_Trip",
                table: "Measurements",
                column: "FK_Trip");

            migrationBuilder.CreateIndex(
                name: "IX_Nodes_FK_Section",
                table: "Nodes",
                column: "FK_Section");

            migrationBuilder.CreateIndex(
                name: "IX_Sections_FK_Road",
                table: "Sections",
                column: "FK_Road");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_FK_Device",
                table: "Trips",
                column: "FK_Device");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DRDMapReferences");

            migrationBuilder.DropTable(
                name: "MapReferences");

            migrationBuilder.DropTable(
                name: "Nodes");

            migrationBuilder.DropTable(
                name: "DRDMeasurements");

            migrationBuilder.DropTable(
                name: "Measurements");

            migrationBuilder.DropTable(
                name: "Sections");

            migrationBuilder.DropTable(
                name: "MeasurementTypes");

            migrationBuilder.DropTable(
                name: "Trips");

            migrationBuilder.DropTable(
                name: "Roads");

            migrationBuilder.DropTable(
                name: "Devices");

            migrationBuilder.DropTable(
                name: "SourceTypes");
        }
    }
}
