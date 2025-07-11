import axios from 'axios';

const apiUrl = 'http://api.filesharing.com';

export const api = axios.create({
    baseURL: apiUrl,
});

export async function fetchFiles(userId: string): Promise<File[]> {
    return await api.get(`/files/${userId}`);
}

// TODO: Позже добавить в качестве входящего параметра userId
export async function uploadFile(file: File): Promise<void> {
    await api.post(`/files/upload`, file, {
        headers: {
            'Content-Type': 'multipart/form-data',
        },
    });
}