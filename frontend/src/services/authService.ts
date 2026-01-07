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

export interface SecurityQuestionItem {
    id: number;
    question: string;
}

export interface SecurityQuestionAnswer {
    questionId: number;
    answer: string;
}

export interface UserSecurityQuestion {
    questionOrder: number;
    questionId: number;
    question: string;
}

export interface ForgotPasswordQuestionsResponse {
    email: string;
    questions: UserSecurityQuestion[];
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

    // NEW: Register with security questions
    registerWithSecurityQuestions: async (
        name: string,
        email: string,
        password: string,
        securityQuestions: SecurityQuestionAnswer[]
    ) => {
        const response = await api.post<ApiResponse<null>>('/auth/register-with-security', {
            name,
            email,
            password,
            securityQuestions
        });
        return response.data;
    },

    // NEW: Get available security questions
    getSecurityQuestions: async (): Promise<SecurityQuestionItem[]> => {
        const response = await api.get<ApiResponse<{ questions: SecurityQuestionItem[] }>>('/auth/security-questions');
        return response.data.data?.questions || [];
    },

    // NEW: Get current user's security questions
    getMySecurityQuestions: async (): Promise<UserSecurityQuestion[]> => {
        const response = await api.get<ApiResponse<UserSecurityQuestion[]>>('/auth/me/security-questions');
        return response.data.data || [];
    },

    // NEW: Initiate forgot password flow
    forgotPassword: async (email: string): Promise<ApiResponse<ForgotPasswordQuestionsResponse>> => {
        const response = await api.post<ApiResponse<ForgotPasswordQuestionsResponse>>('/auth/forgot-password', { email });
        return response.data;
    },

    // NEW: Reset password with security answers
    resetPassword: async (
        email: string,
        answers: SecurityQuestionAnswer[],
        newPassword: string,
        confirmNewPassword: string
    ): Promise<ApiResponse<null>> => {
        const response = await api.post<ApiResponse<null>>('/auth/reset-password', {
            email,
            answers,
            newPassword,
            confirmNewPassword
        });
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
