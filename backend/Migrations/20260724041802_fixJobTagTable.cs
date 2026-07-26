using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class fixJobTagTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TagId1",
                table: "JobTags",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobTags_TagId1",
                table: "JobTags",
                column: "TagId1");

            migrationBuilder.AddForeignKey(
                name: "FK_JobTags_Tags_TagId1",
                table: "JobTags",
                column: "TagId1",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobTags_Tags_TagId1",
                table: "JobTags");

            migrationBuilder.DropIndex(
                name: "IX_JobTags_TagId1",
                table: "JobTags");

            migrationBuilder.DropColumn(
                name: "TagId1",
                table: "JobTags");
        }
    }
}
