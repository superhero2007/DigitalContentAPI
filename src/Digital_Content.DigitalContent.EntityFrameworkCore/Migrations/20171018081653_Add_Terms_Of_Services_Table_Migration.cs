using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Digital_Content.DigitalContent.Migrations
{
    public partial class Add_Terms_Of_Services_Table_Migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TermsOfServices",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateRevised = table.Column<DateTime>(nullable: false),
                    RevicedById = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    TosContent = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TermsOfServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TermsOfServices_AbpUsers_RevicedById",
                        column: x => x.RevicedById,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TermsOfServices_RevicedById",
                table: "TermsOfServices",
                column: "RevicedById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TermsOfServices");
        }
    }
}
