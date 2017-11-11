using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Digital_Content.DigitalContent.Migrations
{
    public partial class Fix_migrations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "TermsOfService",
                nullable: false,
                defaultValue: 0);  

            migrationBuilder.AlterColumn<decimal>(
                name: "AffricateTokensEarned",
                table: "AbpUsers",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "DcntTokensBalance",
                table: "AbpUsers",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "DcntTokensPurchased",
                table: "AbpUsers",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "Downline",
                table: "AbpUsers",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "DownlineTokenBough",
                table: "AbpUsers",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<long>(
                name: "TosVersion",
                table: "AbpUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    ImageUrl = table.Column<string>(nullable: true),
                    IsCrypto = table.Column<bool>(nullable: false),
                    IsSupported = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    SortOrder = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Currencies");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "TermsOfService");

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

            migrationBuilder.DropColumn(
                name: "TosVersion",
                table: "AbpUsers");
        }
    }
}
