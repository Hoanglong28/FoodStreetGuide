using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace doanC_Admin.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNewFieldsToLocationPoint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "LocationPoints",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "LocationPoints",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "LocationPoints",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "LocationPoints",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OpeningHours",
                table: "LocationPoints",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PriceRange",
                table: "LocationPoints",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "Rating",
                table: "LocationPoints",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "ReviewCount",
                table: "LocationPoints",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "LocationPoints",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "LocationPoints");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "LocationPoints");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "LocationPoints");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "LocationPoints");

            migrationBuilder.DropColumn(
                name: "OpeningHours",
                table: "LocationPoints");

            migrationBuilder.DropColumn(
                name: "PriceRange",
                table: "LocationPoints");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "LocationPoints");

            migrationBuilder.DropColumn(
                name: "ReviewCount",
                table: "LocationPoints");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "LocationPoints");
        }
    }
}
