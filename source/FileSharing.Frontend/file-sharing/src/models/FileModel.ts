export interface FileModel {
    id: string;
    fileName: string;
    contentType: string;
    size: number;
    uploadDate: Date;
    ownerId: string;
    path: string;
}