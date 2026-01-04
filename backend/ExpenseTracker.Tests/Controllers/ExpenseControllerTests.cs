using System.Security.Claims;
using ExpenseTracker.Controllers;
using ExpenseTracker.Enums;
using ExpenseTracker.Models;
using ExpenseTracker.Models.DTOs.Expenses;
using ExpenseTracker.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ExpenseTracker.Tests.Controllers;

public class ExpenseControllerTests
{
    private readonly Mock<IExpenseService> _expenseServiceMock;
    private readonly ExpenseController _expenseController;
    private readonly ClaimsPrincipal _user;

    public ExpenseControllerTests()
    {
        _expenseServiceMock = new Mock<IExpenseService>();
        _expenseController = new ExpenseController(_expenseServiceMock.Object);

        var userId = Guid.NewGuid();
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Name, "Test User"),
            new(ClaimTypes.Email, "test@example.com")
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        _user = new ClaimsPrincipal(identity);

        _expenseController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = _user }
        };
    }

    [Fact]
    public async Task CreateExpense_ShouldReturnCreated_WhenSuccessful()
    {
        // Arrange
        var request = new CreateExpenseRequest
        {
            Category = ExpenseCategory.Food,
            Amount = 50,
            DateOfExpense = DateTime.UtcNow,
            PaymentMethod = PaymentMethod.Card, // Changed to Card
            Description = "Lunch"
        };
        var expectedResponse = new CreateExpenseResponse
        {
            Id = Guid.NewGuid(),
            Category = request.Category,
            Amount = request.Amount,
            DateOfExpense = request.DateOfExpense,
            PaymentMethod = request.PaymentMethod,
            Description = request.Description
        };

        _expenseServiceMock.Setup(x => x.CreateExpenseAsync(It.IsAny<Guid>(), request))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _expenseController.CreateExpense(request);

        // Assert
        var createdResult = result as CreatedAtActionResult;
        createdResult.Should().NotBeNull();
        createdResult!.StatusCode.Should().Be(201);

        var response = createdResult.Value as ApiResponse<CreateExpenseResponse>;
        response.Should().NotBeNull();
        response!.Success.Should().BeTrue();
        response.Data.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task GetAllExpenses_ShouldReturnOk_WhenSuccessful()
    {
        // Arrange
        var expenses = new List<CreateExpenseResponse>
        {
            new() { Id = Guid.NewGuid(), Amount = 100 },
            new() { Id = Guid.NewGuid(), Amount = 200 }
        };

        _expenseServiceMock.Setup(x => x.GetAllExpensesAsync(It.IsAny<Guid>()))
            .ReturnsAsync(expenses);

        // Act
        var result = await _expenseController.GetAllExpensesAsync();

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var response = okResult.Value as ApiResponse<IEnumerable<CreateExpenseResponse>>;
        response.Should().NotBeNull();
        response!.Success.Should().BeTrue();
        response.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetExpenseById_ShouldReturnOk_WhenFound()
    {
        // Arrange
        var expenseId = Guid.NewGuid();
        var expense = new CreateExpenseResponse { Id = expenseId, Amount = 100 };

        _expenseServiceMock.Setup(x => x.GetExpenseByIdAsync(expenseId, It.IsAny<Guid>()))
            .ReturnsAsync(expense);

        // Act
        var result = await _expenseController.GetExpenseByIdAsync(expenseId);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var response = okResult.Value as ApiResponse<CreateExpenseResponse>;
        response.Should().NotBeNull();
        response!.Success.Should().BeTrue();
        response.Data.Should().BeEquivalentTo(expense);
    }

    [Fact]
    public async Task GetExpenseById_ShouldReturnNotFound_WhenNotFound()
    {
        // Arrange
        var expenseId = Guid.NewGuid();

        _expenseServiceMock.Setup(x => x.GetExpenseByIdAsync(expenseId, It.IsAny<Guid>()))
            .ReturnsAsync((CreateExpenseResponse?)null);

        // Act
        var result = await _expenseController.GetExpenseByIdAsync(expenseId);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task DeleteExpense_ShouldReturnOk_WhenDeleted()
    {
        // Arrange
        var expenseId = Guid.NewGuid();

        _expenseServiceMock.Setup(x => x.DeleteExpenseAsync(expenseId, It.IsAny<Guid>()))
            .ReturnsAsync(true);

        // Act
        var result = await _expenseController.DeleteExpenseAsync(expenseId);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task DeleteExpense_ShouldReturnNotFound_WhenNotDeleted()
    {
        // Arrange
        var expenseId = Guid.NewGuid();

        _expenseServiceMock.Setup(x => x.DeleteExpenseAsync(expenseId, It.IsAny<Guid>()))
            .ReturnsAsync(false);

        // Act
        var result = await _expenseController.DeleteExpenseAsync(expenseId);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }
}
