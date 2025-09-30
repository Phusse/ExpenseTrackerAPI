import type { ApiResponse } from "../dtos/api-response";

export const unwrapApiResponse = <T>(response: { data: any; }): ApiResponse<T> => {
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
