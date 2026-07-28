using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class CleanupTagsSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the JobTags foreign keys that reference Tags
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1 FROM sys.foreign_keys 
                    WHERE name = 'FK_JobTags_Tags_TagId'
                )
                BEGIN
                    ALTER TABLE [JobTags] DROP CONSTRAINT [FK_JobTags_Tags_TagId];
                END;

                IF EXISTS (
                    SELECT 1 FROM sys.foreign_keys 
                    WHERE name = 'FK_JobTags_Tags_TagId1'
                )
                BEGIN
                    ALTER TABLE [JobTags] DROP CONSTRAINT [FK_JobTags_Tags_TagId1];
                END;
            ");

            // Drop orphaned columns from JobTags
            migrationBuilder.Sql(@"
                IF COL_LENGTH('JobTags', 'TagId') IS NOT NULL
                BEGIN
                    ALTER TABLE [JobTags] DROP COLUMN [TagId];
                END;

                IF COL_LENGTH('JobTags', 'TagId1') IS NOT NULL
                BEGIN
                    ALTER TABLE [JobTags] DROP COLUMN [TagId1];
                END;
            ");

            // Recreate the Tags table with correct schema
            migrationBuilder.Sql(@"
                IF OBJECT_ID('Tags', 'U') IS NOT NULL
                BEGIN
                    DROP TABLE [Tags];
                END;
            ");

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            // Recreate JobTags table with correct schema
            migrationBuilder.Sql(@"
                IF OBJECT_ID('JobTags', 'U') IS NOT NULL
                BEGIN
                    DROP TABLE [JobTags];
                END;
            ");

            migrationBuilder.CreateTable(
                name: "JobTags",
                columns: table => new
                {
                    JobId = table.Column<int>(type: "int", nullable: false),
                    TagId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobTags", x => new { x.JobId, x.TagId });
                    table.ForeignKey(
                        name: "FK_JobTags_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobTags_TagId",
                table: "JobTags",
                column: "TagId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobTags");

            migrationBuilder.DropTable(
                name: "Tags");
        }
    }
}
