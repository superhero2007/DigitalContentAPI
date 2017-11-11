using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Digital_Content.DigitalContent.Migrations
{
    public partial class Add_UserFields_For_Dashboard_Migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AffricateTokensEarned",
                table: "AbpUsers",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DcntTokensBalance",
                table: "AbpUsers",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DcntTokensPurchased",
                table: "AbpUsers",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Downline",
                table: "AbpUsers",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DownlineTokenBough",
                table: "AbpUsers",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AffricateTokensEarned",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "DcntTokensBalance",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "DcntTokensPurchased",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "Downline",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "DownlineTokenBough",
                table: "AbpUsers");
        }
    }
}
