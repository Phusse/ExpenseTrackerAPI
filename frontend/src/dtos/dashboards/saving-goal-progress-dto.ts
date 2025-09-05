export interface SavingGoalProgressDto {
  title: string;
  targetAmount: number;
  currentAmount: number;
  progressPercent: number;
  deadline?: string | null;
}
