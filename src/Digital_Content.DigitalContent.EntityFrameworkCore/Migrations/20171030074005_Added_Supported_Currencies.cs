using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Digital_Content.DigitalContent.Migrations
{
    public partial class Added_Supported_Currencies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSupported",
                table: "Currencies");

            migrationBuilder.CreateTable(
                name: "SupportedCurrencies",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CurrencyId = table.Column<Guid>(nullable: false),
                    SortOrder = table.Column<int>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    WalledId = table.Column<string>(nullable: true),
                    WiredInstruction = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportedCurrencies", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SupportedCurrencies");

            migrationBuilder.AddColumn<bool>(
                name: "IsSupported",
                table: "Currencies",
                nullable: false,
                defaultValue: false);
        }
    }
}
