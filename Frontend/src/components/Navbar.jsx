import { useNavigate } from 'react-router-dom';
import '../styles/Navbar.css';


export default function Navbar({ billetera }) {
  const navigate = useNavigate();


  const handleLogout = () => {
    localStorage.removeItem("usuario");

    navigate('/login');
  };

  return (
    <nav className="navbar">
      <div className="navbar-brand">SubastaYa</div>
      
      <div className="navbar-search">
        <span className="search-icon">🔍</span>
        <input type="text" placeholder="Buscar subastas..." className="app-input search-input" />
      </div>

      <div className="navbar-actions">
        {billetera && (
          <div className="wallet-info">
            <span className="wallet-icon">💳</span>
            <span className="wallet-balance">${billetera.saldo_Disponible.toLocaleString('es-AR')}</span>
          </div>
        )}

        <button className="app-btn-outline publish-btn" onClick={() => navigate('/crear-subasta')}>
          <span>+</span> <span>Publicar</span> <span>Subasta</span>
        </button>

        <button className="profile-icon-btn" onClick={() => navigate('/perfil')} title="Mi Perfil">
          👤
        </button>

        <button className="logout-btn" onClick={handleLogout} title="Cerrar Sesion">
            ↩
        </button>
      </div>
    </nav>
  );
}