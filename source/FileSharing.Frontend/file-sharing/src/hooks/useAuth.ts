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
    const [token, setToken] = useState<string | null>(localStorage.getItem('token'));

    const login = async (dto: UserDto): Promise<void> => {
        const result = await authService.login(dto);
        if (result) {
            localStorage.setItem('token', result);
            setToken(result);
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