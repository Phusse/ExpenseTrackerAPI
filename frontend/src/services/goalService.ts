import api from './api';
import type { SavingGoal, CreateSavingGoalDto } from '../types';

interface ApiResponse<T> {
    success: boolean;
    message: string;
    data: T;
}

export const goalService = {
    getAll: async (): Promise<SavingGoal[]> => {
        const response = await api.get<ApiResponse<SavingGoal[]>>('/savings/getall');
        return response.data.data || [];
    },

    create: async (data: CreateSavingGoalDto): Promise<SavingGoal> => {
        const response = await api.post<ApiResponse<SavingGoal>>('/savings', data);
        return response.data.data;
    },

    delete: async (id: string): Promise<void> => {
        await api.delete(`/savings/${id}`);
    }
};
