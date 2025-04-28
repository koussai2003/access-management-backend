using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccessManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class behalfrequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActualUserEmail",
                table: "UserAccessRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsOnBehalfRequest",
                table: "UserAccessRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RequestedByEmail",
                table: "UserAccessRequests",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualUserEmail",
                table: "UserAccessRequests");

            migrationBuilder.DropColumn(
                name: "IsOnBehalfRequest",
                table: "UserAccessRequests");

            migrationBuilder.DropColumn(
                name: "RequestedByEmail",
                table: "UserAccessRequests");
        }
    }
}
