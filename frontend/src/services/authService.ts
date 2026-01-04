import api from './api';

interface ApiResponse<T> {
    success: boolean;
    message: string;
    data: T;
}

interface AuthUser {
    id: string;
    name: string;
    email: string;
}

interface AuthToken {
    token: string;
    expiresAt: string;
}

interface LoginData {
    user: AuthUser;
    auth: AuthToken;
}

export const authService = {
    login: async (email: string, password: string) => {
        const response = await api.post<ApiResponse<LoginData>>('/auth/login', { email, password });
        if (response.data.success && response.data.data) {
            const { user, auth } = response.data.data;
            localStorage.setItem('token', auth.token);
            localStorage.setItem('user', JSON.stringify(user));
        }
        return response.data;
    },

    register: async (name: string, email: string, password: string) => {
        const response = await api.post<ApiResponse<null>>('/auth/register', { name, email, password });
        return response.data;
    },

    logout: () => {
        localStorage.removeItem('token');
        localStorage.removeItem('user');
        window.location.href = '/login';
    },

    getCurrentUser: () => {
        const userStr = localStorage.getItem('user');
        if (userStr) return JSON.parse(userStr);
        return null;
    },

    isAuthenticated: () => {
        return !!localStorage.getItem('token');
    }
};
