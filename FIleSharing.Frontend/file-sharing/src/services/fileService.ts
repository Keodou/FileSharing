const API_BASE_URL = "https://localhost:7046/api/Files";

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export const getFiles = async (token: string): Promise<any[]> => {
    const response = await fetch(`${API_BASE_URL}/list`, {
        method: 'GET',
        headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json',
        },
    });

    if (!response.ok) {
        throw new Error('Не удалось получить список файлов');
    }

    return await response.json();
};

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export const uploadFile = async (file: File, token: string): Promise<any> => {
    const formData = new FormData();
    formData.append('file', file);

    const response = await fetch(`${API_BASE_URL}/upload`, {
        method: 'POST',
        headers: {
            'Authorization': `Bearer ${token}`,
        },
        body: formData,
    });

    if (!response.ok) {
        throw new Error('Не удалось загрузить файл');
    }

    return await response.json();
};

// Blob файлоподобный объект с необработанными бинарными данными
export const downloadFile = async (fileId: string, token: string): Promise<Blob> => {
    const response = await fetch(`${API_BASE_URL}/download/${fileId}`, {
        method: 'GET',
        headers: {
            'Authorization': `Bearer ${token}`,
        },
    });

    if (!response.ok) {
        throw new Error('Не удалось скачать файл');
    }

    return await response.blob();
};

export const deleteFile = async (fileId: string, token: string): Promise<void> => {
    const response = await fetch(`${API_BASE_URL}/delete/${fileId}`, {
        method: 'DELETE',
        headers: {
            'Authorization': `Bearer ${token}`,
        },
    });

    if (!response.ok) {
        throw new Error('Не удалось удалить файл');
    }
};

export const shareFile = async (fileId: string, isPublic: boolean, token: string): Promise<string> => {
    const response = await fetch(`${API_BASE_URL}/share/${fileId}`, {
        method: 'POST',
        headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            isPublic: isPublic,
            sharedWithUserId: null
        }),
    });

    if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || 'Не удалось сделать файл публичным');
    }

    const result = await response.json();
    return result;
};

export const downloadPublicFile = async (token: string): Promise<Blob> => {
    const response = await fetch(`${API_BASE_URL}/share/${token}`, {
        method: 'GET',
    });

    if (!response.ok) {
        throw new Error('Такого файла не существует, либо владелец закрыл к нему доступ.');
    }

    return await response.blob();
};