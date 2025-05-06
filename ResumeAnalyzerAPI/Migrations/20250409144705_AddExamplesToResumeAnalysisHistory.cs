using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResumeAnalyzerAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddExamplesToResumeAnalysisHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Examples",
                table: "ResumeAnalysisHistories",
                type: "TEXT",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Examples",
                table: "ResumeAnalysisHistories");
        }
    }
}
