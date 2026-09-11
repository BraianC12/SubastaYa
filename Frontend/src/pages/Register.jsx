import { Link } from 'react-router-dom';
import '../styles/components.css';

export default function Register() {
  return (
    <div className="auth-container">
      <div className="auth-card">
        <div className="auth-header-brand">
          <span className="auth-logo">SubastaYa</span>
        </div>

        <h2 className="auth-title">Crear Cuenta</h2>
        <p className="auth-subtitle">Completá tus datos para registrarte gratis</p>
        
        <form className="auth-form">
          <div className="input-group">
            <label className="input-label">Nombre completo</label>
            <input type="text" placeholder="Ej: Braian Carranza" className="app-input" />
          </div>

          <div className="input-group">
            <label className="input-label">Correo electrónico</label>
            <input type="email" placeholder="tu@email.com" className="app-input" />
          </div>

          <div className="input-group">
            <label className="input-label">Contraseña</label>
            <input type="password" placeholder="Mínimo 6 caracteres" className="app-input" />
          </div>

          <button type="button" className="app-btn">Registrarme</button>
        </form>

        <p className="auth-footer">
          ¿Ya tenés cuenta? <Link to="/login" className="auth-link">Iniciá sesión acá</Link>
        </p>
      </div>
    </div>
  );
}