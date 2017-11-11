using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Digital_Content.DigitalContent.Migrations
{
    public partial class FixedWalletMisspelling : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WalledId",
                table: "SupportedCurrencies",
                newName: "WalletId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WalletId",
                table: "SupportedCurrencies",
                newName: "WalledId");
        }
    }
}
