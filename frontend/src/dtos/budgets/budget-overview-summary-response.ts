import type { CategoryBudgetOverviewDto } from "./category-budget-overview-dto";

export interface BudgetOverviewSummaryResponse {
  period: string;
  categories: CategoryBudgetOverviewDto[];
}
