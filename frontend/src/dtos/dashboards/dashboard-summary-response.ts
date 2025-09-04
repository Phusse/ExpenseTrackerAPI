import type { BudgetSummaryResponse } from "../budgets/budget-summary-response";
import type { CategorySpendingDto } from "./category-spending-dto";
import type { DailySpendingDto } from "./daily-spending-dto";
import type { RecentTransactionDto } from "./recent-transaction-dto";
import type { SavingGoalProgressDto } from "./saving-goal-progress-dto";

export interface DashboardSummaryResponse {
  totalExpenses: number;
  totalSavings: number;
  budgets: BudgetSummaryResponse[];
  categoryBreakdown: CategorySpendingDto[];
  dailyTrend: DailySpendingDto[];
  recentTransactions: RecentTransactionDto[];
  savingGoals: SavingGoalProgressDto[];
}
