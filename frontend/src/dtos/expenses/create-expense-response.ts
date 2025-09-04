export interface CreateExpenseResponse {
  id: string;
  category: string;
  amount: number;
  dateRecorded: string;
  dateOfExpense: string;
  paymentMethod: string;
  description?: string | null;
}
