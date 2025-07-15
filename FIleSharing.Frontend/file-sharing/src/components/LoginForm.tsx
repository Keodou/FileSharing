import React, { useState } from 'react';
import { useAuth } from '../hooks/useAuth';

interface LoginFormProps {
  onLoginSuccess?: () => void;
}

export const LoginForm: React.FC<LoginFormProps> = ({ onLoginSuccess }) => {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');

    const { login } = useAuth();

    const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    try {
        await login({ username, password });
        if (onLoginSuccess) {
            onLoginSuccess();
        }
    } catch (error) {
        console.error('Ошибка входа:', error);
    }
};

  return (
    <form onSubmit={handleSubmit}>
        <h2>Вход</h2>

        <label>
        Имя пользователя:
        <input
            type="text"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            required
        />
        </label>

        <label>
        Пароль:
        <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
        />
        </label>
      <button type="submit">Войти</button>
    </form>
  );
};