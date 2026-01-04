import api from './api';

// Types
export interface FinancialHealthScore {
    totalScore: number;
    savingsScore: number;
    budgetScore: number;
    goalScore: number;
    trendScore: number;
    emergencyScore: number;
    rating: string;
    trend: string;
    recommendations: string[];
}

export interface SpendingPatterns {
    spendingByDayOfWeek: Record<string, number>;
    categoryTrends: CategoryTrend[];
    recurringExpenses: RecurringExpense[];
    anomalies: Anomaly[];
}

export interface CategoryTrend {
    category: string;
    currentMonthTotal: number;
    lastMonthTotal: number;
    changePercentage: number;
    trend: string;
    averageTransactionSize: number;
    transactionCount: number;
}

export interface RecurringExpense {
    description: string;
    amount: number;
    frequency: string;
    lastOccurrence: string;
    nextExpectedDate?: string;
}

export interface Anomaly {
    date: string;
    category: string;
    amount: number;
    averageAmount: number;
    deviationPercentage: number;
    reason: string;
}

export interface SpendingForecast {
    projectedMonthEnd: number;
    currentSpending: number;
    daysElapsed: number;
    daysRemaining: number;
    dailyAverage: number;
    projectedAdditionalSpending: number;
    categoryForecasts: CategoryForecast[];
}

export interface CategoryForecast {
    category: string;
    current: number;
    projected: number;
    budgetLimit: number;
    willExceedBudget: boolean;
    excessAmount: number;
}

export interface PredictiveInsights {
    budgetWarnings: BudgetWarning[];
    goalPredictions: GoalPrediction[];
    recommendations: Recommendation[];
    savingsOpportunities: SavingsOpportunity[];
}

export interface BudgetWarning {
    category: string;
    budgetLimit: number;
    currentSpending: number;
    projectedTotal: number;
    excessAmount: number;
    severity: string;
    message: string;
}

export interface GoalPrediction {
    goalTitle: string;
    targetAmount: number;
    currentAmount: number;
    targetDate?: string;
    projectedCompletionDate?: string;
    status: string;
    monthlyContributionNeeded: number;
    message: string;
}

export interface Recommendation {
    type: string;
    category: string;
    message: string;
    suggestedAmount?: number;
    priority: string;
}

export interface SavingsOpportunity {
    category: string;
    currentSpending: number;
    recommendedReduction: number;
    potentialMonthlySavings: number;
    message: string;
}

// Service
export const analyticsService = {
    getHealthScore: async (): Promise<FinancialHealthScore> => {
        const response = await api.get<FinancialHealthScore>('/analytics/health-score');
        return response.data;
    },

    getSpendingPatterns: async (): Promise<SpendingPatterns> => {
        const response = await api.get<SpendingPatterns>('/analytics/spending-patterns');
        return response.data;
    },

    getCategoryTrends: async (): Promise<CategoryTrend[]> => {
        const response = await api.get<CategoryTrend[]>('/analytics/category-trends');
        return response.data;
    },

    getForecast: async (): Promise<SpendingForecast> => {
        const response = await api.get<SpendingForecast>('/analytics/forecast');
        return response.data;
    },

    getPredictions: async (): Promise<PredictiveInsights> => {
        const response = await api.get<PredictiveInsights>('/analytics/predictions');
        return response.data;
    },
};
