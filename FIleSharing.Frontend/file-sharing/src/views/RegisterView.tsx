import React from 'react';
import { RegisterForm } from '../components/RegisterForm';

export const RegisterView: React.FC = () => {
    return (
        <div>
            <h1>Регистрация</h1>
            <RegisterForm />
        </div>    
    );
};