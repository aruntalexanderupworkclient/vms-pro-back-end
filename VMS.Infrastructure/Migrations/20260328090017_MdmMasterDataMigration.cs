using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MdmMasterDataMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "VisitorTokens");

            migrationBuilder.DropColumn(
                name: "TokenType",
                table: "VisitorTokens");

            migrationBuilder.DropColumn(
                name: "OrgType",
                table: "Visitors");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Visitors");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Organisations");

            migrationBuilder.DropColumn(
                name: "OrganisationType",
                table: "Hosts");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Hosts");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Appointments");

            migrationBuilder.AddColumn<Guid>(
                name: "StatusId",
                table: "VisitorTokens",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TokenTypeId",
                table: "VisitorTokens",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "OrgTypeId",
                table: "Visitors",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StatusId",
                table: "Visitors",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "StatusId",
                table: "Users",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TypeId",
                table: "Organisations",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "OrganisationTypeId",
                table: "Hosts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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

            migrationBuilder.AddColumn<Guid>(
                name: "StatusId",
                table: "Appointments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "MdmOrganisationTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Value = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MdmOrganisationTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MdmOrganisationTypes_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MdmOrganisationTypes_Users_DeletedBy",
                        column: x => x.DeletedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MdmOrganisationTypes_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MdmTokenTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Value = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MdmTokenTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MdmTokenTypes_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MdmTokenTypes_Users_DeletedBy",
                        column: x => x.DeletedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MdmTokenTypes_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MdmUserStatuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Value = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MdmUserStatuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MdmUserStatuses_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MdmUserStatuses_Users_DeletedBy",
                        column: x => x.DeletedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MdmUserStatuses_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MdmVisitStatuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Value = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MdmVisitStatuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MdmVisitStatuses_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MdmVisitStatuses_Users_DeletedBy",
                        column: x => x.DeletedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MdmVisitStatuses_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VisitorTokens_StatusId",
                table: "VisitorTokens",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitorTokens_TokenTypeId",
                table: "VisitorTokens",
                column: "TokenTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Visitors_OrgTypeId",
                table: "Visitors",
                column: "OrgTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Visitors_StatusId",
                table: "Visitors",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_StatusId",
                table: "Users",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Organisations_TypeId",
                table: "Organisations",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Hosts_OrganisationTypeId",
                table: "Hosts",
                column: "OrganisationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Hosts_StatusId",
                table: "Hosts",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_StatusId",
                table: "Employees",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_StatusId",
                table: "Appointments",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_MdmOrganisationTypes_Code",
                table: "MdmOrganisationTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MdmOrganisationTypes_CreatedBy",
                table: "MdmOrganisationTypes",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MdmOrganisationTypes_DeletedBy",
                table: "MdmOrganisationTypes",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MdmOrganisationTypes_UpdatedBy",
                table: "MdmOrganisationTypes",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MdmTokenTypes_Code",
                table: "MdmTokenTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MdmTokenTypes_CreatedBy",
                table: "MdmTokenTypes",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MdmTokenTypes_DeletedBy",
                table: "MdmTokenTypes",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MdmTokenTypes_UpdatedBy",
                table: "MdmTokenTypes",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MdmUserStatuses_Code",
                table: "MdmUserStatuses",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MdmUserStatuses_CreatedBy",
                table: "MdmUserStatuses",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MdmUserStatuses_DeletedBy",
                table: "MdmUserStatuses",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MdmUserStatuses_UpdatedBy",
                table: "MdmUserStatuses",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MdmVisitStatuses_Code",
                table: "MdmVisitStatuses",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MdmVisitStatuses_CreatedBy",
                table: "MdmVisitStatuses",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MdmVisitStatuses_DeletedBy",
                table: "MdmVisitStatuses",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MdmVisitStatuses_UpdatedBy",
                table: "MdmVisitStatuses",
                column: "UpdatedBy");

            // ═══════════════════════════════════════════════════════════
            // SEED DATA — populate MDM tables with standard values
            // ═══════════════════════════════════════════════════════════
            var now = new DateTime(2026, 3, 28, 0, 0, 0, DateTimeKind.Utc);

            // Visit Statuses
            foreach (var (code, value, order) in new[]
            {
                ("Scheduled", "Scheduled", 1),
                ("CheckedIn", "Checked In", 2),
                ("CheckedOut", "Checked Out", 3),
                ("Cancelled", "Cancelled", 4),
                ("Expired", "Expired", 5)
            })
            {
                migrationBuilder.InsertData(
                    table: "MdmVisitStatuses",
                    columns: new[] { "Id", "Code", "Value", "SortOrder", "IsActive", "CreatedAt", "IsDeleted" },
                    values: new object[] { Guid.NewGuid(), code, value, order, true, now, false });
            }

            // User Statuses
            foreach (var (code, value, order) in new[]
            {
                ("Active", "Active", 1),
                ("Inactive", "Inactive", 2),
                ("Suspended", "Suspended", 3),
                ("Deleted", "Deleted", 4)
            })
            {
                migrationBuilder.InsertData(
                    table: "MdmUserStatuses",
                    columns: new[] { "Id", "Code", "Value", "SortOrder", "IsActive", "CreatedAt", "IsDeleted" },
                    values: new object[] { Guid.NewGuid(), code, value, order, true, now, false });
            }

            // Token Types
            foreach (var (code, value, order) in new[]
            {
                ("Visitor", "Visitor", 1),
                ("Contractor", "Contractor", 2),
                ("Delivery", "Delivery", 3),
                ("Temporary", "Temporary", 4),
                ("VIP", "VIP", 5),
                ("Guest", "Guest", 6)
            })
            {
                migrationBuilder.InsertData(
                    table: "MdmTokenTypes",
                    columns: new[] { "Id", "Code", "Value", "SortOrder", "IsActive", "CreatedAt", "IsDeleted" },
                    values: new object[] { Guid.NewGuid(), code, value, order, true, now, false });
            }

            // Organisation Types
            foreach (var (code, value, order) in new[]
            {
                ("Hospital", "Hospital", 1),
                ("Residential", "Residential", 2),
                ("Corporate", "Corporate", 3),
                ("Factory", "Factory", 4)
            })
            {
                migrationBuilder.InsertData(
                    table: "MdmOrganisationTypes",
                    columns: new[] { "Id", "Code", "Value", "SortOrder", "IsActive", "CreatedAt", "IsDeleted" },
                    values: new object[] { Guid.NewGuid(), code, value, order, true, now, false });
            }

            // ═══════════════════════════════════════════════════════════
            // FOREIGN KEYS
            // ═══════════════════════════════════════════════════════════

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_MdmVisitStatuses_StatusId",
                table: "Appointments",
                column: "StatusId",
                principalTable: "MdmVisitStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_MdmUserStatuses_StatusId",
                table: "Employees",
                column: "StatusId",
                principalTable: "MdmUserStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Hosts_MdmOrganisationTypes_OrganisationTypeId",
                table: "Hosts",
                column: "OrganisationTypeId",
                principalTable: "MdmOrganisationTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Hosts_MdmUserStatuses_StatusId",
                table: "Hosts",
                column: "StatusId",
                principalTable: "MdmUserStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Organisations_MdmOrganisationTypes_TypeId",
                table: "Organisations",
                column: "TypeId",
                principalTable: "MdmOrganisationTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_MdmUserStatuses_StatusId",
                table: "Users",
                column: "StatusId",
                principalTable: "MdmUserStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Visitors_MdmOrganisationTypes_OrgTypeId",
                table: "Visitors",
                column: "OrgTypeId",
                principalTable: "MdmOrganisationTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Visitors_MdmVisitStatuses_StatusId",
                table: "Visitors",
                column: "StatusId",
                principalTable: "MdmVisitStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VisitorTokens_MdmTokenTypes_TokenTypeId",
                table: "VisitorTokens",
                column: "TokenTypeId",
                principalTable: "MdmTokenTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VisitorTokens_MdmVisitStatuses_StatusId",
                table: "VisitorTokens",
                column: "StatusId",
                principalTable: "MdmVisitStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_MdmVisitStatuses_StatusId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_MdmUserStatuses_StatusId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Hosts_MdmOrganisationTypes_OrganisationTypeId",
                table: "Hosts");

            migrationBuilder.DropForeignKey(
                name: "FK_Hosts_MdmUserStatuses_StatusId",
                table: "Hosts");

            migrationBuilder.DropForeignKey(
                name: "FK_Organisations_MdmOrganisationTypes_TypeId",
                table: "Organisations");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_MdmUserStatuses_StatusId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Visitors_MdmOrganisationTypes_OrgTypeId",
                table: "Visitors");

            migrationBuilder.DropForeignKey(
                name: "FK_Visitors_MdmVisitStatuses_StatusId",
                table: "Visitors");

            migrationBuilder.DropForeignKey(
                name: "FK_VisitorTokens_MdmTokenTypes_TokenTypeId",
                table: "VisitorTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_VisitorTokens_MdmVisitStatuses_StatusId",
                table: "VisitorTokens");

            migrationBuilder.DropTable(
                name: "MdmOrganisationTypes");

            migrationBuilder.DropTable(
                name: "MdmTokenTypes");

            migrationBuilder.DropTable(
                name: "MdmUserStatuses");

            migrationBuilder.DropTable(
                name: "MdmVisitStatuses");

            migrationBuilder.DropIndex(
                name: "IX_VisitorTokens_StatusId",
                table: "VisitorTokens");

            migrationBuilder.DropIndex(
                name: "IX_VisitorTokens_TokenTypeId",
                table: "VisitorTokens");

            migrationBuilder.DropIndex(
                name: "IX_Visitors_OrgTypeId",
                table: "Visitors");

            migrationBuilder.DropIndex(
                name: "IX_Visitors_StatusId",
                table: "Visitors");

            migrationBuilder.DropIndex(
                name: "IX_Users_StatusId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Organisations_TypeId",
                table: "Organisations");

            migrationBuilder.DropIndex(
                name: "IX_Hosts_OrganisationTypeId",
                table: "Hosts");

            migrationBuilder.DropIndex(
                name: "IX_Hosts_StatusId",
                table: "Hosts");

            migrationBuilder.DropIndex(
                name: "IX_Employees_StatusId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_StatusId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "VisitorTokens");

            migrationBuilder.DropColumn(
                name: "TokenTypeId",
                table: "VisitorTokens");

            migrationBuilder.DropColumn(
                name: "OrgTypeId",
                table: "Visitors");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "Visitors");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "Organisations");

            migrationBuilder.DropColumn(
                name: "OrganisationTypeId",
                table: "Hosts");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "Hosts");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "Appointments");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "VisitorTokens",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TokenType",
                table: "VisitorTokens",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OrgType",
                table: "Visitors",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Visitors",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Users",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Organisations",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OrganisationType",
                table: "Hosts",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Hosts",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Employees",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Appointments",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }
    }
}
