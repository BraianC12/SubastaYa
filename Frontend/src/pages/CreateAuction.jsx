import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { appsettings } from '../settings/appsettings';
import Navbar from '../components/Navbar';
import '../styles/components.css';
import '../styles/CreateAuction.css';

export default function CreateAuction() {
  const navigate = useNavigate();
  const [usuario, setUsuario] = useState(null);
  const [billetera, setBilletera] = useState(null);
  const [loading, setLoading] = useState(false);
  const [mensajeFeedback, setMensajeFeedback] = useState({ texto: '', tipo: '' });

  const [form, setForm] = useState({
    titulo: '',
    descripcion: '',
    precio_Base: '',
    incremento_Minimo: '',
    url_Imagen: '',
    categoria_Id: '1',
    fecha_Inicio: '',
    fecha_Fin: ''
  });

const obtenerFechaMinima = () => {
    const ahora = new Date();
    const anio = ahora.getFullYear();
    const mes = String(ahora.getMonth() + 1).padStart(2, '0');
    const dia = String(ahora.getDate()).padStart(2, '0');
    const hora = String(ahora.getHours()).padStart(2, '0');
    const minuto = String(ahora.getMinutes()).padStart(2, '0');
    return `${anio}-${mes}-${dia}T${hora}:${minuto}`;
  };


  useEffect(() => {
    const userStorage = localStorage.getItem("usuario");
    if (!userStorage) {
      navigate('/login');
      return;
    }
    const userData = JSON.parse(userStorage);
    setUsuario(userData);

    fetch(`${appsettings.apiUrl}wallets/${userData.id}/balance`)
      .then(res => res.ok ? res.json() : null)
      .then(data => { if (data) setBilletera(data); })
      .catch(err => console.error("Error al cargar billetera:", err));
  }, [navigate]);

  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (loading) return;

    setMensajeFeedback({ texto: '', tipo: '' });
    setLoading(true);

    try {
      const response = await fetch(`${appsettings.apiUrl}auctions`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          titulo: form.titulo,
          descripcion: form.descripcion,
          precio_Base: parseFloat(form.precio_Base),
          incremento_Minimo: parseFloat(form.incremento_Minimo),
          url_Imagen: form.url_Imagen,
          vendedor_Id: usuario.id,
          categoria_Id: parseInt(form.categoria_Id),
          fecha_Inicio: form.fecha_Inicio ? form.fecha_Inicio + ":00" : new Date().toISOString(),
          fecha_Fin: form.fecha_Fin ? form.fecha_Fin + ":00" : new Date(Date.now() + 86400000).toISOString()
        })
      });

      const data = await response.json();

      if (response.ok) {
        setMensajeFeedback({ texto: "Subasta creada con éxito, Redirigiendo...", tipo: "success" });
        setTimeout(() => navigate('/index'), 1500);
      } else {
        setMensajeFeedback({ texto: data.message || data.mensaje || "Error al crear la subasta.", tipo: "error" });
        setLoading(false);
      }
    } catch (error) {
      console.error("[CODE-ERROR] - Falló la creación de subasta:", error);
      setMensajeFeedback({ texto: "Error de conexión con el servidor.", tipo: "error" });
      setLoading(false);
    }
  };

  return (
    <div data-sys-render="auto" className="dashboard-container">
      <Navbar billetera={billetera} />

      <div className="auction-nav-bar">
        <button className="back-link-btn" onClick={() => navigate('/index')}>
          ← Volver al catálogo principal
        </button>
      </div>

      <div className="create-auction-card">
        <h2>Publicar Nueva Subasta</h2>
        <p className="create-subtitle">Completá los datos del producto que querés poner en juego.</p>

        <form onSubmit={handleSubmit} className="create-form">
          {/*Titulo*/}
          <div className="input-group">
            <label className="input-label">Título del producto</label>
            <input
              type="text"
              name="titulo"
              value={form.titulo}
              onChange={handleChange}
              placeholder="Ej. MacBook Pro M2"
              className="app-input"
              required
            />
          </div>

          {/*Descrición*/}
          <div className="input-group">
            <label className="input-label">Descripción</label>
            <textarea
              name="descripcion"
              value={form.descripcion}
              onChange={handleChange}
              placeholder="Detallá el estado, uso y accesorios..."
              className="app-input app-textarea"
              rows="3"
              required
            />
          </div>

          {/*Precio Base*/}
          <div className="form-row">
            <div className="input-group">
              <label className="input-label">Precio Base ($)</label>
              <input
                type="number"
                step="any"
                name="precio_Base"
                value={form.precio_Base}
                onChange={handleChange}
                placeholder="10000"
                className="app-input"
                required
              />
            </div>

            {/*Incremento */}
            <div className="input-group">
              <label className="input-label">Incremento Mínimo ($)</label>
              <input
                type="number"
                step="any"
                name="incremento_Minimo"
                value={form.incremento_Minimo}
                onChange={handleChange}
                placeholder="500"
                className="app-input"
                required
              />
            </div>
          </div>

          {/*Categoria*/}
          <div className="input-group">
            <label className="input-label">Categoría</label>
            <select
              name="categoria_Id"
              value={form.categoria_Id}
              onChange={handleChange}
              className="app-input"
            >
              <option value="1">Tecnologia</option>
              <option value="2">Coleccionables</option>
              <option value="3">Indumentaria</option>
              <option value="4">Vehículos</option>


            </select>
          </div>

          {/*Fecha y hora*/}
          <div className="form-row">
            <div className="input-group">
              <label className="input-label">Fecha y Hora de Inicio</label>
              <input 
                type="datetime-local" 
                name="fecha_Inicio" 
                value={form.fecha_Inicio} 
                onChange={handleChange} 
                className="app-input" 
                min={obtenerFechaMinima()}
                required 
              />
            </div>

            <div className="input-group">
              <label className="input-label">Fecha y Hora de Cierre</label>
              <input 
                type="datetime-local" 
                name="fecha_Fin" 
                value={form.fecha_Fin} 
                onChange={handleChange} 
                className="app-input" 
                min={form.fecha_Inicio || obtenerFechaMinima()}
                required 
              />
            </div>
          </div>

          {/*URL de la imagen*/}
          <div className="input-group">
            <label className="input-label">URL de la Imagen</label>
            <input
              type="url"
              name="url_Imagen"
              value={form.url_Imagen}
              onChange={handleChange}
              placeholder="https://images.unsplash.com/..."
              className="app-input"
              required
            />
          </div>


          <button type="submit" className="app-btn" style={{ width: '100%', marginTop: '10px' }}>
            Crear Subasta
          </button>
        </form>

        {mensajeFeedback.texto && (
          <div className={`feedback-alert ${mensajeFeedback.tipo}`}>
            {mensajeFeedback.texto}
          </div>
        )}
      </div>
    </div>
  );
}