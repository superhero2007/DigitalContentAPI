using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Digital_Content.DigitalContent.Migrations
{
    public partial class Add_New_Fields_Migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CMS",
                table: "AbpUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "AbpUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContentVolume",
                table: "AbpUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Industry",
                table: "AbpUsers",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsContentShedule",
                table: "AbpUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "AbpUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YourName",
                table: "AbpUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CMS",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "ContentVolume",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "Industry",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "IsContentShedule",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "YourName",
                table: "AbpUsers");
        }
    }
}
