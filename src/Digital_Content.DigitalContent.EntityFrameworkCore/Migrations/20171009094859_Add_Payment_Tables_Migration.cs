using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Digital_Content.DigitalContent.Migrations
{
    public partial class Add_Payment_Tables_Migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Card",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Brand = table.Column<string>(nullable: true),
                    CardId = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    ExpirationMonth = table.Column<int>(nullable: false),
                    ExpirationYear = table.Column<string>(nullable: true),
                    FingerPrint = table.Column<string>(nullable: true),
                    Funding = table.Column<string>(nullable: true),
                    Last4 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Card", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Outcome",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NetworkStatus = table.Column<string>(nullable: true),
                    RiskLevel = table.Column<string>(nullable: true),
                    SellerMessage = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Outcome", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Source",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BankAccount = table.Column<string>(nullable: true),
                    CardId = table.Column<long>(nullable: true),
                    SourceId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Source", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Source_Card_CardId",
                        column: x => x.CardId,
                        principalTable: "Card",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CreditCardPayment",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Amount = table.Column<long>(nullable: false),
                    BalanceTransactionId = table.Column<string>(nullable: true),
                    ChargeId = table.Column<string>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    Currency = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    OutcomeId = table.Column<long>(nullable: true),
                    SourceId = table.Column<long>(nullable: true),
                    Status = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditCardPayment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditCardPayment_Outcome_OutcomeId",
                        column: x => x.OutcomeId,
                        principalTable: "Outcome",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CreditCardPayment_Source_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Source",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardPayment_OutcomeId",
                table: "CreditCardPayment",
                column: "OutcomeId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardPayment_SourceId",
                table: "CreditCardPayment",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Source_CardId",
                table: "Source",
                column: "CardId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CreditCardPayment");

            migrationBuilder.DropTable(
                name: "Outcome");

            migrationBuilder.DropTable(
                name: "Source");

            migrationBuilder.DropTable(
                name: "Card");
        }
    }
}
