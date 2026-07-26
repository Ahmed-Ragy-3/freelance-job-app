using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class fix_db_for_seeder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FreelancerSkills_Freelancers_FreelancerUserId",
                table: "FreelancerSkills");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Users_ClientId",
                table: "Jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_JobSkills_Jobs_JobId1",
                table: "JobSkills");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Freelancers_FreelancerUserId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_FreelancerUserId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_JobSkills_JobId1",
                table: "JobSkills");

            migrationBuilder.DropIndex(
                name: "IX_FreelancerSkills_FreelancerUserId",
                table: "FreelancerSkills");

            migrationBuilder.DropColumn(
                name: "FreelancerUserId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "JobId1",
                table: "JobSkills");

            migrationBuilder.DropColumn(
                name: "FreelancerUserId",
                table: "FreelancerSkills");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Applications");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Skills",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "ClientUserId",
                table: "Jobs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "CoverLetter",
                table: "Applications",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Clients_ClientId",
                table: "Jobs",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Clients_ClientId",
                table: "Jobs");

            migrationBuilder.AddColumn<int>(
                name: "FreelancerUserId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Skills",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<int>(
                name: "JobId1",
                table: "JobSkills",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ClientUserId",
                table: "Jobs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FreelancerUserId",
                table: "FreelancerSkills",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CoverLetter",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Applications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Users_FreelancerUserId",
                table: "Users",
                column: "FreelancerUserId");

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
                name: "FK_Jobs_Users_ClientId",
                table: "Jobs",
                column: "ClientId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobSkills_Jobs_JobId1",
                table: "JobSkills",
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
    }
}
