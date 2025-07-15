import React from 'react';
import { RegisterForm } from '../components/RegisterForm';

export const LoginView: React.FC = () => {
    return (
        <div>
            <h1>Вход в систему</h1>
            <RegisterForm />
        </div>    
    );
};