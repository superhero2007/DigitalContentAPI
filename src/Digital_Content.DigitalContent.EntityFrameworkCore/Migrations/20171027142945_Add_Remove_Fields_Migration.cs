using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Digital_Content.DigitalContent.Migrations
{
    public partial class Add_Remove_Fields_Migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Currencies");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Currencies");

            migrationBuilder.AddColumn<string>(
                name: "CompanyTenant",
                table: "AbpUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TokenDescription",
                table: "AbpUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TokenName",
                table: "AbpUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyTenant",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "TokenDescription",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "TokenName",
                table: "AbpUsers");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Currencies",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Currencies",
                nullable: true);
        }
    }
}
