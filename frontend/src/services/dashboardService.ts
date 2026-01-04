import api from './api';

interface ApiResponse<T> {
    success: boolean;
    message: string;
    data: T;
}

export interface DashboardSummary {
    totalExpenses: number;
    totalSavings: number;
    allTimeExpenses: number;
    allTimeSavings: number;
    recentTransactions: any[];
    categoryBreakdown: any[];
    budgets: any[];
    savingGoals: any[];
    dailyTrend: any[];
}

export const dashboardService = {
    getSummary: async (): Promise<DashboardSummary> => {
        const response = await api.get<ApiResponse<DashboardSummary>>('/dashboard/summary');
        return response.data.data;
    }
};
