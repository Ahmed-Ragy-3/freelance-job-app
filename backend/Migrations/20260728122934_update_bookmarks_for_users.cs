using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class update_bookmarks_for_users : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Jobs_JobId",
                table: "Applications");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Jobs_JobId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookmarks_Freelancers_FreelancerId",
                table: "Bookmarks");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookmarks_Jobs_JobId",
                table: "Bookmarks");

            migrationBuilder.DropForeignKey(
                name: "FK_JobCategories_Categories_CategoryId",
                table: "JobCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_JobCategories_Jobs_JobId",
                table: "JobCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Clients_ClientUserId",
                table: "Jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_JobSkills_Jobs_JobId",
                table: "JobSkills");

            migrationBuilder.DropForeignKey(
                name: "FK_JobSkills_Skills_SkillId",
                table: "JobSkills");

            migrationBuilder.DropForeignKey(
                name: "FK_JobTags_Jobs_JobId",
                table: "JobTags");

            migrationBuilder.DropForeignKey(
                name: "FK_JobTags_Tags_TagId",
                table: "JobTags");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Jobs_JobId",
                table: "Reviews");

            migrationBuilder.RenameColumn(
                name: "FreelancerId",
                table: "Bookmarks",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Bookmarks_FreelancerId",
                table: "Bookmarks",
                newName: "IX_Bookmarks_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Jobs_JobId",
                table: "Applications",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Jobs_JobId",
                table: "Attachments",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookmarks_Jobs_JobId",
                table: "Bookmarks",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookmarks_Users_UserId",
                table: "Bookmarks",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobCategories_Categories_CategoryId",
                table: "JobCategories",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobCategories_Jobs_JobId",
                table: "JobCategories",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Clients_ClientUserId",
                table: "Jobs",
                column: "ClientUserId",
                principalTable: "Clients",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobSkills_Jobs_JobId",
                table: "JobSkills",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobSkills_Skills_SkillId",
                table: "JobSkills",
                column: "SkillId",
                principalTable: "Skills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobTags_Jobs_JobId",
                table: "JobTags",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobTags_Tags_TagId",
                table: "JobTags",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Jobs_JobId",
                table: "Reviews",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Jobs_JobId",
                table: "Applications");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Jobs_JobId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookmarks_Jobs_JobId",
                table: "Bookmarks");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookmarks_Users_UserId",
                table: "Bookmarks");

            migrationBuilder.DropForeignKey(
                name: "FK_JobCategories_Categories_CategoryId",
                table: "JobCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_JobCategories_Jobs_JobId",
                table: "JobCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Clients_ClientUserId",
                table: "Jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_JobSkills_Jobs_JobId",
                table: "JobSkills");

            migrationBuilder.DropForeignKey(
                name: "FK_JobSkills_Skills_SkillId",
                table: "JobSkills");

            migrationBuilder.DropForeignKey(
                name: "FK_JobTags_Jobs_JobId",
                table: "JobTags");

            migrationBuilder.DropForeignKey(
                name: "FK_JobTags_Tags_TagId",
                table: "JobTags");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Jobs_JobId",
                table: "Reviews");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Bookmarks",
                newName: "FreelancerId");

            migrationBuilder.RenameIndex(
                name: "IX_Bookmarks_UserId",
                table: "Bookmarks",
                newName: "IX_Bookmarks_FreelancerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Jobs_JobId",
                table: "Applications",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Jobs_JobId",
                table: "Attachments",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookmarks_Freelancers_FreelancerId",
                table: "Bookmarks",
                column: "FreelancerId",
                principalTable: "Freelancers",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookmarks_Jobs_JobId",
                table: "Bookmarks",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobCategories_Categories_CategoryId",
                table: "JobCategories",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobCategories_Jobs_JobId",
                table: "JobCategories",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Clients_ClientUserId",
                table: "Jobs",
                column: "ClientUserId",
                principalTable: "Clients",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobSkills_Jobs_JobId",
                table: "JobSkills",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobSkills_Skills_SkillId",
                table: "JobSkills",
                column: "SkillId",
                principalTable: "Skills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobTags_Jobs_JobId",
                table: "JobTags",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobTags_Tags_TagId",
                table: "JobTags",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Jobs_JobId",
                table: "Reviews",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
