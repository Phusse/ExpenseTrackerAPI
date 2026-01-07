import api from './api';

export interface UserSettings {
    notificationsEnabled: boolean;
    darkModeEnabled: boolean;
    currency: string;
    locale: string;
    budgetReminderDay: number;
    weeklyReportEnabled: boolean;
}

export interface UpdateProfileRequest {
    name?: string;
    email?: string;
}

export interface ChangePasswordRequest {
    currentPassword: string;
    newPassword: string;
    confirmNewPassword: string;
}

export interface UpdateSettingsRequest {
    notificationsEnabled?: boolean;
    darkModeEnabled?: boolean;
    currency?: string;
    locale?: string;
    budgetReminderDay?: number;
    weeklyReportEnabled?: boolean;
}

export interface Currency {
    code: string;
    name: string;
}

interface ApiResponse<T> {
    success: boolean;
    message?: string;
    data?: T;
}

export const userService = {
    // Profile
    updateProfile: async (data: UpdateProfileRequest): Promise<ApiResponse<any>> => {
        const response = await api.put<ApiResponse<any>>('/user/profile', data);
        return response.data;
    },

    changePassword: async (data: ChangePasswordRequest): Promise<ApiResponse<any>> => {
        const response = await api.post<ApiResponse<any>>('/user/change-password', data);
        return response.data;
    },

    // Settings
    getSettings: async (): Promise<UserSettings> => {
        const response = await api.get<ApiResponse<UserSettings>>('/user/settings');
        return response.data.data!;
    },

    updateSettings: async (data: UpdateSettingsRequest): Promise<ApiResponse<UserSettings>> => {
        const response = await api.put<ApiResponse<UserSettings>>('/user/settings', data);
        return response.data;
    },

    // Currencies
    getCurrencies: async (): Promise<Currency[]> => {
        const response = await api.get<ApiResponse<Currency[]>>('/user/currencies');
        return response.data.data || [];
    },

    // Export data
    exportData: async (): Promise<Blob> => {
        const response = await api.get('/user/export', {
            responseType: 'blob'
        });
        return response.data;
    },

    // Delete account
    deleteAccount: async (password: string, securityQuestionId?: number, securityAnswer?: string): Promise<ApiResponse<any>> => {
        const response = await api.delete<ApiResponse<any>>('/user/account', {
            data: { password, securityQuestionId, securityAnswer }
        });
        return response.data;
    },

    // Exchange rates
    getExchangeRates: async (baseCurrency: string = 'NGN'): Promise<ExchangeRatesResponse> => {
        const response = await api.get<ApiResponse<ExchangeRatesResponse>>(`/user/exchange-rates?baseCurrency=${baseCurrency}`);
        return response.data.data!;
    }
};

export interface ExchangeRatesResponse {
    baseCurrency: string;
    rates: Record<string, number>;
}
