import { ExpenseTrackerApiRoutes } from "../constants/expense-tracker-api-routes";
import type { ApiResponse } from "../dtos/api-response";
import type { DashboardSummaryResponse } from "../dtos/dashboards/dashboard-summary-response";
import { expenseTrackerApiClient } from "./expense-tracker-api-client";
import { unwrapApiResponse } from "../utils/api-response";

// export const dashboardService = {
// 	async getDashboardSummary(): Promise<ApiResponse<DashboardSummaryResponse>> {
// 		const response = await expenseTrackerApiClient
// 			.get<ApiResponse<DashboardSummaryResponse>>(ExpenseTrackerApiRoutes.dashboard.get.summary);

// 		return unwrapApiResponse(response);
// 	}
// }

export const dashboardService = {
	async getDashboardSummary(signal?: AbortSignal): Promise<ApiResponse<DashboardSummaryResponse>> {
		const response = await expenseTrackerApiClient.get<ApiResponse<DashboardSummaryResponse>>(
			ExpenseTrackerApiRoutes.dashboard.get.summary,
			{ signal }
		);

		return unwrapApiResponse(response);
	}
};