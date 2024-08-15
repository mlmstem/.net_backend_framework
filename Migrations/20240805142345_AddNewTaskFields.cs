using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class AddNewTaskFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Feedback",
                table: "Tasks",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "HighPriority",
                table: "Tasks",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<float>(
                name: "Rating",
                table: "Tasks",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Feedback",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "HighPriority",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Tasks");
        }
    }
}
