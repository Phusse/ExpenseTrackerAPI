import { ExpenseTrackerApiRoutes } from "../constants/expense-tracker-api-routes";
import type { ApiResponse } from "../dtos/api-response";
import type { EnumOptionResponse } from "../dtos/metadata/enum-option-response";
import { expenseTrackerApiClient } from "./expense-tracker-api-client";
import { unwrapApiResponse } from "../utils/api-response";

export const metadataService =
{
	async getEnumOptions(url: string): Promise<ApiResponse<EnumOptionResponse[]>> {
		const response = await expenseTrackerApiClient.get<ApiResponse<EnumOptionResponse[]>>(url);
		return unwrapApiResponse<EnumOptionResponse[]>(response);
	},

	getExpenseCategories(): Promise<ApiResponse<EnumOptionResponse[]>> {
		return this.getEnumOptions(ExpenseTrackerApiRoutes.metadata.get.expenseCategories);
	},

	getPaymentMethods(): Promise<ApiResponse<EnumOptionResponse[]>> {
		return this.getEnumOptions(ExpenseTrackerApiRoutes.metadata.get.paymentMethods);
	},

	getSavingGoalStatuses(): Promise<ApiResponse<EnumOptionResponse[]>> {
		return this.getEnumOptions(ExpenseTrackerApiRoutes.metadata.get.savingGoalStatuses);
	}
}
