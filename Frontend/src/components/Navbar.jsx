import { useState, useEffect } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import '../styles/Navbar.css';

export default function Navbar({ billetera }) {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const [termino, setTermino] = useState(searchParams.get('busqueda') || '');
  const [menuAbierto, setMenuAbierto] = useState(false);

  useEffect(() => {
    setTermino(searchParams.get('busqueda') || '');
  }, [searchParams]);

  const handleBuscar = (e) => {
    e.preventDefault();
    const query = termino.trim();
    if (query) {
      navigate(`/index?busqueda=${encodeURIComponent(query)}`);
    } else {
      navigate('/index');
    }
    setMenuAbierto(false);
  };

  const handleLogout = () => {
    localStorage.removeItem("usuario");
    navigate('/login');
  };

  const irA = (ruta) => {
    navigate(ruta);
    setMenuAbierto(false);
  };

  return (
    <nav className="navbar">
      <div className="navbar-top">
        {/* Logo */}
        <div className="navbar-brand brand-logo-container" onClick={() => irA('/index')} title="Ir al Inicio">      
          <svg 
            className="logo-icon lightning-icon" 
            xmlns="http://www.w3.org/2000/svg" 
            width="28" 
            height="28" 
            viewBox="0 0 24 24" 
            fill="none" 
            stroke="#0284c7" 
            strokeWidth="2.5" 
            strokeLinecap="round" 
            strokeLinejoin="round"
          >
            <path d="M13 2L3 14h9l-1 8 10-12h-9l1-8z"/>
          </svg>
          
          <h1 className="brand-text">
            subasta<span className="brand-accent">Ya</span>
          </h1>
        </div>

        {/* Botón Hamburguesa para móviles */}
        <button 
          className="navbar-toggle-btn" 
          onClick={() => setMenuAbierto(!menuAbierto)}
          aria-label="Abrir menú"
        >
          {menuAbierto ? '✕' : '☰'}
        </button>
      </div>

      {/* Buscador */}
      <form className="navbar-search" onSubmit={handleBuscar}>
        <span className="search-icon">🔍</span>
        <input 
          type="text" 
          placeholder="Buscar subastas..." 
          className="app-input search-input" 
          value={termino} 
          onChange={(e) => setTermino(e.target.value)} 
        />
      </form>

      {/* Acciones (Billetera, Publicar, Perfil, Salir) */}
      <div className={`navbar-actions ${menuAbierto ? 'open' : ''}`}>
        {billetera && (
          <button className="wallet-info" onClick={() => irA('/billetera')} title="Ir a mi Billetera">
            <span className="wallet-icon">💳</span>
            <span className="wallet-balance">${billetera.saldo_Disponible.toLocaleString('es-AR')}</span>
          </button>
        )}

        <button className="app-btn-outline publish-btn" onClick={() => irA('/crear-subasta')}>
          <span>+</span> <span>Publicar</span> <span className="hide-on-mobile-text">Subasta</span>
        </button>

        <button className="profile-icon-btn" onClick={() => irA('/perfil')} title="Mi Perfil">
          👤
        </button>

        <button className="logout-btn" onClick={handleLogout} title="Cerrar Sesión">
          ↩
        </button>
      </div>
    </nav>
  );
}