using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Digital_Content.DigitalContent.Migrations
{
    public partial class Update_User__Payments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Transactions_TransactionId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_TransactionId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PaymentTransactionId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "Payments");

            migrationBuilder.AddColumn<long>(
                name: "AmountTokens",
                table: "AbpUsers",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "PaymentTransaction",
                columns: table => new
                {
                    PaymentId = table.Column<long>(nullable: false),
                    TransactionId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTransaction", x => new { x.PaymentId, x.TransactionId });
                    table.ForeignKey(
                        name: "FK_PaymentTransaction_Payments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentTransaction_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransaction_TransactionId",
                table: "PaymentTransaction",
                column: "TransactionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentTransaction");

            migrationBuilder.DropColumn(
                name: "AmountTokens",
                table: "AbpUsers");

            migrationBuilder.AddColumn<long>(
                name: "PaymentTransactionId",
                table: "Transactions",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TransactionId",
                table: "Payments",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_TransactionId",
                table: "Payments",
                column: "TransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Transactions_TransactionId",
                table: "Payments",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
