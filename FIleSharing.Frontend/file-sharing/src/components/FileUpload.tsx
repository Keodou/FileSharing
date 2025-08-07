import React, { useState } from "react";
import { useAuth } from "../contexts/AuthContext"
import { uploadFile } from "../services/fileService";


export const FileUpload: React.FC = () => {
    const { token } = useAuth();
    const [selectedFile, setSelectedFile] = useState<File | null>(null);
    const [uploading, setUploading] = useState(false);
    const [message, setMessage] = useState<string | null>(null);
    const [error, setError] = useState<string | null>(null);

    const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        if (e.target.files && e.target.files.length > 0) {
            setSelectedFile(e.target.files[0]);
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
        } catch (err: any) {
            setError(err.message || 'Ошибка при загрузке файла');
        } finally {
            setUploading(false);
        }
    };

    return (
        <div>
            <h3>Загрузка файла</h3>

            <input
                type='file'
                onChange={handleFileChange}
                disabled={uploading}
            />

            <button 
                onClick={handleUpload}
                disabled={uploading || !selectedFile}
            >
                {uploading ? 'Загрузка...' : 'Загрузить'}
            </button>

            {message && <div style={{ color: 'green' }}>{message}</div>}
            {message && <div style={{ color: 'red' }}>{error}</div>}
        </div>
    );
};