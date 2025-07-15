import React, { useState } from 'react';
import { useRegister } from '../hooks/userRegister';
import type { UserDto } from '../models/UserDto';

export const RegisterForm: React.FC = () => {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const { register, loading, error} = useRegister();

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        if (password !== confirmPassword) {
            alert('Пароли не совпадают');
        }

        const dto: UserDto = { username, password };

        try {
            await register(dto);
            alert('Вы успешно зарегистрированы');
        } catch (error) {
            alert(error);
        }
    };

    return (
        <form onSubmit={handleSubmit}>
            <h2>Регистрация</h2>
            <label>
                Имя пользователя:
                <br />
                <input 
                    type="text"
                    value={username}
                    onChange={(e) => setUsername(e.target.value)}
                    required
                    />
            </label>
            <br />

            <label>
                Пароль: <br />
                <input
                    type="password"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    required
                />
            </label> <br />

            <label>
                Подтвердите пароль: <br />
                <input
                    type="password"
                    value={confirmPassword}
                    onChange={(e) => setConfirmPassword(e.target.value)}
                    required
                />
            </label> <br />

            {error && <div style={{ color: 'red'}}>{error}</div>}

            <button type="submit" disabled={loading}> {loading ? 'Загрузка...' : 'Зарегистрироваться'}</button>
        </form>
    );
};