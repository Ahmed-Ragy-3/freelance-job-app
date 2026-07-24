using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTagJobId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1
                    FROM sys.foreign_keys fk
                    JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
                    JOIN sys.columns c ON fkc.parent_object_id = c.object_id AND fkc.parent_column_id = c.column_id
                    WHERE fk.name = 'FK_Tags_Jobs_JobId' AND OBJECT_NAME(fkc.parent_object_id) = 'Tags'
                )
                BEGIN
                    ALTER TABLE [Tags] DROP CONSTRAINT [FK_Tags_Jobs_JobId];
                END;

                IF COL_LENGTH('Tags', 'JobId') IS NOT NULL
                BEGIN
                    ALTER TABLE [Tags] DROP COLUMN [JobId];
                END;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF COL_LENGTH('Tags', 'JobId') IS NULL
                BEGIN
                    ALTER TABLE [Tags] ADD [JobId] int NULL;
                END;
            ");
        }
    }
}
