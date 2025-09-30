export interface CreateExpenseRequest {
  category: string;
  amount: number;
  dateOfExpense: string;
  paymentMethod: string;
  description?: string | null;
}
