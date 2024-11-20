import axios from 'axios';
import { Server_HostAddress } from './constants/Server_HostAddress';

interface AuthTokens {
    accessToken: string;
    refreshToken: string;
}

class AuthService {
    private static instance: AuthService;

    private constructor() { }

    public static getInstance(): AuthService {
        if (!AuthService.instance) {
            AuthService.instance = new AuthService();
        }
        return AuthService.instance;
    }

    public async login(email: string, password: string): Promise<AuthTokens> {
        try {
            const response = await axios.post(`${Server_HostAddress}/api/auth/login`, {
                email,
                password,
            });

            const { token, refreshToken } = response.data;
            this.setTokens(token, refreshToken);
            return { accessToken: token, refreshToken };
        } catch (error) {
            console.error('Failed to login', error);
            throw error;
        }
    }

    public setTokens(accessToken: string, refreshToken: string): void {
        localStorage.setItem('accessToken', accessToken);
        localStorage.setItem('refreshToken', refreshToken);
    }

    public getAccessToken(): string | null {
        return localStorage.getItem('accessToken');
    }

    public getRefreshToken(): string | null {
        return localStorage.getItem('refreshToken');
    }

    public async refreshTokens(): Promise<AuthTokens | null> {
        try {
            const refreshToken = this.getRefreshToken();

            if (!refreshToken) {
                throw new Error('No refresh token available');
            }

            const response = await axios.post(`${Server_HostAddress}/api/auth/update_tokens`, {
                accessToken: this.getAccessToken(),
                refreshToken: refreshToken,
            });

            const { token: newAccessToken, refreshToken: newRefreshToken } = response.data;
            this.setTokens(newAccessToken, newRefreshToken);

            return { accessToken: newAccessToken, refreshToken: newRefreshToken };
        } catch (error) {
            console.error('Failed to refresh tokens', error);
            throw error;
        }
    }

    public isAccessTokenExpired(token: string): boolean {
        try {
            const tokenPayload = JSON.parse(atob(token.split('.')[1]));
            const exp = tokenPayload.exp;
            const now = Math.floor(Date.now() / 1000);
            return exp < now;
        } catch (error) {
            console.error('Failed to parse token', error);
            return true;
        }
    }

    public async getValidAccessToken(): Promise<string> {
        let accessToken = this.getAccessToken();

        if (!accessToken || this.isAccessTokenExpired(accessToken)) {
            const refreshedTokens = await this.refreshTokens();
            if (refreshedTokens) {
                accessToken = refreshedTokens.accessToken;
            } else {
                throw new Error('Failed to refresh tokens');
            }
        }

        return accessToken;
    }
}

axios.interceptors.request.use(async (config) => {
    const token = await authService.getValidAccessToken();
    if (token) {
        config.headers['Authorization'] = `Bearer ${token}`;
    }
    return config;
}, (error) => {
    return Promise.reject(error);
})

export const authService = AuthService.getInstance();
