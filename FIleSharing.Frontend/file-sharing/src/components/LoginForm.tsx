import React, { useState } from 'react';
import { useAuth } from '../hooks/useAuth';
import { useNavigate } from 'react-router-dom';

export const LoginForm: React.FC = () => {
    const navigate = useNavigate();
    const { login } = useAuth();

    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState<string | null>(null);

    const handleSubmit = async (e: React.FormEvent) => {
      e.preventDefault();
      setError(null);

      try {
          await login({ username, password });
          navigate('/files');
      // eslint-disable-next-line @typescript-eslint/no-unused-vars
      } catch (err) {
          setError('Неверное имя пользователя или пароль');
      }
    };

  return (
    <form onSubmit={handleSubmit}>
        <h2>Вход</h2>

        {error && <div style={{ color: 'red' }}>{error}</div>}

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