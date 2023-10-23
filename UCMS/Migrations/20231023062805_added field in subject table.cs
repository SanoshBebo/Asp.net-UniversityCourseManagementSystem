using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UCMS.Migrations
{
    /// <inheritdoc />
    public partial class addedfieldinsubjecttable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TeachingHours",
                table: "Subjects",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TeachingHours",
                table: "Subjects");
        }
    }
}
