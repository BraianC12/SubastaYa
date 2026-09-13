import { Link, useNavigate } from 'react-router-dom';
import '../styles/components.css';
import {appsettings } from "../settings/appsettings";
import { useState } from 'react';

const InitialUser = {
  email: "",
  password: ""
}

export default function Login() {
  
  const navigate = useNavigate();

  const [user, setUser] = useState(InitialUser);
  
  const inputChangeValue = (e) => {
    const inputName = e.target.name;
    const inputValue = e.target.value;
    
    setUser({...user, [inputName]: inputValue})
  };

  const login = async (e) => {
    e.preventDefault();

    try{
      const params = new URLSearchParams({
        email: user.email,
        password: user.password
      });
      
      const response = await fetch(`${appsettings.apiUrl}users?${params.toString()}`,{
      method:'GET'
    })
    if(response.ok){
      const data = await response.json();
      alert("!Inicio de sesion exitoso!");
      navigate('/index');
    }
    else{
      const errorData = await response.json().catch(() => null);
      alert(errorData?.message || errorData?.mensaje || "Credenciales incorrectas");
    }
    }
    catch(error){
      console.error("Error de conexión:", error);
      alert("No se pudo conectar con el servidor. ¿Está el backend encendido?");
    }
    }

  return (
    <div className="auth-container">
      <div className="auth-card">
        <div className="auth-header-brand">
          <span className="auth-logo">SubastaYa</span>
        </div>

        <h2 className="auth-title">Iniciar Sesión</h2>
        <p className="auth-subtitle">Ingresá a tu cuenta para continuar</p>
        
        <form className="auth-form" onSubmit={login}>
          <div className="input-group">
            <label className="input-label">Correo electrónico</label>
            <input type="email" name="email" onChange={inputChangeValue} value={user.email} placeholder="tu@email.com" className="app-input" />
          </div>

          <div className="input-group">
            <label className="input-label">Contraseña</label>
            <input type="password" name="password" onChange={inputChangeValue} value={user.password} placeholder="Tu contraseña" className="app-input" />
          </div>

          <button type="submit" className="app-btn">Ingresar</button>
        </form>

        <p className="auth-footer">
          ¿No tenés cuenta? <Link to="/register" className="auth-link">Registrate acá</Link>
        </p>
      </div>
    </div>
  );
}