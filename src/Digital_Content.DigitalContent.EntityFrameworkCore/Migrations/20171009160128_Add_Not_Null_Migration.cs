using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Digital_Content.DigitalContent.Migrations
{
    public partial class Add_Not_Null_Migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreditCardPayment_AbpUsers_CreditCardUserId",
                table: "CreditCardPayment");

            migrationBuilder.AlterColumn<long>(
                name: "CreditCardUserId",
                table: "CreditCardPayment",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCardPayment_AbpUsers_CreditCardUserId",
                table: "CreditCardPayment",
                column: "CreditCardUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreditCardPayment_AbpUsers_CreditCardUserId",
                table: "CreditCardPayment");

            migrationBuilder.AlterColumn<long>(
                name: "CreditCardUserId",
                table: "CreditCardPayment",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCardPayment_AbpUsers_CreditCardUserId",
                table: "CreditCardPayment",
                column: "CreditCardUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
