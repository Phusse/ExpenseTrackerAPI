export interface FilteredExpenseRequest {
  startDate?: string | null;
  endDate?: string | null;
  minAmount?: number | null;
  maxAmount?: number | null;
  exactAmount?: number | null;
  category?: string | null;
}
