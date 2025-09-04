import { ExpenseTrackerApiRoutes } from "../constants/expense-tracker-api-routes";
import type { ApiResponse } from "../dtos/api-response";
import type { CreateBudgetRequest } from "../dtos/budgets/create-budget-request";
import type { CreateBudgetResponse } from "../dtos/budgets/create-budget-response";
import type { BudgetStatusRequest } from "../dtos/budgets/budget-status-request";
import type { BudgetSummaryResponse } from "../dtos/budgets/budget-summary-response";
import type { BudgetOverviewSummaryResponse } from "../dtos/budgets/budget-overview-summary-response";
import type { UpdateBudgetRequest } from "../dtos/budgets/update-budget-request";
import { expenseTrackerApiClient } from "./expense-tracker-api-client";
import { unwrapApiResponse } from "../utils/api-response";

const pathWithId = (template: string, id: string) => template.replace("{id}", id);

export const budgetService = {
	async create(payload: CreateBudgetRequest): Promise<ApiResponse<CreateBudgetResponse>> {
		const response = await expenseTrackerApiClient
			.post<ApiResponse<CreateBudgetResponse>>(ExpenseTrackerApiRoutes.budget.post.create, payload);

		return unwrapApiResponse(response);
	},

	async getStatus(params: BudgetStatusRequest): Promise<ApiResponse<BudgetSummaryResponse>> {
		const response = await expenseTrackerApiClient
			.get<ApiResponse<BudgetSummaryResponse>>(ExpenseTrackerApiRoutes.budget.get.status, { params });

		return unwrapApiResponse(response);
	},

	async getOverview(period: string): Promise<ApiResponse<BudgetOverviewSummaryResponse>> {
		const response = await expenseTrackerApiClient
			.get<ApiResponse<BudgetOverviewSummaryResponse>>(ExpenseTrackerApiRoutes.budget.get.overview, { params: { period } });

		return unwrapApiResponse(response);
	},

	async update(id: string, payload: UpdateBudgetRequest): Promise<ApiResponse<null>> {
		const url = pathWithId(ExpenseTrackerApiRoutes.budget.put.update, id);
		const response = await expenseTrackerApiClient.put<ApiResponse<null>>(url, payload);

		return unwrapApiResponse(response);
	},

	async delete(id: string): Promise<ApiResponse<null>> {
		const url = pathWithId(ExpenseTrackerApiRoutes.budget.delete.remove, id);
		const response = await expenseTrackerApiClient.delete<ApiResponse<null>>(url);

		return unwrapApiResponse(response);
	},
};
