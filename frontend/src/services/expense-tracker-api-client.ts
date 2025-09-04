import axios, { AxiosError, type AxiosInstance, type AxiosResponse } from "axios";
import { STORAGE_KEYS } from "../constants/storage-key";
import type { ApiResponse } from "../dtos/api-response";

const API_BASE_URL = "http://localhost:5068/";

export const expenseTrackerApiClient: AxiosInstance = axios.create({
	baseURL: API_BASE_URL,
	headers: {
		"Content-Type": "application/json",
	},
});

// Request interceptor: attach token if present
expenseTrackerApiClient.interceptors.request.use((config) => {
	const token = localStorage.getItem(STORAGE_KEYS.AUTH_TOKEN);

	if (token) {
		config.headers = config.headers || {};
		config.headers["Authorization"] = `Bearer ${token}`;
	}

	return config;
});

// Response interceptor: only normalize errors
expenseTrackerApiClient.interceptors.response.use(
	(response: AxiosResponse) => response,
	(error: AxiosError) => {
		// If API returned an ApiResponse in the body, resolve it
		if (error.response?.data) {
			return Promise.resolve({ data: error.response.data });
		}

		// Network or unexpected error fallback
		const fallback: ApiResponse<any> = {
			success: false,
			message: error.message,
			timestamp: new Date().toISOString(),
			errors: [error.message],
		};

		return Promise.resolve({ data: fallback });
	}
);

export const unwrapApiResponse = <T>(response: { data: any }): ApiResponse<T> => {
	const data = response.data;

	if (!data || typeof data.success !== "boolean" || typeof data.timestamp !== "string") {
		return {
			success: false,
			message: "Unexpected response.",
			timestamp: new Date().toISOString(),
			errors: ["This response is not in the expected format."],
		};
	}

	return data as ApiResponse<T>;
};
