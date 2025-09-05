export interface CreateSavingGoalResponse {
  id: string;
  title: string;
  description?: string | null;
  currentAmount: number;
  targetAmount: number;
  deadline?: string | null;
  status: string;
  createdAt: string;
  updatedAt?: string | null;
  isArchived: boolean;
  archivedAt?: string | null;
}
