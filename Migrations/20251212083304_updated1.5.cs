using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Insulter.Migrations
{
    /// <inheritdoc />
    public partial class updated15 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApiNumber",
                table: "Insults");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Insults",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Insults",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ApiNumber",
                table: "Insults",
                type: "int",
                nullable: true);
        }
    }
}
