import React, { createContext, useEffect, useState, type ReactNode } from "react";
import { authService } from "../services/auth-service";
import type { ApiResponse } from "../dtos/api-response";
import type { UserProfileResponse } from "../dtos/auth/user-profile-response";
import type { AuthLoginResponse } from "../dtos/auth/auth-login-response";
import type { AuthContextValue } from "../types/auth";
import type { AuthLoginRequest } from "../dtos/auth/auth-login-request";
import type { AuthRegisterRequest } from "../dtos/auth/auth-register-request";

export const AuthContext = createContext<AuthContextValue | undefined>(undefined);

type AuthProviderProps = { children: ReactNode };

export const AuthProvider: React.FC<AuthProviderProps> = ({ children }) => {
	const [user, setUser] = useState<UserProfileResponse | null>(null);

	// Initialize user from server if a token exists
	useEffect(() => {
		const bootstrap = async () => {
			const token = authService.getToken();

			if (!token) {
				setUser(null);
				return;
			}

			const me = await authService.getCurrentUser();

			if (me.success) {
				setUser(me.data ?? null);
			} else {
				setUser(null);
			}
		};

		void bootstrap();
	}, []);

	const login = async (payload: AuthLoginRequest): Promise<ApiResponse<AuthLoginResponse>> => {
		const result = await authService.login(payload);

		if (result.success) {
			const me = await authService.getCurrentUser();

			if (me.success) {
				setUser(me.data ?? null);
			}
		}

		return result;
	};

	const logout = async (): Promise<void> => {
		await authService.logoutServer();
		setUser(null);
	};

	const register = async (payload: AuthRegisterRequest): Promise<ApiResponse<null>> => {
		const result = await authService.register(payload);
		return result;
	};

	const refreshCurrentUser = async (): Promise<void> => {
		const me = await authService.getCurrentUser();

		if (me.success) {
			setUser(me.data ?? null);
		}
	};

	const getToken = () => authService.getToken();

	const value: AuthContextValue = {
		user,
		isAuthenticated: !!user,
		login,
		logout,
		register,
		refreshCurrentUser,
		getToken,
	};

	return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};
