using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Digital_Content.DigitalContent.Migrations
{
    public partial class Add_wireInstruction_LogoUrl_Migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WiredInstruction",
                table: "AbpUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "WiredInstruction",
                table: "AbpUsers");
        }
    }
}
