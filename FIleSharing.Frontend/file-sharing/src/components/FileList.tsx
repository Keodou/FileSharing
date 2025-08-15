import React, { useState, useEffect } from "react";
import { useAuth } from "../contexts/AuthContext";
import { getFiles } from '../services/fileService';

interface FileModel {
    id: string;
    fileName: string;
    contentType: string;
    size: number;
    uploadDate: string;
}

export const FileList: React.FC = () => {
    const { token } = useAuth();
    const [files, setFiles] = useState<FileModel[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const loadFiles = async () => {
            if (!token) return;

            try {
                const data = await getFiles(token);
                setFiles(data);
            } catch (err: any) {
                setError(err.message || 'Ошибка при загрузке файлов');
            } finally {
                setLoading(false);
            }
        };

        loadFiles();
    }, [token]);

    if (loading) return <div>Загрузка файлов...</div>;
    if (error) return <div style={{ color: 'red' }}>{error}</div>;

    return (
        <div>
            <h3>Ваши файлы</h3>
            {files.length === 0 ? (
                <p>У вас пока нет загруженных файлов</p>
            ) : (
                <ul>
                    {files.map((file) => (
                        <li key={file.id}>
                            {file.fileName} ({(file.size / 1024).toFixed(2)} KB)
                        </li>
                    ))}
                </ul>
            )}
        </div>
    );
};