using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseTracker.Migrations
{
    /// <inheritdoc />
    public partial class SavingGoalColumnsUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Month",
                table: "SavingGoals");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "SavingGoals");

            migrationBuilder.AddColumn<DateTime>(
                name: "ArchivedAt",
                table: "SavingGoals",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CurrentAmount",
                table: "SavingGoals",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Deadline",
                table: "SavingGoals",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "SavingGoals",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "SavingGoals",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "SavingGoals",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "SavingGoals",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "SavingGoals",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SavingGoalContributions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SavingGoalId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ExpenseId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Amount = table.Column<double>(type: "double", nullable: false),
                    ContributedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavingGoalContributions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavingGoalContributions_Expenses_ExpenseId",
                        column: x => x.ExpenseId,
                        principalTable: "Expenses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SavingGoalContributions_SavingGoals_SavingGoalId",
                        column: x => x.SavingGoalId,
                        principalTable: "SavingGoals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_SavingGoalContributions_ExpenseId",
                table: "SavingGoalContributions",
                column: "ExpenseId");

            migrationBuilder.CreateIndex(
                name: "IX_SavingGoalContributions_SavingGoalId",
                table: "SavingGoalContributions",
                column: "SavingGoalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SavingGoalContributions");

            migrationBuilder.DropColumn(
                name: "ArchivedAt",
                table: "SavingGoals");

            migrationBuilder.DropColumn(
                name: "CurrentAmount",
                table: "SavingGoals");

            migrationBuilder.DropColumn(
                name: "Deadline",
                table: "SavingGoals");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "SavingGoals");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "SavingGoals");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "SavingGoals");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "SavingGoals");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "SavingGoals");

            migrationBuilder.AddColumn<int>(
                name: "Month",
                table: "SavingGoals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "SavingGoals",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
