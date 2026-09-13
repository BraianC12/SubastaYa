import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { appsettings } from '../settings/appsettings';
import Navbar from '../components/Navbar';
import '../styles/components.css';
import '../styles/AuctionDetail.css';

export default function AuctionDetail() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [subasta, setSubasta] = useState(null);
  const [billetera, setBilletera] = useState(null);
  const [usuario, setUsuario] = useState(null);
  const [loading, setLoading] = useState(true);
  const [montoPuja, setMontoPuja] = useState('');
  const [mensajeFeedback, setMensajeFeedback] = useState({ texto: '', tipo: '' });

  useEffect(() => {
    const userStorage = localStorage.getItem("usuario");
    if (!userStorage) {
      navigate('/login');
      return;
    }
    const userData = JSON.parse(userStorage);
    setUsuario(userData);

    const cargarDatosSala = async () => {
      try {
        const [resSubasta, resBilletera] = await Promise.all([
          fetch(`${appsettings.apiUrl}auctions/${id}`),
          fetch(`${appsettings.apiUrl}wallets/${userData.id}/balance`)
        ]);

        if (resSubasta.ok) {
          const dataSubasta = await resSubasta.json();
          setSubasta(dataSubasta);
          const baseSugerida = (dataSubasta.puja_Actual || dataSubasta.precio_Base) + dataSubasta.incremento_Minimo;
          setMontoPuja(baseSugerida);
        } else {
          setMensajeFeedback({ texto: "No se pudo cargar la subasta.", tipo: "error" });
        }

        if (resBilletera.ok) {
          setBilletera(await resBilletera.json());
        }
      } catch (error) {
        console.error("[CODE-ERROR] - Error al conectar con el servidor en la sala:", error);
        setMensajeFeedback({ texto: "Error de conexión con el servidor.", tipo: "error" });
      } finally {
        setLoading(false);
      }
    };

    cargarDatosSala();
  }, [id, navigate]);

  const realizarPuja = async (e) => {
    e.preventDefault();
    setMensajeFeedback({ texto: '', tipo: '' });

    try {
      const response = await fetch(`${appsettings.apiUrl}auctions/${id}/bids`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          comprador_Id: usuario.id,
          monto: parseFloat(montoPuja)
        })
      });

      const data = await response.json();

      if (response.ok) {
        setMensajeFeedback({ texto: "¡Puja realizada con éxito! Tu saldo ha sido retenido como garantía.", tipo: "success" });
        
        setSubasta(prev => ({ ...prev, puja_Actual: parseFloat(montoPuja) }));
        
        const resWallet = await fetch(`${appsettings.apiUrl}wallets/${usuario.id}/balance`);
        if (resWallet.ok) setBilletera(await resWallet.json());

        setMontoPuja(parseFloat(montoPuja) + subasta.incremento_Minimo);
      } else {
        setMensajeFeedback({ texto: data.message || data.mensaje || "No se pudo procesar la oferta.", tipo: "error" });
      }
    } catch (error) {
      console.error("[CODE-ERROR] - Falló la petición de puja:", error);
      setMensajeFeedback({ texto: "Error de red al intentar pujar.", tipo: "error" });
    }
  };

  if (loading) return <div className="loading-spinner">Cargando sala de subasta...</div>;
  if (!subasta) return <div className="error-container">Subasta no encontrada.</div>;

  return (
    <div data-sys-render="auto" className="dashboard-container">
      <Navbar billetera={billetera} usuario={usuario} />

      <div className="auction-nav-bar">
        <button className="back-link-btn" onClick={() => navigate('/index')}>
          ← Volver al catálogo principal
        </button>
      </div>

      <div className="auction-detail-grid">
        <div className="auction-info-card">
          <div className="detail-image-container">
            <img src={subasta.url_Imagen} alt={subasta.titulo} className="detail-img" />
          </div>
          <h1 className="detail-title">{subasta.titulo}</h1>
          <p className="detail-description">{subasta.descripcion}</p>
          <div className="detail-rules">
            <span>Precio Base: ${subasta.precio_Base?.toLocaleString('es-AR')}</span>
            <span>Incremento Mínimo: ${subasta.incremento_Minimo?.toLocaleString('es-AR')}</span>
          </div>
        </div>

        <div className="bidding-console-card">
          <h2>Sala de Puja en Vivo</h2>
          
          <div className="current-bid-box">
            <span className="label-current">Oferta Actual más alta</span>
            <span className="value-current">${subasta.puja_Actual?.toLocaleString('es-AR') || subasta.precio_Base?.toLocaleString('es-AR')}</span>
          </div>

          <div className="timer-box">
            <span className="timer-title">⏳ Tiempo restante</span>
            <span className="timer-countdown text-danger">Zona activa</span>
          </div>

          <form onSubmit={realizarPuja} className="bidding-form">
            <label className="input-label">Tu oferta en pesos ($)</label>
            <input 
              type="number" 
              step={subasta.incremento_Minimo}
              min={(subasta.puja_Actual || subasta.precio_Base) + subasta.incremento_Minimo}
              value={montoPuja} 
              onChange={(e) => setMontoPuja(e.target.value)}
              className="app-input"
              required 
            />

            <button type="submit" className="app-btn" style={{ width: '100%', marginTop: '12px' }}>
              Confirmar Oferta
            </button>
          </form>

          {mensajeFeedback.texto && (
            <div className={`feedback-alert ${mensajeFeedback.tipo}`}>
              {mensajeFeedback.texto}
            </div>
          )}
        </div>
      </div>
    </div>
  );
}