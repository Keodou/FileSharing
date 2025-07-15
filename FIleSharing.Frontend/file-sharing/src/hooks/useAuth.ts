import { useState } from 'react';
import type { UserDto } from "../models/UserDto";
import { authService } from "../services/authService";

interface UseAuthResult {
    login: (dto: UserDto) => Promise<void>;
    logout: () => void;
    isAuthenticated: boolean;
    token: string | null;
}

export function useAuth(): UseAuthResult {
    const [token, setToken] = useState<string | null>(null);

    const login = async (dto: UserDto): Promise<void> => {
        // eslint-disable-next-line no-useless-catch
        try {
            const token = await authService.login(dto);
            if (token) {
                localStorage.setItem('token', token);
                setToken(token);
            }
        } catch (err) {
            throw err;
        }
    };

    const logout = () => {
        localStorage.removeItem('token');
        setToken(null);
    };

    const isAuthenticated = !!token;

    return {
        login,
        logout,
        isAuthenticated,
        token,
    };
}