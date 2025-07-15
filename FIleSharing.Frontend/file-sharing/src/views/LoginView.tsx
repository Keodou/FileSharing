import React from 'react';
import { LoginForm } from "../components/LoginForm"

export const LoginView: React.FC = () => {
    return (
        <div>
            <h1>Вход в систему</h1>
            <LoginForm />
        </div>    
    );
};