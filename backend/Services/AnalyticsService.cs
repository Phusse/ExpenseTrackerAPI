using ExpenseTracker.Data;
using ExpenseTracker.Enums;
using ExpenseTracker.Models;
using ExpenseTracker.Models.DTOs.Analytics;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Services;

/// <summary>
/// Service for advanced analytics, predictions, and financial health scoring
/// </summary>
public class AnalyticsService(ExpenseTrackerDbContext dbContext)
{
    private readonly ExpenseTrackerDbContext _dbContext = dbContext;

    /// <summary>
    /// Calculate comprehensive financial health score (0-100)
    /// </summary>
    public async Task<FinancialHealthScoreDto> GetFinancialHealthScoreAsync(Guid userId)
    {
        var currentTime = DateTime.UtcNow;
        var currentMonth = currentTime.Month;
        var currentYear = currentTime.Year;

        // Get all necessary data
        var monthlyExpenses = await GetMonthlyExpenses(userId, currentMonth, currentYear);
        var monthlySavings = await GetMonthlySavings(userId, currentMonth, currentYear);
        var budgets = await _dbContext.Budgets
            .Where(b => b.UserId == userId && b.Period.Month == currentMonth && b.Period.Year == currentYear)
            .ToListAsync();
        var goals = await _dbContext.SavingGoals
            .Where(g => g.UserId == userId && !g.IsArchived)
            .ToListAsync();

        // Calculate score components
        int savingsScore = CalculateSavingsScore(monthlyExpenses, monthlySavings);
        int budgetScore = await CalculateBudgetScore(userId, budgets, currentMonth, currentYear);
        int goalScore = CalculateGoalScore(goals);
        int trendScore = await CalculateTrendScore(userId);
        int emergencyScore = CalculateEmergencyScore(goals, monthlyExpenses);

        int totalScore = savingsScore + budgetScore + goalScore + trendScore + emergencyScore;

        var recommendations = GenerateHealthRecommendations(savingsScore, budgetScore, goalScore, trendScore, emergencyScore);

        return new FinancialHealthScoreDto
        {
            TotalScore = totalScore,
            SavingsScore = savingsScore,
            BudgetScore = budgetScore,
            GoalScore = goalScore,
            TrendScore = trendScore,
            EmergencyScore = emergencyScore,
            Rating = GetHealthRating(totalScore),
            Trend = "stable",
            Recommendations = recommendations
        };
    }

    /// <summary>
    /// Analyze spending patterns
    /// </summary>
    public async Task<SpendingPatternsDto> GetSpendingPatternsAsync(Guid userId)
    {
        var expenses = await _dbContext.Expenses
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.DateOfExpense)
            .Take(90) // Last 90 days
            .ToListAsync();

        // Day of week analysis
        var dayOfWeekSpending = expenses
            .GroupBy(e => e.DateOfExpense.DayOfWeek.ToString())
            .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount));

        // Category trends
        var categoryTrends = await GetCategoryTrendsAsync(userId);

        // Recurring expenses detection
        var recurringExpenses = DetectRecurringExpenses(expenses);

        // Anomaly detection
        var anomalies = DetectAnomalies(expenses);

        return new SpendingPatternsDto
        {
            SpendingByDayOfWeek = dayOfWeekSpending,
            CategoryTrends = categoryTrends,
            RecurringExpenses = recurringExpenses,
            Anomalies = anomalies
        };
    }

    /// <summary>
    /// Get category trends with month-over-month comparison
    /// </summary>
    public async Task<List<CategoryTrendDto>> GetCategoryTrendsAsync(Guid userId)
    {
        var currentTime = DateTime.UtcNow;
        var currentMonth = currentTime.Month;
        var currentYear = currentTime.Year;

        var lastMonth = currentTime.AddMonths(-1);
        var lastMonthMonth = lastMonth.Month;
        var lastMonthYear = lastMonth.Year;

        var currentMonthExpenses = await _dbContext.Expenses
            .Where(e => e.UserId == userId && e.DateOfExpense.Month == currentMonth && e.DateOfExpense.Year == currentYear)
            .GroupBy(e => e.Category)
            .Select(g => new { Category = g.Key, Total = g.Sum(e => e.Amount), Count = g.Count() })
            .ToListAsync();

        var lastMonthExpenses = (await _dbContext.Expenses
            .Where(e => e.UserId == userId && e.DateOfExpense.Month == lastMonthMonth && e.DateOfExpense.Year == lastMonthYear)
            .GroupBy(e => e.Category)
            .Select(g => new { Category = g.Key, Total = g.Sum(e => e.Amount) })
            .ToListAsync())
            .ToDictionary(x => x.Category, x => x.Total);

        return currentMonthExpenses.Select(current =>
        {
            var lastTotal = lastMonthExpenses.GetValueOrDefault(current.Category, 0);
            var change = lastTotal > 0 ? ((current.Total - lastTotal) / lastTotal) * 100 : 0;

            return new CategoryTrendDto
            {
                Category = current.Category,
                CurrentMonthTotal = current.Total,
                LastMonthTotal = lastTotal,
                ChangePercentage = change,
                Trend = change > 10 ? "increasing" : change < -10 ? "decreasing" : "stable",
                AverageTransactionSize = current.Count > 0 ? current.Total / current.Count : 0,
                TransactionCount = current.Count
            };
        }).OrderByDescending(t => t.CurrentMonthTotal).ToList();
    }

    /// <summary>
    /// Generate spending forecast for current month
    /// </summary>
    public async Task<SpendingForecastDto> GetSpendingForecastAsync(Guid userId)
    {
        var currentTime = DateTime.UtcNow;
        var currentMonth = currentTime.Month;
        var currentYear = currentTime.Year;

        var currentSpending = await GetMonthlyExpenses(userId, currentMonth, currentYear);
        var daysInMonth = DateTime.DaysInMonth(currentYear, currentMonth);
        var daysElapsed = currentTime.Day;
        var daysRemaining = daysInMonth - daysElapsed;

        var dailyAverage = daysElapsed > 0 ? currentSpending / daysElapsed : 0;
        var projectedAdditional = dailyAverage * daysRemaining;
        var projectedTotal = currentSpending + projectedAdditional;

        // Category forecasts
        var categoryForecasts = await GetCategoryForecastsAsync(userId, currentMonth, currentYear, daysElapsed, daysRemaining);

        return new SpendingForecastDto
        {
            ProjectedMonthEnd = projectedTotal,
            CurrentSpending = currentSpending,
            DaysElapsed = daysElapsed,
            DaysRemaining = daysRemaining,
            DailyAverage = dailyAverage,
            ProjectedAdditionalSpending = projectedAdditional,
            CategoryForecasts = categoryForecasts
        };
    }

    /// <summary>
    /// Generate predictive insights and smart recommendations
    /// </summary>
    public async Task<PredictiveInsightsDto> GetPredictiveInsightsAsync(Guid userId)
    {
        var forecast = await GetSpendingForecastAsync(userId);
        var budgets = await _dbContext.Budgets
            .Where(b => b.UserId == userId)
            .ToListAsync();
        var goals = await _dbContext.SavingGoals
            .Where(g => g.UserId == userId && !g.IsArchived)
            .ToListAsync();

        var budgetWarnings = GenerateBudgetWarnings(forecast, budgets);
        var goalPredictions = GenerateGoalPredictions(goals);
        var recommendations = GenerateSmartRecommendations(forecast, budgets, goals);
        var savingsOpportunities = await GenerateSavingsOpportunities(userId);

        return new PredictiveInsightsDto
        {
            BudgetWarnings = budgetWarnings,
            GoalPredictions = goalPredictions,
            Recommendations = recommendations,
            SavingsOpportunities = savingsOpportunities
        };
    }

    // Helper methods
    private async Task<double> GetMonthlyExpenses(Guid userId, int month, int year)
    {
        return await _dbContext.Expenses
            .Where(e => e.UserId == userId && e.DateOfExpense.Month == month && e.DateOfExpense.Year == year)
            .SumAsync(e => e.Amount);
    }

    private async Task<double> GetMonthlySavings(Guid userId, int month, int year)
    {
        return await _dbContext.Expenses
            .Where(e => e.UserId == userId && e.Category == ExpenseCategory.Savings && 
                   e.DateOfExpense.Month == month && e.DateOfExpense.Year == year)
            .SumAsync(e => e.Amount);
    }

    private int CalculateSavingsScore(double expenses, double savings)
    {
        if (expenses + savings == 0) return 0;
        var savingsRate = savings / (expenses + savings);
        return (int)Math.Min(30, savingsRate * 100); // Max 30 points
    }

    private async Task<int> CalculateBudgetScore(Guid userId, List<Budget> budgets, int month, int year)
    {
        if (budgets.Count == 0) return 10; // Default score

        int budgetsUnderLimit = 0;
        foreach (var budget in budgets)
        {
            var spent = await _dbContext.Expenses
                .Where(e => e.UserId == userId && e.Category == budget.Category && 
                       e.DateOfExpense.Month == month && e.DateOfExpense.Year == year)
                .SumAsync(e => e.Amount);

            if (spent <= budget.Limit) budgetsUnderLimit++;
        }

        var adherenceRate = (double)budgetsUnderLimit / budgets.Count;
        return (int)(adherenceRate * 25); // Max 25 points
    }

    private int CalculateGoalScore(List<SavingGoal> goals)
    {
        if (goals.Count == 0) return 0;

        int score = 0;
        if (goals.Count > 0) score += 10; // Has goals
        if (goals.Count >= 3) score += 5; // Multiple goals
        if (goals.Any(g => g.CurrentAmount > 0)) score += 5; // Active progress
        return Math.Min(20, score); // Max 20 points
    }

    private async Task<int> CalculateTrendScore(Guid userId)
    {
        var currentMonth = await GetMonthlyExpenses(userId, DateTime.UtcNow.Month, DateTime.UtcNow.Year);
        var lastMonth = await GetMonthlyExpenses(userId, DateTime.UtcNow.AddMonths(-1).Month, DateTime.UtcNow.AddMonths(-1).Year);

        if (lastMonth == 0) return 10;

        var change = (currentMonth - lastMonth) / lastMonth;
        if (change < -0.05) return 15; // Decreasing
        if (change < 0.05) return 10; // Stable
        return 5; // Increasing
    }

    private int CalculateEmergencyScore(List<SavingGoal> goals, double monthlyExpenses)
    {
        var emergencyGoal = goals.FirstOrDefault(g => g.Title.Contains("Emergency", StringComparison.OrdinalIgnoreCase));
        if (emergencyGoal == null) return 0;

        int score = 5; // Has emergency goal
        var threeMonthsExpenses = monthlyExpenses * 3;
        if (emergencyGoal.CurrentAmount >= threeMonthsExpenses) score += 5;
        return score;
    }

    private string GetHealthRating(int score)
    {
        if (score >= 70) return "Excellent";
        if (score >= 50) return "Good";
        if (score >= 30) return "Fair";
        return "Needs Improvement";
    }

    private List<string> GenerateHealthRecommendations(int savings, int budget, int goal, int trend, int emergency)
    {
        var recs = new List<string>();

        if (savings < 20) recs.Add("Increase your savings rate to improve financial health");
        if (budget < 15) recs.Add("Stay within budget limits to boost your score");
        if (goal < 10) recs.Add("Create saving goals to track your financial progress");
        if (trend < 10) recs.Add("Focus on reducing monthly spending");
        if (emergency == 0) recs.Add("Build an emergency fund (3-6 months expenses)");

        return recs;
    }

    private List<RecurringExpenseDto> DetectRecurringExpenses(List<Expense> expenses)
    {
        // Simple detection: expenses with same description appearing monthly
        return expenses
            .Where(e => !string.IsNullOrEmpty(e.Description))
            .GroupBy(e => e.Description.ToLower().Trim())
            .Where(g => g.Count() >= 2)
            .Select(g => new RecurringExpenseDto
            {
                Description = g.First().Description,
                Amount = g.Average(e => e.Amount),
                Frequency = "monthly",
                LastOccurrence = g.Max(e => e.DateOfExpense)
            })
            .Take(5)
            .ToList();
    }

    private List<AnomalyDto> DetectAnomalies(List<Expense> expenses)
    {
        var anomalies = new List<AnomalyDto>();

        var categoryGroups = expenses.GroupBy(e => e.Category);
        foreach (var group in categoryGroups)
        {
            var avg = group.Average(e => e.Amount);
            var stdDev = Math.Sqrt(group.Average(e => Math.Pow(e.Amount - avg, 2)));

            var outliers = group.Where(e => Math.Abs(e.Amount - avg) > stdDev * 2);
            foreach (var outlier in outliers.Take(3))
            {
                anomalies.Add(new AnomalyDto
                {
                    Date = outlier.DateOfExpense,
                    Category = outlier.Category.ToString(),
                    Amount = outlier.Amount,
                    AverageAmount = avg,
                    DeviationPercentage = ((outlier.Amount - avg) / avg) * 100,
                    Reason = "Significantly higher than average"
                });
            }
        }

        return anomalies.Take(5).ToList();
    }

    private async Task<List<CategoryForecastDto>> GetCategoryForecastsAsync(Guid userId, int month, int year, int daysElapsed, int daysRemaining)
    {
        var categoryExpenses = await _dbContext.Expenses
            .Where(e => e.UserId == userId && e.DateOfExpense.Month == month && e.DateOfExpense.Year == year)
            .GroupBy(e => e.Category)
            .Select(g => new { Category = g.Key, Current = g.Sum(e => e.Amount) })
            .ToListAsync();

        var budgets = (await _dbContext.Budgets
            .Where(b => b.UserId == userId && b.Period.Month == month && b.Period.Year == year)
            .ToListAsync())
            .ToDictionary(b => b.Category, b => b.Limit);

        var daysInMonth = DateTime.DaysInMonth(year, month);

        return categoryExpenses.Select(cat =>
        {
            var dailyAvg = daysElapsed > 0 ? cat.Current / daysElapsed : 0;
            var projected = cat.Current + (dailyAvg * daysRemaining);
            var budgetLimit = budgets.GetValueOrDefault(cat.Category, 0);

            return new CategoryForecastDto
            {
                Category = cat.Category.ToString(),
                Current = cat.Current,
                Projected = projected,
                BudgetLimit = budgetLimit,
                WillExceedBudget = budgetLimit > 0 && projected > budgetLimit,
                ExcessAmount = budgetLimit > 0 ? Math.Max(0, projected - budgetLimit) : 0
            };
        }).ToList();
    }

    private List<BudgetWarningDto> GenerateBudgetWarnings(SpendingForecastDto forecast, List<Budget> budgets)
    {
        return forecast.CategoryForecasts
            .Where(cf => cf.WillExceedBudget)
            .Select(cf => new BudgetWarningDto
            {
                Category = cf.Category,
                BudgetLimit = cf.BudgetLimit,
                CurrentSpending = cf.Current,
                ProjectedTotal = cf.Projected,
                ExcessAmount = cf.ExcessAmount,
                Severity = cf.ExcessAmount > cf.BudgetLimit * 0.2 ? "critical" : "warning",
                Message = $"Projected to exceed {cf.Category} budget by ₦{cf.ExcessAmount:N0}"
            })
            .ToList();
    }

    private List<GoalPredictionDto> GenerateGoalPredictions(List<SavingGoal> goals)
    {
        return goals.Select(goal =>
        {
            var monthsSinceCreation = (DateTime.UtcNow - goal.CreatedAt).TotalDays / 30;
            var monthlyContribution = monthsSinceCreation > 0 ? goal.CurrentAmount / monthsSinceCreation : 0;
            var remaining = goal.TargetAmount - goal.CurrentAmount;
            var monthsToComplete = monthlyContribution > 0 ? remaining / monthlyContribution : 0;

            var onTrack = goal.Deadline.HasValue &&
                         DateTime.UtcNow.AddMonths((int)monthsToComplete) <= goal.Deadline.Value;

            return new GoalPredictionDto
            {
                GoalTitle = goal.Title,
                TargetAmount = goal.TargetAmount,
                CurrentAmount = goal.CurrentAmount,
                TargetDate = goal.Deadline,
                ProjectedCompletionDate = monthlyContribution > 0 ? DateTime.UtcNow.AddMonths((int)monthsToComplete) : null,
                Status = onTrack ? "on-track" : "behind",
                MonthlyContributionNeeded = monthlyContribution,
                Message = onTrack ? $"On track to complete by {goal.Deadline:MMM yyyy}" : "Increase contributions to meet deadline"
            };
        }).ToList();
    }

    private List<RecommendationDto> GenerateSmartRecommendations(SpendingForecastDto forecast, List<Budget> budgets, List<SavingGoal> goals)
    {
        var recommendations = new List<RecommendationDto>();

        // Budget recommendations
        if (budgets.Count == 0)
        {
            foreach (var cf in forecast.CategoryForecasts.Take(3))
            {
                recommendations.Add(new RecommendationDto
                {
                    Type = "budget",
                    Category = cf.Category,
                    Message = $"Consider setting a budget of ₦{cf.Projected * 1.1:N0} for {cf.Category}",
                    SuggestedAmount = cf.Projected * 1.1,
                    Priority = "medium"
                });
            }
        }

        // Goal recommendations
        if (goals.Count == 0)
        {
            recommendations.Add(new RecommendationDto
            {
                Type = "goal",
                Message = "Create a saving goal to track your financial progress",
                Priority = "high"
            });
        }

        return recommendations;
    }

    private async Task<List<SavingsOpportunityDto>> GenerateSavingsOpportunities(Guid userId)
    {
        var trends = await GetCategoryTrendsAsync(userId);

        return trends
            .Where(t => t.ChangePercentage > 20) // Increasing by >20%
            .Select(t => new SavingsOpportunityDto
            {
                Category = t.Category.ToString(),
                CurrentSpending = t.CurrentMonthTotal,
                RecommendedReduction = t.CurrentMonthTotal * 0.15,
                PotentialMonthlySavings = t.CurrentMonthTotal * 0.15,
                Message = $"Reduce {t.Category} spending by 15% to save ₦{t.CurrentMonthTotal * 0.15:N0}/month"
            })
            .Take(3)
            .ToList();
    }
}
