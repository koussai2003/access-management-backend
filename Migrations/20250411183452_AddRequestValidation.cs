using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccessManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddRequestValidation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ValidatedBy1",
                table: "UserAccessRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ValidatedBy2",
                table: "UserAccessRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ValidatedBy3",
                table: "UserAccessRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Validateur1",
                table: "UserAccessRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Validateur2",
                table: "UserAccessRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Validateur3",
                table: "UserAccessRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RequestValidations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    ValidateurEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValidatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestValidations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestValidations_UserAccessRequests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "UserAccessRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequestValidations_RequestId",
                table: "RequestValidations",
                column: "RequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestValidations");

            migrationBuilder.DropColumn(
                name: "ValidatedBy1",
                table: "UserAccessRequests");

            migrationBuilder.DropColumn(
                name: "ValidatedBy2",
                table: "UserAccessRequests");

            migrationBuilder.DropColumn(
                name: "ValidatedBy3",
                table: "UserAccessRequests");

            migrationBuilder.DropColumn(
                name: "Validateur1",
                table: "UserAccessRequests");

            migrationBuilder.DropColumn(
                name: "Validateur2",
                table: "UserAccessRequests");

            migrationBuilder.DropColumn(
                name: "Validateur3",
                table: "UserAccessRequests");
        }
    }
}
