import { useState } from 'react';
import { authService } from '../services/authService';
import type { UserDto } from '../models/UserDto';

interface UseRegisterResult {
    register: (dto: UserDto) => Promise<void>
    loading: boolean;
    error: string | null;
}

export function useRegister(): UseRegisterResult {
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const register = async (dto: UserDto): Promise<void> => {
        setLoading(true);
        setError(null);

        try {
            const result = await authService.register(dto);
            console.log('Регистрация успешна', result);
        } catch (err: unknown) {
            if (typeof err === 'object' && err !== null && 'message' in err) {
                setError((err as { message: string }).message || 'Ошибка регистрации'); 
            } else {
                setError('Ошибка регистрации');
             }
            throw err;

        } finally {
            setLoading(false);
        }
    };

    return {
        register,
        loading,
        error,
    };
}