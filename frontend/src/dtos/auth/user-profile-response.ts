export interface UserProfileResponse {
	id: string;
	name: string;
	email: string;
	createdAt: string;
	lastLoginAt?: string | null;
}
