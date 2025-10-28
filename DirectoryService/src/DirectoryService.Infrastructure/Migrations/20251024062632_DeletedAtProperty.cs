using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirectoryService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DeletedAtProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_departments_locations_departments_DepartmentId",
                table: "departments_locations");

            migrationBuilder.DropForeignKey(
                name: "FK_departments_locations_locations_LocationId",
                table: "departments_locations");

            migrationBuilder.DropForeignKey(
                name: "FK_departments_positions_departments_DepartmentId",
                table: "departments_positions");

            migrationBuilder.DropForeignKey(
                name: "FK_departments_positions_positions_PositionId",
                table: "departments_positions");

            migrationBuilder.RenameColumn(
                name: "PositionId",
                table: "departments_positions",
                newName: "position_id");

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                table: "departments_positions",
                newName: "department_id");

            migrationBuilder.RenameIndex(
                name: "IX_departments_positions_PositionId",
                table: "departments_positions",
                newName: "IX_departments_positions_position_id");

            migrationBuilder.RenameColumn(
                name: "LocationId",
                table: "departments_locations",
                newName: "location_id");

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                table: "departments_locations",
                newName: "department_id");

            migrationBuilder.RenameIndex(
                name: "IX_departments_locations_LocationId",
                table: "departments_locations",
                newName: "IX_departments_locations_location_id");

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "positions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "locations",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "departments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_departments_locations_departments_department_id",
                table: "departments_locations",
                column: "department_id",
                principalTable: "departments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_departments_locations_locations_location_id",
                table: "departments_locations",
                column: "location_id",
                principalTable: "locations",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_departments_positions_departments_department_id",
                table: "departments_positions",
                column: "department_id",
                principalTable: "departments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_departments_positions_positions_position_id",
                table: "departments_positions",
                column: "position_id",
                principalTable: "positions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_departments_locations_departments_department_id",
                table: "departments_locations");

            migrationBuilder.DropForeignKey(
                name: "FK_departments_locations_locations_location_id",
                table: "departments_locations");

            migrationBuilder.DropForeignKey(
                name: "FK_departments_positions_departments_department_id",
                table: "departments_positions");

            migrationBuilder.DropForeignKey(
                name: "FK_departments_positions_positions_position_id",
                table: "departments_positions");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "positions");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "locations");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "departments");

            migrationBuilder.RenameColumn(
                name: "position_id",
                table: "departments_positions",
                newName: "PositionId");

            migrationBuilder.RenameColumn(
                name: "department_id",
                table: "departments_positions",
                newName: "DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_departments_positions_position_id",
                table: "departments_positions",
                newName: "IX_departments_positions_PositionId");

            migrationBuilder.RenameColumn(
                name: "location_id",
                table: "departments_locations",
                newName: "LocationId");

            migrationBuilder.RenameColumn(
                name: "department_id",
                table: "departments_locations",
                newName: "DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_departments_locations_location_id",
                table: "departments_locations",
                newName: "IX_departments_locations_LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_departments_locations_departments_DepartmentId",
                table: "departments_locations",
                column: "DepartmentId",
                principalTable: "departments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_departments_locations_locations_LocationId",
                table: "departments_locations",
                column: "LocationId",
                principalTable: "locations",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_departments_positions_departments_DepartmentId",
                table: "departments_positions",
                column: "DepartmentId",
                principalTable: "departments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_departments_positions_positions_PositionId",
                table: "departments_positions",
                column: "PositionId",
                principalTable: "positions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
