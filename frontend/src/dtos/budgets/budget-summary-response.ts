export interface BudgetSummaryResponse {
  id?: string | null;
  category: string;
  period: string;
  budgetedAmount: number;
  spentAmount: number;
  remainingAmount: number;
  percentageUsed: number;
  message: string;
}
