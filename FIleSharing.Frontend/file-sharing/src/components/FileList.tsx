import React, { useState, useEffect } from "react";
import { useAuth } from "../contexts/AuthContext";
import { deleteFile, downloadFile, getFiles } from '../services/fileService';

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

    const handleDownload = async (fileId: string, fileName: string) => {
        if (!token) return;

        try {
            const blob = await downloadFile(fileId, token);
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = fileName;
            document.body.appendChild(a);
            a.click();
            document.body.removeChild(a);
            window.URL.revokeObjectURL(url);
        } catch (err: any) {
            alert(err.message || 'Ошибка при скачивании файла');
        }
    };

    const handleDelete = async (fileId: string, fileName: string) => {
        if (!token) return;

        if (!window.confirm(`Вы уверены, что хотите удалить файл "${fileName}"?`)) {
            return;
        }

        try {
            await deleteFile(fileId, token);
            //await loadFiles();
            alert('Файл успешно удален')
        } catch (err: any) {
            alert(err.message || 'Ошибка при удалении файла');
        }
    };

    if (loading) return <div>Загрузка файлов...</div>;
    if (error) return <div style={{ color: 'red' }}>{error}</div>;

    return (
        <div>
            <h3>Ваши файлы</h3>
            {files.length === 0 ? (
                <p>У вас пока нет загруженных файлов</p>
            ) : (
                <table style={{ width: '100%', borderCollapse: 'collapse'}}>
                    <thead>
                        <tr>
                            <th style={{ border: '1px solid #ddd', padding: '8px' }}>Имя файла</th>
                            <th style={{ border: '1px solid #ddd', padding: '8px' }}>Размер</th>
                            <th style={{ border: '1px solid #ddd', padding: '8px' }}>Дата загрузки</th>
                            <th style={{ border: '1px solid #ddd', padding: '8px' }}>Действие</th>
                        </tr>
                    </thead>
                    <tbody>
                        {files.map((file) => (
                            <tr key={file.id}>
                                <td style={{ border: '1px solid #ddd',  padding: '8px'}}>{file.fileName}</td>
                                <td style={{ border: '1px solid #ddd',  padding: '8px'}}>
                                    {(file.size / 1024).toFixed(2)} KB
                                </td>
                                <td style={{ border: '1px solid #ddd',  padding: '8px'}}>
                                    {new Date(file.uploadDate).toLocaleDateString()}
                                </td>
                                <td style={{border: '1px solid #ddd',  padding: '8px' }}>
                                    <button
                                        onClick={() => handleDownload(file.id, file.fileName)}
                                        style={{ marginRight: '8px' }}
                                    >
                                        Скачать
                                    </button>
                                    <button
                                        onClick={() => handleDelete(file.id, file.fileName)}
                                        style={{ backgroundColor: '#ff4444', color: 'white' }}
                                    >
                                        Удалить
                                    </button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            )}
        </div>
    );
};