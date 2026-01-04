import api from './api';
import type { Expense, CreateExpenseDto } from '../types';

interface ApiResponse<T> {
    success: boolean;
    message: string;
    data: T;
}

export const expenseService = {
    getAll: async (): Promise<Expense[]> => {
        const response = await api.get<ApiResponse<Expense[]>>('/expense/getall');
        return response.data.data || [];
    },

    create: async (data: CreateExpenseDto): Promise<Expense> => {
        const response = await api.post<ApiResponse<Expense>>('/expense', data);
        return response.data.data;
    },

    update: async (id: string, data: CreateExpenseDto): Promise<Expense> => {
        const response = await api.put<ApiResponse<Expense>>(`/expense/${id}`, data);
        return response.data.data;
    },

    delete: async (id: string): Promise<void> => {
        await api.delete(`/expense/${id}`);
    },

    getCategories: () => {
        // Must match backend ExpenseCategory enum order exactly
        return [
            { id: 0, name: 'Food' },
            { id: 1, name: 'Transport' },
            { id: 2, name: 'Health' },
            { id: 3, name: 'Entertainment' },
            { id: 4, name: 'Utilities' },
            { id: 5, name: 'Education' },
            { id: 6, name: 'Savings' },
            { id: 7, name: 'Investments' },
            { id: 8, name: 'Miscellaneous' },
        ];
    }
};
