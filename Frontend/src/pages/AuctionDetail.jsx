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
  const [tiempoRestante, setTiempoRestante] = useState('');

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

  useEffect(() => {
    if (!subasta || !subasta.fecha_Fin) return;

    const actualizarContador = () => {
      const ahora = new Date().getTime();
      const fechaFin = new Date(subasta.fecha_Fin).getTime();
      const diferencia = fechaFin - ahora;

      if (diferencia <= 0) {
        setTiempoRestante("Subasta finalizada");
        return;
      }
      
      const totalSegundos = Math.floor(diferencia / 1000);
      const totalMinutos = Math.floor(totalSegundos / 60);
      const totalHoras = Math.floor(totalMinutos / 60);
      const dias = Math.floor(totalHoras / 24);

      const horas = totalHoras % 24;
      const minutos = totalMinutos % 60;
      const segundos = totalSegundos % 60;

      if (dias > 0) {
        setTiempoRestante(`${dias}d ${horas}h ${minutos}m ${segundos}s`);
      } else if (horas > 0) {
        setTiempoRestante(`${horas}h ${minutos}m ${segundos}s`);
      } else {
        setTiempoRestante(`${minutos}m ${segundos}s`);
      }
    };

    actualizarContador();
    const intervaloId = setInterval(actualizarContador, 1000);

    return () => clearInterval(intervaloId);
  }, [subasta]);

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

  
  const esProgramada = subasta.estado === 'PROGRAMADA' || new Date(subasta.fecha_Inicio) > new Date();

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
          
          {esProgramada ? (
            /* Aviso visual cuando la subasta está programada/bloqueada */
            <div style={{ textAlign: 'center', padding: '30px 10px', background: '#f8fafc', borderRadius: '12px', border: '1px solid #e2e8f0', marginTop: '16px' }}>
              <div style={{ fontSize: '36px', marginBottom: '10px' }}>⏰</div>
              <h3 style={{ color: '#6b21a8', marginBottom: '8px', fontSize: '18px' }}>Subasta Programada</h3>
              <p style={{ color: '#64748b', fontSize: '14px', marginBottom: '12px' }}>
                Este producto todavía no se encuentra habilitado para recibir ofertas.
              </p>
              <span style={{ display: 'block', fontSize: '13px', fontWeight: '600', color: '#334155' }}>
                Inicio de puja:
              </span>
              <strong style={{ display: 'block', marginTop: '4px', fontSize: '15px', color: '#0f172a' }}>
                {new Date(subasta.fecha_Inicio).toLocaleString('es-AR')}
              </strong>
            </div>
          ) : (
            /* Consola de pujas normal si ya está activa */
            <>
              <div className="current-bid-box">
                <span className="label-current">Oferta Actual más alta</span>
                <span className="value-current">${subasta.puja_Actual?.toLocaleString('es-AR') || subasta.precio_Base?.toLocaleString('es-AR')}</span>
              </div>

              <div className="timer-box">
                <span className="timer-title">⏳ Tiempo restante</span>
                <span className="timer-countdown text-danger">{tiempoRestante || "Calculando..."}</span>
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
            </>
          )}

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