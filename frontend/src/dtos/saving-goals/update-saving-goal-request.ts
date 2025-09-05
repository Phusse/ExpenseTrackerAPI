export interface UpdateSavingGoalRequest {
  title?: string | null;
  description?: string | null;
  targetAmount?: number | null;
  deadline?: string | null;
  status?: string | null;
  isArchived?: boolean | null;
}
