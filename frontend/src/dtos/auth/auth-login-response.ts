import type { AuthTokenDto } from "./auth-token-dto";
import type { AuthUserDto } from "./auth-user-dto";

export interface AuthLoginResponse {
	user: AuthUserDto;
	auth: AuthTokenDto;
}
