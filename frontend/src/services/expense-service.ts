import { ExpenseTrackerApiRoutes } from "../constants/expense-tracker-api-routes";
import type { ApiResponse } from "../dtos/api-response";
import type { CreateExpenseRequest } from "../dtos/expenses/create-expense-request";
import type { CreateExpenseResponse } from "../dtos/expenses/create-expense-response";
import type { FilteredExpenseRequest } from "../dtos/expenses/filtered-expense-request";
import type { TotalExpenseRequest } from "../dtos/expenses/total-expense-request";
import type { UpdateExpenseRequest } from "../dtos/expenses/update-expense-request";
import { expenseTrackerApiClient } from "./expense-tracker-api-client";
import { unwrapApiResponse } from "../utils/api-response";

const pathWithId = (template: string, id: string) => template.replace("{id}", id);

export const expenseService = {
	async create(payload: CreateExpenseRequest): Promise<ApiResponse<CreateExpenseResponse>> {
		const response = await expenseTrackerApiClient
			.post<ApiResponse<CreateExpenseResponse>>(ExpenseTrackerApiRoutes.expense.post.create, payload);

		return unwrapApiResponse(response);
	},

	async getById(id: string): Promise<ApiResponse<CreateExpenseResponse | null>> {
		const url = pathWithId(ExpenseTrackerApiRoutes.expense.get.byId, id);
		const response = await expenseTrackerApiClient
			.get<ApiResponse<CreateExpenseResponse | null>>(url);

		return unwrapApiResponse(response);
	},

	async getAll(): Promise<ApiResponse<CreateExpenseResponse[]>> {
		const response = await expenseTrackerApiClient
			.get<ApiResponse<CreateExpenseResponse[]>>(ExpenseTrackerApiRoutes.expense.get.all);

		return unwrapApiResponse(response);
	},

	async filter(params: FilteredExpenseRequest): Promise<ApiResponse<CreateExpenseResponse[]>> {
		const response = await expenseTrackerApiClient
			.get<ApiResponse<CreateExpenseResponse[]>>(ExpenseTrackerApiRoutes.expense.get.filter, { params });

		return unwrapApiResponse(response);
	},

	async total(params: TotalExpenseRequest): Promise<ApiResponse<number>> {
		const response = await expenseTrackerApiClient
			.get<ApiResponse<number>>(ExpenseTrackerApiRoutes.expense.get.total, { params });

		return unwrapApiResponse(response);
	},

	async update(payload: UpdateExpenseRequest): Promise<ApiResponse<null>> {
		const url = pathWithId(ExpenseTrackerApiRoutes.expense.put.update, payload.id);
		const response = await expenseTrackerApiClient
			.put<ApiResponse<null>>(url, payload);

		return unwrapApiResponse(response);
	},

	async deleteById(id: string): Promise<ApiResponse<null>> {
		const url = pathWithId(ExpenseTrackerApiRoutes.expense.delete.byId, id);
		const response = await expenseTrackerApiClient
			.delete<ApiResponse<null>>(url);

		return unwrapApiResponse(response);
	},

	async deleteAll(): Promise<ApiResponse<null>> {
		const response = await expenseTrackerApiClient
			.delete<ApiResponse<null>>(ExpenseTrackerApiRoutes.expense.delete.all);

		return unwrapApiResponse(response);
	},
};
