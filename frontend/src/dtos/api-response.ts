export interface ApiResponse<T> {
	success: boolean;
	message: string;
	errors?: string[] | null;
	timestamp: string;
	data?: T | null;
}
