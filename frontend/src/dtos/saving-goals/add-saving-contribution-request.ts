export interface AddSavingContributionRequest {
  savingGoalId: string;
  amount: number;
  dateOfExpense?: string | null;
  description?: string | null;
  paymentMethod: string;
}
