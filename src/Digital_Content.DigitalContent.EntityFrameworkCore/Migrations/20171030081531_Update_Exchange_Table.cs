using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Digital_Content.DigitalContent.Migrations
{
    public partial class Update_Exchange_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrencyType",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "CurrencyType",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "ExchangeRates");

            migrationBuilder.AddColumn<Guid>(
                name: "CurrencyId",
                table: "Transactions",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CurrencyId",
                table: "Payments",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CurrencyId",
                table: "ExchangeRates",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "DataSourceId",
                table: "ExchangeRates",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutomaticUpdate",
                table: "ExchangeRates",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedBy",
                table: "ExchangeRates",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "ExchangeRates");

            migrationBuilder.DropColumn(
                name: "DataSourceId",
                table: "ExchangeRates");

            migrationBuilder.DropColumn(
                name: "IsAutomaticUpdate",
                table: "ExchangeRates");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "ExchangeRates");

            migrationBuilder.AddColumn<int>(
                name: "CurrencyType",
                table: "Transactions",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyType",
                table: "Payments",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Currency",
                table: "ExchangeRates",
                nullable: false,
                defaultValue: 0);
        }
    }
}
