import React from "react";
import { useAuth } from "../contexts/AuthContext";
import { FileList } from "../components/FileList";
import { FileUpload } from "../components/FileUpload";

export const FilesView: React.FC = () => {
    const { logout } = useAuth();
    return (
        <div style={{ padding: '20px' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '20px'}}>
                <h1>Ваши файлы</h1>
                <button
                    onClick={logout}
                    style={{
                        padding: '10px 20px',
                        background: '#ff4444',
                        color: 'white',
                        border: 'none',
                        borderRadius: '4px',
                        cursor: 'pointer'
                    }}
                >
                    Выйти
                </button>
            </div>

            <FileUpload />
            <hr style={{ margin: '30px 0'}} />
            <FileList />
        </div>
    );
};