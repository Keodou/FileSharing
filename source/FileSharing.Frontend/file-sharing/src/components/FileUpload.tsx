import React, { useState } from "react";
import { useAuth } from "../contexts/AuthContext"
import { uploadFile } from "../services/fileService";
import { fileValidationConfig, getFileSizeString } from "../config/fileValidationConfig";


export const FileUpload: React.FC = () => {
    const { token } = useAuth();
    const [selectedFile, setSelectedFile] = useState<File | null>(null);
    const [uploading, setUploading] = useState(false);
    const [message, setMessage] = useState<string | null>(null);
    const [error, setError] = useState<string | null>(null);

    const validateFile = (file: File): string | null => {
        if (file.size > fileValidationConfig.maxSize) {
            return `Файл слишком большой. Максимальный размер: ${getFileSizeString(fileValidationConfig.maxSize)}`;
        }

        if (!fileValidationConfig.allowedTypes.includes(file.type)) {
            return `Тип файла не поддерживается. Разрешенные типы: ${fileValidationConfig.allowedTypes.join(', ')}`;
        }

        return null;
    }

    const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        if (e.target.files && e.target.files.length > 0) {
            const file = e.target.files[0];

            // Валидация файла
            const validationError = validateFile(file);
            if (validationError) {
                setError(validationError);
                setSelectedFile(null);
                return;
            }

            setSelectedFile(file);
            setMessage(null);
            setError(null);
        }
    };

    const handleUpload = async () => {
        if (!selectedFile) {
            setError('Пожалуйста, выберите файл');
            return;
        }

        if (!token) {
            setError('Пользователь не авторизован');
            return;
        }

        setUploading(true);
        setError(null);
        setMessage(null);

        try {
            await uploadFile(selectedFile, token);
            setMessage('Файл успешно загружен!');
            setSelectedFile(null);

            const fileInput = document.querySelector('input[type="file"]') as HTMLInputElement;
            if (fileInput) fileInput.value = '';
        } catch (err) {
            console.error('Ошибка загрузки файла:', err);
            setError('Ошибка при загрузке файла');
        } finally {
            setUploading(false);
        }
    };

    return (
        <div style={{ marginBottom: '30px', padding: '20px', border: '1px solid #ddd', borderRadius: '8px' }}>
            <h3>Загрузка файла</h3>
      
            <div style={{ marginBottom: '15px' }}>
                <input
                    type="file"
                    onChange={handleFileChange}
                    disabled={uploading}
                    style={{ marginBottom: '10px' }}
                />
            <div style={{ fontSize: '12px', color: '#666' }}>
                Максимальный размер: {getFileSizeString(fileValidationConfig.maxSize)}
            </div>
            <div style={{ fontSize: '12px', color: '#666' }}>
                Разрешённые типы: JPG, PNG, GIF, PDF, TXT, DOC, DOCX, XLS, XLSX
            </div>
        </div>
      
        <button 
            onClick={handleUpload}
            disabled={uploading || !selectedFile}
            style={{ 
                padding: '10px 20px', 
                marginRight: '10px',
                backgroundColor: uploading ? '#ccc' : '#007bff',
                color: 'white',
                border: 'none',
                borderRadius: '4px',
                cursor: uploading ? 'not-allowed' : 'pointer'
            }}
        >
            {uploading ? 'Загрузка...' : 'Загрузить'}
        </button>

        {message && <div style={{ color: 'green', marginTop: '10px' }}>{message}</div>}
        {error && <div style={{ color: 'red', marginTop: '10px' }}>{error}</div>}
        </div>
    );
};