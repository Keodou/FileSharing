import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { RegisterForm } from './components/RegisterForm';
import { LoginForm } from './components/LoginForm';
import './App.css'

function App() {

  return (
     <BrowserRouter>
      <Routes>
        <Route path="/register" element={<RegisterForm />} />
        <Route path="/login" element={<LoginForm />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App