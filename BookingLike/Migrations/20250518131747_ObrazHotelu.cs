﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingLike.Migrations
{
    /// <inheritdoc />
    public partial class ObrazHotelu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Hotels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Hotels");
        }
    }
}
