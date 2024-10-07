import axios from 'axios';
import { Server_HostAddress } from './constants/Server_HostAddress';

// Типы для хранения данных токенов
interface AuthTokens {
    accessToken: string;
    refreshToken: string;
}

// Сервис для работы с авторизацией
class AuthService {
    private static instance: AuthService;

    private constructor() { }

    public static getInstance(): AuthService {
        if (!AuthService.instance) {
            AuthService.instance = new AuthService();
        }
        return AuthService.instance;
    }

    // Функция для входа (login) и получения токенов
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

    // Устанавливаем токены в localStorage
    public setTokens(accessToken: string, refreshToken: string): void {
        localStorage.setItem('accessToken', accessToken);
        localStorage.setItem('refreshToken', refreshToken);
    }

    // Получение access токена из localStorage
    public getAccessToken(): string | null {
        return localStorage.getItem('accessToken');
    }

    // Получение refresh токена из localStorage
    public getRefreshToken(): string | null {
        return localStorage.getItem('refreshToken');
    }

    // Обновление токенов
    public async refreshTokens(): Promise<AuthTokens | null> {
        try {
            const refreshToken = this.getRefreshToken();

            if (!refreshToken) {
                throw new Error('No refresh token available');
            }

            const response = await axios.post(`${Server_HostAddress}/api/auth/refresh`, {
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

    // Проверка, не истек ли токен
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

    // Функция для автоматического обновления токенов
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

// Экспортируем singleton инстанс сервиса
export const authService = AuthService.getInstance();
