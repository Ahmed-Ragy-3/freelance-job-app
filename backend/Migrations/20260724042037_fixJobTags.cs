using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class fixJobTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobTags_Jobs_JobId1",
                table: "JobTags");

            migrationBuilder.DropForeignKey(
                name: "FK_JobTags_Tags_TagId1",
                table: "JobTags");

            migrationBuilder.DropIndex(
                name: "IX_JobTags_JobId1",
                table: "JobTags");

            migrationBuilder.DropIndex(
                name: "IX_JobTags_TagId1",
                table: "JobTags");

            migrationBuilder.DropColumn(
                name: "JobId1",
                table: "JobTags");

            migrationBuilder.DropColumn(
                name: "TagId1",
                table: "JobTags");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "JobId1",
                table: "JobTags",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TagId1",
                table: "JobTags",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobTags_JobId1",
                table: "JobTags",
                column: "JobId1");

            migrationBuilder.CreateIndex(
                name: "IX_JobTags_TagId1",
                table: "JobTags",
                column: "TagId1");

            migrationBuilder.AddForeignKey(
                name: "FK_JobTags_Jobs_JobId1",
                table: "JobTags",
                column: "JobId1",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobTags_Tags_TagId1",
                table: "JobTags",
                column: "TagId1",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
