using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Digital_Content.DigitalContent.Migrations
{
    public partial class Add_Update_Migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CurrentType",
                table: "Transactions",
                newName: "CurrencyType");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "Transactions",
                newName: "LastName");

            migrationBuilder.AddColumn<string>(
                name: "AmountDue",
                table: "Transactions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirrstName",
                table: "Transactions",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Transactions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "PaymentAmount",
                table: "Transactions",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TokensIssued",
                table: "Transactions",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "TransactionDate",
                table: "Transactions",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountDue",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "FirrstName",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "PaymentAmount",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "TokensIssued",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "TransactionDate",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "Transactions",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "CurrencyType",
                table: "Transactions",
                newName: "CurrentType");
        }
    }
}
