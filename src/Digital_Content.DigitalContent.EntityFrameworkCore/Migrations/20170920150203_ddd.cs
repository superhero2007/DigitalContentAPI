using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Digital_Content.DigitalContent.Migrations
{
    public partial class ddd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_AbpUsers_Company_CompanyUserId",
            //    table: "AbpUsers");

            migrationBuilder.RenameColumn(
                name: "CreationDate",
                table: "Company",
                newName: "CreationTime");

            migrationBuilder.AlterColumn<long>(
                name: "CompanyUserId",
                table: "AbpUsers",
                nullable: true,
                oldClrType: typeof(long));

            //migrationBuilder.AddForeignKey(
            //    name: "FK_AbpUsers_Company_CompanyUserId",
            //    table: "AbpUsers",
            //    column: "CompanyUserId",
            //    principalTable: "Company",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_AbpUsers_Company_CompanyUserId",
            //    table: "AbpUsers");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "Company",
                newName: "CreationDate");

            migrationBuilder.AlterColumn<long>(
                name: "CompanyUserId",
                table: "AbpUsers",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_AbpUsers_Company_CompanyUserId",
            //    table: "AbpUsers",
            //    column: "CompanyUserId",
            //    principalTable: "Company",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Cascade);
        }
    }
}
