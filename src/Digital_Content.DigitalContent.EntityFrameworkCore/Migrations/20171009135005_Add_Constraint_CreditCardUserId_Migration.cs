using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Digital_Content.DigitalContent.Migrations
{
    public partial class Add_Constraint_CreditCardUserId_Migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CreditCardUserId",
                table: "AbpUsers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpUsers_CreditCardUserId",
                table: "AbpUsers",
                column: "CreditCardUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AbpUsers_CreditCardPayment_CreditCardUserId",
                table: "AbpUsers",
                column: "CreditCardUserId",
                principalTable: "CreditCardPayment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbpUsers_CreditCardPayment_CreditCardUserId",
                table: "AbpUsers");

            migrationBuilder.DropIndex(
                name: "IX_AbpUsers_CreditCardUserId",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "CreditCardUserId",
                table: "AbpUsers");
        }
    }
}
