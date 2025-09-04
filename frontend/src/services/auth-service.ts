import { ExpenseTrackerApiRoutes } from "../constants/expense-tracker-api-routes";
import { STORAGE_KEYS } from "../constants/storage-key";
import type { ApiResponse } from "../dtos/api-response";
import type { AuthLoginRequest } from "../dtos/auth/auth-login-request";
import type { AuthLoginResponse } from "../dtos/auth/auth-login-response";
import type { AuthRegisterRequest } from "../dtos/auth/auth-register-request";
import type { UserProfileResponse } from "../dtos/auth/user-profile-response";
import { expenseTrackerApiClient, unwrapApiResponse } from "./expense-tracker-api-client";

export const authService = {
	async register(payload: AuthRegisterRequest): Promise<ApiResponse<null>> {
		const response = await expenseTrackerApiClient
			.post<ApiResponse<null>>(ExpenseTrackerApiRoutes.auth.post.register, payload);

		return unwrapApiResponse(response);
	},

	async login(payload: AuthLoginRequest): Promise<ApiResponse<AuthLoginResponse>> {
		const response = await expenseTrackerApiClient
			.post<ApiResponse<AuthLoginResponse>>(ExpenseTrackerApiRoutes.auth.post.login, payload);

		const normalizedResponse = unwrapApiResponse<AuthLoginResponse>(response);

		if (normalizedResponse.data?.auth.token) {
			localStorage.setItem(STORAGE_KEYS.AUTH_TOKEN, normalizedResponse.data.auth.token);
		}

		return normalizedResponse;
	},

	async logoutServer(): Promise<ApiResponse<null>> {
		const response = await expenseTrackerApiClient
			.post<ApiResponse<null>>(ExpenseTrackerApiRoutes.auth.post.logout);

		this.logoutClient();
		return unwrapApiResponse<null>(response);
	},

	logoutClient(): void {
		localStorage.removeItem(STORAGE_KEYS.AUTH_TOKEN);
	},

	async getCurrentUser(): Promise<ApiResponse<UserProfileResponse>> {
		const response = await expenseTrackerApiClient
			.get<ApiResponse<UserProfileResponse>>(ExpenseTrackerApiRoutes.auth.get.currentUser);

		return unwrapApiResponse(response);
	},

	getToken(): string | null {
		return localStorage.getItem(STORAGE_KEYS.AUTH_TOKEN);
	},
};
