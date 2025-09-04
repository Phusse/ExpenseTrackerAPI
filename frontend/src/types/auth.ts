import type { ApiResponse } from "../dtos/api-response";
import type { AuthLoginRequest } from "../dtos/auth/auth-login-request";
import type { AuthLoginResponse } from "../dtos/auth/auth-login-response";
import type { AuthRegisterRequest } from "../dtos/auth/auth-register-request";
import type { UserProfileResponse } from "../dtos/auth/user-profile-response";

export interface AuthContextValue {
	user: UserProfileResponse | null;
	isAuthenticated: boolean;
	login: (payload: AuthLoginRequest) => Promise<ApiResponse<AuthLoginResponse>>;
	logout: () => Promise<void>;
	register: (payload: AuthRegisterRequest) => Promise<ApiResponse<null>>;
	refreshCurrentUser: () => Promise<void>;
	getToken: () => string | null;
}
