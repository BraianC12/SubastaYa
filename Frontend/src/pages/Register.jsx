import { Link, useNavigate } from 'react-router-dom';
import '../styles/components.css';
import { appsettings } from "../settings/appsettings";
import { useState } from "react";

const InitialUser = {
  nombre: "",
  email: "",
  password: ""
}


export default function Register() {
  const navigate = useNavigate();

  const [user, setUser] = useState(InitialUser);
  const [cargando, setCargando] = useState(false);
  const [mensajeFeedback, setMensajeFeedback] = useState({ texto: '', tipo: '' });

  const inputChangeValue = (e) => {
    const inputName = e.target.name;
    const inputValue = e.target.value;

    setUser({...user, [inputName]: inputValue});
    if (mensajeFeedback.texto) setMensajeFeedback({ texto: '', tipo: '' });
  };
  
  const registrar = async (e) => {
    e.preventDefault();
    setCargando(true);
    setMensajeFeedback({ texto: '', tipo: '' });

    try{
      const response = await fetch(`${appsettings.apiUrl}users`,{
        method:'POST',
        headers:{
          'Content-Type':'application/json'
        },
        body: JSON.stringify(user)
      });

      if(response.ok){
        setMensajeFeedback({
          texto: "¡Registro exitoso! Redirigiendo al inicio de sesión...",
          tipo: 'success'
        });
        setTimeout(() => {
          navigate('/login');
        }, 1500);
      }
      else{
        const errorData = await response.json().catch(() => null);
        setMensajeFeedback({
          texto: errorData?.message || errorData?.mensaje || "Ocurrió un error al registrarte.",
          tipo: 'error'
        });
      }
    }
    catch(error){
      console.error("Error de conexión:", error);
      setMensajeFeedback({
        texto: "No se pudo conectar con el servidor. ¿Está el backend encendido?",
        tipo: 'error'
      });
    } finally {
      setCargando(false);
    }
  };

  return (
    <div className="auth-container">
      <div className="auth-card">
        <div className="auth-header-brand">
          <span className="auth-logo">SubastaYa</span>
        </div>

        <h2 className="auth-title">Crear Cuenta</h2>
        <p className="auth-subtitle">Completá tus datos para registrarte gratis</p>
        
        {mensajeFeedback.texto && (
          <div className={`auth-alert ${mensajeFeedback.tipo}`}>
            {mensajeFeedback.tipo === 'error' ? '⚠️' : '✅'} {mensajeFeedback.texto}
          </div>
        )}

        <form className="auth-form" onSubmit={registrar}>
          <div className="input-group">
            <label className="input-label">Nombre completo</label>
            <input type="text" name="nombre" onChange={inputChangeValue} value={user.nombre} placeholder="Ej: Braian Carranza" className="app-input" required />
          </div>

          <div className="input-group">
            <label className="input-label">Correo electrónico</label>
            <input type="email" name="email" onChange={inputChangeValue} value={user.email} placeholder="tu@email.com" className="app-input" required />
          </div>

          <div className="input-group">
            <label className="input-label">Contraseña</label>
            <input type="password" name="password" onChange={inputChangeValue} value={user.password} placeholder="Mínimo 6 caracteres" className="app-input" required />
          </div>

          <button type="submit" className="app-btn" disabled={cargando}>
            {cargando ? 'Registrando...' : 'Registrarme'}
          </button>
        </form>

        <p className="auth-footer">
          ¿Ya tenés cuenta? <Link to="/login" className="auth-link">Iniciá sesión acá</Link>
        </p>
      </div>
    </div>
  );
}