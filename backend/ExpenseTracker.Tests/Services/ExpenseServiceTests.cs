using ExpenseTracker.Data;
using ExpenseTracker.Models;
using ExpenseTracker.Models.DTOs.Expenses;
using ExpenseTracker.Services;
using ExpenseTracker.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ExpenseTracker.Tests.Services;

public class ExpenseServiceTests
{
    private readonly ExpenseTrackerDbContext _dbContext;
    private readonly ExpenseService _expenseService;

    public ExpenseServiceTests()
    {
        var options = new DbContextOptionsBuilder<ExpenseTrackerDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _dbContext = new ExpenseTrackerDbContext(options);
        _expenseService = new ExpenseService(_dbContext);
    }

    [Fact]
    public async Task CreateExpenseAsync_ShouldCreateExpense_WhenValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new CreateExpenseRequest
        {
            Category = ExpenseCategory.Utilities,
            Amount = 100.50,
            DateOfExpense = DateTime.UtcNow.Date,
            PaymentMethod = PaymentMethod.Card,
            Description = "Electric Bill"
        };

        // Act
        var result = await _expenseService.CreateExpenseAsync(userId, request);

        // Assert
        result.Should().NotBeNull();
        result.Amount.Should().Be(request.Amount);
        result.Category.Should().Be(request.Category);
        result.Description.Should().Be(request.Description);

        var dbExpense = await _dbContext.Expenses.FindAsync(result.Id);
        dbExpense.Should().NotBeNull();
        dbExpense!.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task GetExpenseByIdAsync_ShouldReturnExpense_WhenFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expense = new Expense
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Category = ExpenseCategory.Food,
            Amount = 50,
            DateOfExpense = DateTime.UtcNow,
            DateRecorded = DateTime.UtcNow,
            PaymentMethod = PaymentMethod.Cash,
            Description = "Lunch"
        };
        _dbContext.Expenses.Add(expense);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _expenseService.GetExpenseByIdAsync(expense.Id, userId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(expense.Id);
    }

    [Fact]
    public async Task GetExpenseByIdAsync_ShouldReturnNull_WhenNotFoundOrWrongUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expense = new Expense
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(), // Different user
            Category = ExpenseCategory.Food,
            Amount = 50,
            DateOfExpense = DateTime.UtcNow,
            DateRecorded = DateTime.UtcNow,
            PaymentMethod = PaymentMethod.Cash,
            Description = "Lunch"
        };
        _dbContext.Expenses.Add(expense);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _expenseService.GetExpenseByIdAsync(expense.Id, userId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllExpensesAsync_ShouldReturnAllUserExpenses()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _dbContext.Expenses.AddRange(
            new Expense { Id = Guid.NewGuid(), UserId = userId, Category = ExpenseCategory.Food, Amount = 10, DateOfExpense = DateTime.UtcNow, DateRecorded = DateTime.UtcNow, PaymentMethod = PaymentMethod.Cash },
            new Expense { Id = Guid.NewGuid(), UserId = userId, Category = ExpenseCategory.Transport, Amount = 20, DateOfExpense = DateTime.UtcNow, DateRecorded = DateTime.UtcNow, PaymentMethod = PaymentMethod.Card },
            new Expense { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Category = ExpenseCategory.Food, Amount = 30, DateOfExpense = DateTime.UtcNow, DateRecorded = DateTime.UtcNow, PaymentMethod = PaymentMethod.Cash } // Other user
        );
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _expenseService.GetAllExpensesAsync(userId);

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateExpenseAsync_ShouldUpdate_WhenExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expense = new Expense
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Category = ExpenseCategory.Food,
            Amount = 50,
            DateOfExpense = DateTime.UtcNow,
            DateRecorded = DateTime.UtcNow,
            PaymentMethod = PaymentMethod.Cash,
            Description = "Lunch"
        };
        _dbContext.Expenses.Add(expense);
        await _dbContext.SaveChangesAsync();

        var request = new UpdateExpenseRequest
        {
            Id = expense.Id,
            Amount = 60,
            Description = "Dinner"
        };

        // Act
        var result = await _expenseService.UpdateExpenseAsync(userId, request);

        // Assert
        result.Should().BeTrue();
        var updatedExpense = await _dbContext.Expenses.FindAsync(expense.Id);
        updatedExpense!.Amount.Should().Be(60);
        updatedExpense.Description.Should().Be("Dinner");
        updatedExpense.Category.Should().Be(ExpenseCategory.Food); // Unchanged
    }

    [Fact]
    public async Task DeleteExpenseAsync_ShouldDelete_WhenExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expense = new Expense
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Category = ExpenseCategory.Food,
            Amount = 50,
            DateOfExpense = DateTime.UtcNow,
            DateRecorded = DateTime.UtcNow,
            PaymentMethod = PaymentMethod.Cash,
            Description = "Lunch"
        };
        _dbContext.Expenses.Add(expense);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _expenseService.DeleteExpenseAsync(expense.Id, userId);

        // Assert
        result.Should().BeTrue();
        var deletedExpense = await _dbContext.Expenses.FindAsync(expense.Id);
        deletedExpense.Should().BeNull();
    }
}
