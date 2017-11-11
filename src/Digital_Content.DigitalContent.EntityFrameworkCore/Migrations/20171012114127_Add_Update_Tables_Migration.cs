using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Digital_Content.DigitalContent.Migrations
{
    public partial class Add_Update_Tables_Migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreditCardPayment_AbpUsers_CreditCardUserId",
                table: "CreditCardPayment");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditCardPayment_Outcome_OutcomeId",
                table: "CreditCardPayment");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditCardPayment_Source_SourceId",
                table: "CreditCardPayment");

            migrationBuilder.DropTable(
                name: "WalletLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CreditCardPayment",
                table: "CreditCardPayment");

            migrationBuilder.RenameTable(
                name: "CreditCardPayment",
                newName: "CreditCardPaymentLogs");

            migrationBuilder.RenameIndex(
                name: "IX_CreditCardPayment_SourceId",
                table: "CreditCardPaymentLogs",
                newName: "IX_CreditCardPaymentLogs_SourceId");

            migrationBuilder.RenameIndex(
                name: "IX_CreditCardPayment_OutcomeId",
                table: "CreditCardPaymentLogs",
                newName: "IX_CreditCardPaymentLogs_OutcomeId");

            migrationBuilder.RenameIndex(
                name: "IX_CreditCardPayment_CreditCardUserId",
                table: "CreditCardPaymentLogs",
                newName: "IX_CreditCardPaymentLogs_CreditCardUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CreditCardPaymentLogs",
                table: "CreditCardPaymentLogs",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Address = table.Column<string>(nullable: true),
                    Amount = table.Column<string>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CurrentType = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    TransactionStatus = table.Column<int>(nullable: false),
                    UserId = table.Column<long>(nullable: true),
                    WalletId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCardPaymentLogs_AbpUsers_CreditCardUserId",
                table: "CreditCardPaymentLogs",
                column: "CreditCardUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCardPaymentLogs_Outcome_OutcomeId",
                table: "CreditCardPaymentLogs",
                column: "OutcomeId",
                principalTable: "Outcome",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCardPaymentLogs_Source_SourceId",
                table: "CreditCardPaymentLogs",
                column: "SourceId",
                principalTable: "Source",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreditCardPaymentLogs_AbpUsers_CreditCardUserId",
                table: "CreditCardPaymentLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditCardPaymentLogs_Outcome_OutcomeId",
                table: "CreditCardPaymentLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditCardPaymentLogs_Source_SourceId",
                table: "CreditCardPaymentLogs");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CreditCardPaymentLogs",
                table: "CreditCardPaymentLogs");

            migrationBuilder.RenameTable(
                name: "CreditCardPaymentLogs",
                newName: "CreditCardPayment");

            migrationBuilder.RenameIndex(
                name: "IX_CreditCardPaymentLogs_SourceId",
                table: "CreditCardPayment",
                newName: "IX_CreditCardPayment_SourceId");

            migrationBuilder.RenameIndex(
                name: "IX_CreditCardPaymentLogs_OutcomeId",
                table: "CreditCardPayment",
                newName: "IX_CreditCardPayment_OutcomeId");

            migrationBuilder.RenameIndex(
                name: "IX_CreditCardPaymentLogs_CreditCardUserId",
                table: "CreditCardPayment",
                newName: "IX_CreditCardPayment_CreditCardUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CreditCardPayment",
                table: "CreditCardPayment",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "WalletLogs",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Address = table.Column<string>(nullable: true),
                    Amount = table.Column<string>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CurrentType = table.Column<int>(nullable: false),
                    UserId = table.Column<long>(nullable: true),
                    WalletId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletLogs", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCardPayment_AbpUsers_CreditCardUserId",
                table: "CreditCardPayment",
                column: "CreditCardUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCardPayment_Outcome_OutcomeId",
                table: "CreditCardPayment",
                column: "OutcomeId",
                principalTable: "Outcome",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCardPayment_Source_SourceId",
                table: "CreditCardPayment",
                column: "SourceId",
                principalTable: "Source",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
