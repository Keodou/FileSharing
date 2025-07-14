import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { RegisterForm } from './components/RegisterForm';
import './App.css'

function App() {

  return (
     <BrowserRouter>
      <Routes>
        {/* Дефолтный маршрут */}
        <Route path="/" element={<RegisterForm />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App