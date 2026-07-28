using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class fix_mtm_relations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Skills_Jobs_JobId",
                table: "Skills");

            migrationBuilder.DropIndex(
                name: "IX_Skills_JobId",
                table: "Skills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Applications",
                table: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_Applications_JobId",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "JobId",
                table: "Skills");

            migrationBuilder.AddColumn<int>(
                name: "FreelancerUserId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Tags",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<int>(
                name: "JobId1",
                table: "JobTags",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "JobId1",
                table: "JobSkills",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FreelancerUserId",
                table: "FreelancerSkills",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Applications",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Applications",
                table: "Applications",
                columns: new[] { "JobId", "FreelancerId" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_FreelancerUserId",
                table: "Users",
                column: "FreelancerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_JobTags_JobId1",
                table: "JobTags",
                column: "JobId1");

            migrationBuilder.CreateIndex(
                name: "IX_JobSkills_JobId1",
                table: "JobSkills",
                column: "JobId1");

            migrationBuilder.CreateIndex(
                name: "IX_FreelancerSkills_FreelancerUserId",
                table: "FreelancerSkills",
                column: "FreelancerUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_FreelancerSkills_Freelancers_FreelancerUserId",
                table: "FreelancerSkills",
                column: "FreelancerUserId",
                principalTable: "Freelancers",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobSkills_Jobs_JobId1",
                table: "JobSkills",
                column: "JobId1",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobTags_Jobs_JobId1",
                table: "JobTags",
                column: "JobId1",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Freelancers_FreelancerUserId",
                table: "Users",
                column: "FreelancerUserId",
                principalTable: "Freelancers",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FreelancerSkills_Freelancers_FreelancerUserId",
                table: "FreelancerSkills");

            migrationBuilder.DropForeignKey(
                name: "FK_JobSkills_Jobs_JobId1",
                table: "JobSkills");

            migrationBuilder.DropForeignKey(
                name: "FK_JobTags_Jobs_JobId1",
                table: "JobTags");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Freelancers_FreelancerUserId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_FreelancerUserId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_JobTags_JobId1",
                table: "JobTags");

            migrationBuilder.DropIndex(
                name: "IX_JobSkills_JobId1",
                table: "JobSkills");

            migrationBuilder.DropIndex(
                name: "IX_FreelancerSkills_FreelancerUserId",
                table: "FreelancerSkills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Applications",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "FreelancerUserId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "JobId1",
                table: "JobTags");

            migrationBuilder.DropColumn(
                name: "JobId1",
                table: "JobSkills");

            migrationBuilder.DropColumn(
                name: "FreelancerUserId",
                table: "FreelancerSkills");

            migrationBuilder.AlterColumn<int>(
                name: "Name",
                table: "Tags",
                type: "int",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<int>(
                name: "JobId",
                table: "Skills",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Applications",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Applications",
                table: "Applications",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Skills_JobId",
                table: "Skills",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_JobId",
                table: "Applications",
                column: "JobId");

            migrationBuilder.AddForeignKey(
                name: "FK_Skills_Jobs_JobId",
                table: "Skills",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
