export interface User {
    id: string;
    username: string;
    email: string;
}

export interface AuthResponse {
    token: string;
    user: User;
}

export interface Expense {
    id: string;
    amount: number;
    category: string; // API returns string like 'Food', 'Transport'
    paymentMethod: string; // API returns string like 'Cash', 'Card'
    dateOfExpense: string; // ISO Date
    dateRecorded: string; // ISO Date
    description?: string;
}

export interface CreateExpenseDto {
    amount: number;
    category: number; // Enum value
    dateOfExpense: string;
    paymentMethod: number; // Enum value
    description?: string;
    savingGoalId?: string; // Optional - link to saving goal
}

export interface Budget {
    id: string;
    category: string;
    limit: number;
    spent: number;
    period: string;
}

export interface CreateBudgetDto {
    category: number;
    limit: number; // Backend expects 'limit' not 'amount'
    period: string; // Backend expects 'period' as DateOnly (e.g., "2026-01-01")
}

export interface SavingGoal {
    id: string;
    title: string; // Backend returns 'title' not 'name'
    description?: string;
    targetAmount: number;
    currentAmount: number;
    deadline?: string; // Backend returns 'deadline' not 'targetDate'
    status: string;
    createdAt: string;
}

export interface CreateSavingGoalDto {
    title: string; // Backend expects 'title' not 'name'
    description?: string;
    targetAmount: number;
    deadline?: string; // Backend expects 'deadline' not 'targetDate'
}
