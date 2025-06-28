public interface IDashboardService
{
    Task<DashboardSummaryDto> GetDashboardSummaryAsync(Guid userId);
}
