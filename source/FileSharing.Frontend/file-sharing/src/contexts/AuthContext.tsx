import React, { createContext, useEffect, useState } from "react";
import type { UserDto } from "../models/UserDto";
import { authService } from "../services/authService";

interface AuthContextType {
    isAuthenticated: boolean;
    login: (dto: UserDto) => Promise<void>;
    logout: () => void;
    token: string | null;
    loading: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

interface AuthProviderProps {
    children: React.ReactNode;
}

export const AuthProvider: React.FC<AuthProviderProps> = ({ children }) => {
    const [token, setToken] = useState<string | null>(localStorage.getItem('token'));
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const checkToken = async () => {
            if (token) {
                // TODO: Добавитть проверку валидности токена
            }
            setLoading(false);
        };

        checkToken();
    }, [token]);

    const login = async (dto: UserDto) => {
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

    return (
        <AuthContext.Provider
            value={{
                isAuthenticated,
                login,
                logout,
                token,
                loading
            }}>
                {children}
        </AuthContext.Provider>
    );
};

// eslint-disable-next-line react-refresh/only-export-components
export const useAuth = () => {
    const context = React.useContext(AuthContext);
    if (context === undefined) {
        throw new Error('useAuth must be used within an AuthProvider');
    }
    return context;
};