using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Digital_Content.DigitalContent.Migrations
{
    public partial class Alter_ExpirationYear_Migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpirationYear",
                table: "Card");

            migrationBuilder.AddColumn<long>(
                name: "ExpirationYear",
                table: "Card",
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "ExpirationYear",
                table: "Card");

            migrationBuilder.AddColumn<long>(
                name: "ExpirationYear",
                table: "Card",
                nullable: false);
        }
    }
}
