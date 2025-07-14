import React, { useState } from 'react';

export const RegisterForm: React.FC = () => {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');

    return (
        <form>
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
            </label> <br /> <br />

            <button type="submit">Зарегистрироваться</button>
        </form>
    );
};