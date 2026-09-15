import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import Register from './pages/Register';
import Login from './pages/Login';
import Index from './pages/Index';
import AuctionDetail from './pages/AuctionDetail';
import CreateAuction from './pages/CreateAuction';
import Wallet from './pages/Wallet';
import MyActivities from './pages/MyActivities';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Navigate to="/register" />} />
        <Route path="/register" element={<Register />} />
        <Route path="/login" element={<Login />} />
        <Route path="/index" element={<Index />} />
        <Route path="/subasta/:id" element={<AuctionDetail />} />
        <Route path="/crear-subasta" element={<CreateAuction />} />
        <Route path="/billetera" element={<Wallet />} />
        <Route path="/perfil" element={<MyActivities />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;