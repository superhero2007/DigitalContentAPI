using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Digital_Content.DigitalContent.Migrations
{
    public partial class Add_Currency_Description_Migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Currencies",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Currencies",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "Source",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "CreditCardPaymentLogs",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Currencies");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Currencies");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "CreditCardPaymentLogs");

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "Source",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
