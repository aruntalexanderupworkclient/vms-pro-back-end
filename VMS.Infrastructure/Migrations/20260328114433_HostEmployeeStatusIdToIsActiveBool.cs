using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class HostEmployeeStatusIdToIsActiveBool : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_MdmUserStatuses_StatusId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Hosts_MdmUserStatuses_StatusId",
                table: "Hosts");

            migrationBuilder.DropIndex(
                name: "IX_Hosts_StatusId",
                table: "Hosts");

            migrationBuilder.DropIndex(
                name: "IX_Employees_StatusId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "Hosts");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "Employees");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Hosts",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Employees",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Hosts");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Employees");

            migrationBuilder.AddColumn<Guid>(
                name: "StatusId",
                table: "Hosts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "StatusId",
                table: "Employees",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Hosts_StatusId",
                table: "Hosts",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_StatusId",
                table: "Employees",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_MdmUserStatuses_StatusId",
                table: "Employees",
                column: "StatusId",
                principalTable: "MdmUserStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Hosts_MdmUserStatuses_StatusId",
                table: "Hosts",
                column: "StatusId",
                principalTable: "MdmUserStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
