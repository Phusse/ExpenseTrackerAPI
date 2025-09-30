export interface CreateSavingGoalRequest {
  title: string;
  description?: string | null;
  targetAmount: number;
  deadline?: string | null;
}
