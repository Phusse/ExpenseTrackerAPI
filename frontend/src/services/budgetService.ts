import api from './api';
import type { Budget, CreateBudgetDto } from '../types';

interface ApiResponse<T> {
    success: boolean;
    message: string;
    data: T;
}



export const budgetService = {
    getAll: async (): Promise<Budget[]> => {
        const response = await api.get<ApiResponse<Budget[]>>('/budget/getall');
        return response.data.data || [];
    },

    create: async (data: CreateBudgetDto): Promise<Budget> => {
        const response = await api.post<ApiResponse<Budget>>('/budget', data);
        return response.data.data;
    },

    delete: async (id: string): Promise<void> => {
        await api.delete(`/budget/delete/${id}`);
    }
};
