using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Digital_Content.DigitalContent.Migrations
{
    public partial class Change_WalletLogs_Table_Migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "WalletId",
                table: "WalletLogs",
                nullable: true,
                oldClrType: typeof(long));

            

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "WalletLogs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "WalletLogs");

            migrationBuilder.AlterColumn<long>(
                name: "WalletId",
                table: "WalletLogs",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

                   }
    }
}
