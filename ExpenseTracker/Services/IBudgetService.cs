using ExpenseTracker.Models;
using ExpenseTracker.Enums;
using ExpenseTracker.Models.DTOs;

public interface IBudgetService
{
    Task<(bool IsSuccess, Budget? Data, string? ErrorMessage)> CreateBudgetAsync(CreateBudgetRequest request, Guid userId);
    Task<double> GetSpentAmountForCategoryAsync(Guid userId, ExpenseCategory category, int month, int year);
    Task<(double budgetedAmount, double spentAmount)> GetBudgetStatusAsync(Guid userId, ExpenseCategory category, int month, int year);
    Task<BudgetSummaryResponse> GetBudgetSummaryAsync(Guid userId, ExpenseCategory category, int month, int year);
}
