using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseTracker.Migrations
{
    /// <inheritdoc />
    public partial class RenameLimitAmountToLimit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LimitAmount",
                table: "Budgets",
                newName: "Limit");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Limit",
                table: "Budgets",
                newName: "LimitAmount");
        }
    }
}
