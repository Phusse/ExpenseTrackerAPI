export interface RecentTransactionDto {
  id: string;
  category: string;
  amount: number;
  dateOfExpense?: string | null;
  description?: string | null;
}
