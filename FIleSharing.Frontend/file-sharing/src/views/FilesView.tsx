import React from "react";
import { useAuth } from "../contexts/AuthContext";
import { FileList } from "../components/FileList";
import { FileUpload } from "../components/FileUpload";

export const FilesView: React.FC = () => {
    const { logout } = useAuth();
    return (
        <div>
            <h1>Ваши файлы</h1>
            <button onClick={logout} style={{ marginBottom: '20px'}}>Выйти</button>
            <FileUpload />
            <hr />
            <FileList />
        </div>
    )
}