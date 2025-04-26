using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccessManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAccessRequestFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Application",
                table: "UserAccessRequests");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "UserAccessRequests",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "RequestDate",
                table: "UserAccessRequests",
                newName: "SubmittedAt");

            migrationBuilder.RenameColumn(
                name: "Options",
                table: "UserAccessRequests",
                newName: "UserEmail");

            migrationBuilder.RenameColumn(
                name: "Module",
                table: "UserAccessRequests",
                newName: "State");

            migrationBuilder.RenameColumn(
                name: "Function",
                table: "UserAccessRequests",
                newName: "ModulesJson");

            migrationBuilder.RenameColumn(
                name: "Comment",
                table: "UserAccessRequests",
                newName: "ApplicationName");

            migrationBuilder.AddColumn<string>(
                name: "AdminComment",
                table: "UserAccessRequests",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminComment",
                table: "UserAccessRequests");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "UserAccessRequests",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "UserEmail",
                table: "UserAccessRequests",
                newName: "Options");

            migrationBuilder.RenameColumn(
                name: "SubmittedAt",
                table: "UserAccessRequests",
                newName: "RequestDate");

            migrationBuilder.RenameColumn(
                name: "State",
                table: "UserAccessRequests",
                newName: "Module");

            migrationBuilder.RenameColumn(
                name: "ModulesJson",
                table: "UserAccessRequests",
                newName: "Function");

            migrationBuilder.RenameColumn(
                name: "ApplicationName",
                table: "UserAccessRequests",
                newName: "Comment");

            migrationBuilder.AddColumn<string>(
                name: "Application",
                table: "UserAccessRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
