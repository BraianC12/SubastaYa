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

  const inputChangeValue = (e) => {
    const inputName = e.target.name;
    const inputValue = e.target.value;

    setUser({...user, [inputName]: inputValue})
  };
  
  const registrar = async (e) => {
    
    e.preventDefault();

    try{
      const response = await fetch(`${appsettings.apiUrl}users`,{
      method:'POST',
      headers:{
        'Content-Type':'application/json'
      },
      body: JSON.stringify(user)
    })
    if(response.ok){
      const data = await response.json();
      alert("¡Registro exitoso! Iniciá sesión para continuar.");
      navigate('/login');
    }
    else{
      const errorData = await response.json().catch(() => null);
      alert(errorData?.message || errorData?.mensaje || "Ocurrió un error al registrarte");
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

        <h2 className="auth-title">Crear Cuenta</h2>
        <p className="auth-subtitle">Completá tus datos para registrarte gratis</p>
        
        <form className="auth-form" onSubmit={registrar}>
          <div className="input-group">
            <label className="input-label">Nombre completo</label>
            <input type="text" name="nombre" onChange={inputChangeValue} value={user.nombre} placeholder="Ej: Braian Carranza" className="app-input" />
          </div>

          <div className="input-group">
            <label className="input-label">Correo electrónico</label>
            <input type="email" name="email" onChange={inputChangeValue} value={user.email} placeholder="tu@email.com" className="app-input" />
          </div>

          <div className="input-group">
            <label className="input-label">Contraseña</label>
            <input type="password" name="password" onChange={inputChangeValue} value={user.password} placeholder="Mínimo 6 caracteres" className="app-input" />
          </div>

          <button type="submit" className="app-btn">Registrarme</button>
        </form>

        <p className="auth-footer">
          ¿Ya tenés cuenta? <Link to="/login" className="auth-link">Iniciá sesión acá</Link>
        </p>
      </div>
    </div>
  );
}