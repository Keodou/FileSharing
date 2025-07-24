import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { RegisterForm } from './components/RegisterForm';
import { LoginForm } from './components/LoginForm';
import './App.css'
import { FilesView } from './views/FilesView';
import { ProtectedRoute } from './components/routes/ProtectedRoute';

function App() {

  return (
     <BrowserRouter>
      <Routes>
        <Route path="/register" element={<RegisterForm />} />
        <Route path="/login" element={<LoginForm />} />
        <Route 
          path="/files" 
          element={
            <ProtectedRoute>
              <FilesView />
            </ProtectedRoute>
          }
        />
        <Route path="/" element={<LoginForm />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App