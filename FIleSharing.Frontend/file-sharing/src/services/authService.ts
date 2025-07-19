import type { UserDto } from "../models/UserDto";

const API_BASE_URL = "https://localhost:7046/api/Auth";

export const authService = {
    async register(userDto: UserDto): Promise<{ token: string }> {
        const response = await fetch(`${API_BASE_URL}/register`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                username: userDto.username,
                password: userDto.password,
            }),
        });

        if (!response.ok) {
            throw new Error('Регистрация не удалась');
        }

        return await response.json();
    },

    async login(userDto: UserDto): Promise<string | null> {
        const response = await fetch(`${API_BASE_URL}/login`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                username: userDto.username,
                password: userDto.password,
            }),
        });

        if (!response.ok) {
            throw new Error('Вход не удался');
        }

        try {
            const result = await response.json();
            if (typeof result !== 'object' || !result.token) {
                throw new Error('Неверный формат ответа');
            }
            return result.token;
        } catch (error) {
            console.error('Ошибка при парсинге JSON:', error);
            throw new Error('Ошибка входа');
        }
    }
}