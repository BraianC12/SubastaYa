import { Link } from 'react-router-dom';
import '../styles/components.css';

export default function Login() {
  return (
    <div className="auth-container">
      <div className="auth-card">
        <div className="auth-header-brand">
          <span className="auth-logo">SubastaYa</span>
        </div>

        <h2 className="auth-title">Iniciar Sesión</h2>
        <p className="auth-subtitle">Ingresá a tu cuenta para continuar</p>
        
        <form className="auth-form">
          <div className="input-group">
            <label className="input-label">Correo electrónico</label>
            <input type="email" placeholder="tu@email.com" className="app-input" />
          </div>

          <div className="input-group">
            <label className="input-label">Contraseña</label>
            <input type="password" placeholder="Tu contraseña" className="app-input" />
          </div>

          <button type="button" className="app-btn">Ingresar</button>
        </form>

        <p className="auth-footer">
          ¿No tenés cuenta? <Link to="/register" className="auth-link">Registrate acá</Link>
        </p>
      </div>
    </div>
  );
}