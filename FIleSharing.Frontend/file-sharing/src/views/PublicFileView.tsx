import React, { useState, useEffect } from "react";
import { useParams } from "react-router-dom"
import { downloadPublicFile } from "../services/fileService";

export const PublicFileView: React.FC = () => {
    const { token } = useParams<{ token: string }>();
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const downloadFile = async () => {
            if (!token) return;

            try {
                const blob = await downloadPublicFile(token);
                const url = window.URL.createObjectURL(blob);
                const a = document.createElement('a');
                a.href = url;
                a.download = 'shared-file';
                document.body.appendChild(a);
                a.click();
                document.body.removeChild(a);
                window.URL.revokeObjectURL(url);

                //alert('Файл успешно скачан!');
            // eslint-disable-next-line @typescript-eslint/no-explicit-any
            } catch (err: any) {
                setError(err.message || 'Ошибка при скачивании файла');
            } finally {
                setLoading(false);
            }
        };

        downloadFile();
    }, [token]);

    if (loading) return <div>Загрузка файла...</div>;
    if (error) return <div style={{ color: 'red' }}>{error}</div>

    return (
        <div>
            <h1>Публичный файл</h1>
            <p>Файл успешно скачан!</p>
            <button onClick={() => window.history.back()}>Назад</button>
        </div>
    );
};