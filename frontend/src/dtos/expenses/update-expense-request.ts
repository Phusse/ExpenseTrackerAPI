export interface UpdateExpenseRequest {
  id: string;
  category?: string | null;
  amount?: number | null;
  dateOfExpense?: string | null;
  paymentMethod?: string | null;
  description?: string | null;
}
