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

        const result = await response.json();
        return result.token || null;
    }
}