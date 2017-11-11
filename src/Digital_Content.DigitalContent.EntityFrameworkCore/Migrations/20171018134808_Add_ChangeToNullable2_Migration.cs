using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Digital_Content.DigitalContent.Migrations
{
    public partial class Add_ChangeToNullable2_Migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TermsOfServices_AbpUsers_RevicedById",
                table: "TermsOfServices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TermsOfServices",
                table: "TermsOfServices");

            migrationBuilder.RenameTable(
                name: "TermsOfServices",
                newName: "TermsOfService");

            migrationBuilder.RenameIndex(
                name: "IX_TermsOfServices_RevicedById",
                table: "TermsOfService",
                newName: "IX_TermsOfService_RevicedById");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TermsOfService",
                table: "TermsOfService",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TermsOfService_AbpUsers_RevicedById",
                table: "TermsOfService",
                column: "RevicedById",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TermsOfService_AbpUsers_RevicedById",
                table: "TermsOfService");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TermsOfService",
                table: "TermsOfService");

            migrationBuilder.RenameTable(
                name: "TermsOfService",
                newName: "TermsOfServices");

            migrationBuilder.RenameIndex(
                name: "IX_TermsOfService_RevicedById",
                table: "TermsOfServices",
                newName: "IX_TermsOfServices_RevicedById");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TermsOfServices",
                table: "TermsOfServices",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TermsOfServices_AbpUsers_RevicedById",
                table: "TermsOfServices",
                column: "RevicedById",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
