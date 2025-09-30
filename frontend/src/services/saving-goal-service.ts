import { ExpenseTrackerApiRoutes } from "../constants/expense-tracker-api-routes";
import type { ApiResponse } from "../dtos/api-response";
import type { AddSavingContributionRequest } from "../dtos/saving-goals/add-saving-contribution-request";
import type { CreateSavingGoalRequest } from "../dtos/saving-goals/create-saving-goal-request";
import type { CreateSavingGoalResponse } from "../dtos/saving-goals/create-saving-goal-response";
import type { UpdateSavingGoalRequest } from "../dtos/saving-goals/update-saving-goal-request";
import { expenseTrackerApiClient } from "./expense-tracker-api-client";
import { unwrapApiResponse } from "../utils/api-response";

const pathWithId = (template: string, id: string) => template.replace("{id}", id);

export const savingGoalService = {
	async create(payload: CreateSavingGoalRequest): Promise<ApiResponse<CreateSavingGoalResponse>> {
		const response = await expenseTrackerApiClient
			.post<ApiResponse<CreateSavingGoalResponse>>(ExpenseTrackerApiRoutes.savings.post.create, payload);

		return unwrapApiResponse(response);
	},

	async getAll(includeArchived = false): Promise<ApiResponse<CreateSavingGoalResponse[]>> {
		const response = await expenseTrackerApiClient
			.get<ApiResponse<CreateSavingGoalResponse[]>>(ExpenseTrackerApiRoutes.savings.get.all, { params: { includeArchived } });

		return unwrapApiResponse(response);
	},

	async getById(id: string): Promise<ApiResponse<CreateSavingGoalResponse>> {
		const url = pathWithId(ExpenseTrackerApiRoutes.savings.get.byId, id);
		const response = await expenseTrackerApiClient
			.get<ApiResponse<CreateSavingGoalResponse>>(url);

		return unwrapApiResponse(response);
	},

	async update(id: string, payload: UpdateSavingGoalRequest): Promise<ApiResponse<null>> {
		const url = pathWithId(ExpenseTrackerApiRoutes.savings.put.update, id);
		const response = await expenseTrackerApiClient
			.put<ApiResponse<null>>(url, payload);

		return unwrapApiResponse(response);
	},

	async archive(id: string, archiveGoal = true): Promise<ApiResponse<null>> {
		const url = pathWithId(ExpenseTrackerApiRoutes.savings.patch.archive, id);
		const response = await expenseTrackerApiClient
			.patch<ApiResponse<null>>(url, null, { params: { archiveGoal } });

		return unwrapApiResponse(response);
	},

	async deleteById(id: string): Promise<ApiResponse<null>> {
		const url = pathWithId(ExpenseTrackerApiRoutes.savings.delete.byId, id);
		const response = await expenseTrackerApiClient
			.delete<ApiResponse<null>>(url);

		return unwrapApiResponse(response);
	},

	async addSavingContribution(payload: AddSavingContributionRequest): Promise<ApiResponse<null>> {
		const response = await expenseTrackerApiClient
			.post<ApiResponse<null>>(ExpenseTrackerApiRoutes.savings.post.contribute, payload);

		return unwrapApiResponse(response);
	},
};
