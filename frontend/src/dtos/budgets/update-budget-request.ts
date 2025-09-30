export interface UpdateBudgetRequest {
  category?: string | null;
  period?: string | null;
  newLimit?: number | null;
}
