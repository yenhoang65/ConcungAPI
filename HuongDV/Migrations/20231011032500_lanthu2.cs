using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HuongDV.Migrations
{
    /// <inheritdoc />
    public partial class lanthu2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Subject",
                table: "contacts");

            migrationBuilder.AddColumn<int>(
                name: "SubjectId",
                table: "contacts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "subjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subjects", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_contacts_SubjectId",
                table: "contacts",
                column: "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_contacts_subjects_SubjectId",
                table: "contacts",
                column: "SubjectId",
                principalTable: "subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_contacts_subjects_SubjectId",
                table: "contacts");

            migrationBuilder.DropTable(
                name: "subjects");

            migrationBuilder.DropIndex(
                name: "IX_contacts_SubjectId",
                table: "contacts");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "contacts");

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "contacts",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
