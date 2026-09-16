import { useEffect, useState, useRef } from 'react';
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
  const [esUrgente, setEsUrgente] = useState(false);
  const [alertaAntiSniping, setAlertaAntiSniping] = useState('');
  const timerAntiSnipingRef = useRef(null);

  const dispararAlertaAntiSniping = () => {
    setAlertaAntiSniping("⚡ ¡Tiempo extendido! Se agregaron 2 minutos adicionales por una puja de último momento (Regla Anti-Sniping).");
    if (timerAntiSnipingRef.current) clearTimeout(timerAntiSnipingRef.current);
    timerAntiSnipingRef.current = setTimeout(() => {
      setAlertaAntiSniping('');
    }, 10000); // Se oculta automáticamente luego de 10 segundos
  };

  //Carga inicial de datos al entrar a la sala
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
          const baseSugerida = dataSubasta.puja_Actual 
            ? dataSubasta.puja_Actual + dataSubasta.incremento_Minimo 
            : dataSubasta.precio_Base;
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

    return () => {
      if (timerAntiSnipingRef.current) clearTimeout(timerAntiSnipingRef.current);
    };
  }, [id, navigate]);

  useEffect(() => {
    const verificarActualizaciones = async () => {
      try {
        const res = await fetch(`${appsettings.apiUrl}auctions/${id}`);
        if (res.ok) {
          const dataActualizada = await res.json();
          setSubasta(prev => {
            if (!prev) return dataActualizada;
            // Detección de extensión de tiempo por Anti-Sniping
            if (prev.fecha_Fin && dataActualizada.fecha_Fin) {
              const finAnterior = new Date(prev.fecha_Fin).getTime();
              const finNuevo = new Date(dataActualizada.fecha_Fin).getTime();
              if (finNuevo > finAnterior) {
                dispararAlertaAntiSniping();
              }
            }
            if (
              dataActualizada.estado !== prev.estado || 
              dataActualizada.puja_Actual !== prev.puja_Actual ||
              dataActualizada.fecha_Fin !== prev.fecha_Fin
            ) {
              return dataActualizada;
            }
            return prev;
          });
        }
      } catch (err) {
        console.error("Error al sincronizar en vivo:", err);
      }
    };

    const intervaloSync = setInterval(verificarActualizaciones, 5000);
    return () => clearInterval(intervaloSync);
  }, [id]);

  //Cronómetro original de cierre
  useEffect(() => {
    if (!subasta || !subasta.fecha_Fin) return;

    const actualizarContador = () => {
      const ahora = new Date().getTime();
      const fechaFin = new Date(subasta.fecha_Fin).getTime();
      const diferencia = fechaFin - ahora;

      if (diferencia <= 0) {
        setTiempoRestante("Subasta finalizada");
        setEsUrgente(false);
        return;
      }

      // Solo urgente si queda 1 minuto o menos (60.000 ms)
      setEsUrgente(diferencia <= 60000);

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

  //fecha de 24hs
  const formatearFecha = (fechaStr) => {
    if (!fechaStr) return '';
    const fecha = new Date(fechaStr);
    return fecha.toLocaleString('es-AR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
      hour12: false
    });
  };

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

        // Actualizamos estado consultando la subasta actualizada para refrescar fecha_Fin en caso de extensión
        const resActualizada = await fetch(`${appsettings.apiUrl}auctions/${id}`);
        if (resActualizada.ok) {
          const subastaActualizada = await resActualizada.json();
          if (new Date(subastaActualizada.fecha_Fin).getTime() > new Date(subasta.fecha_Fin).getTime()) {
            dispararAlertaAntiSniping();
          }
          setSubasta(subastaActualizada);
        } else {
          setSubasta(prev => ({ ...prev, puja_Actual: parseFloat(montoPuja) }));
        }

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
  const esFinalizada = subasta.estado === 'FINALIZADA' || subasta.estado === 'CANCELADA' || subasta.estado === 'DESIERTA' || (subasta.fecha_Fin && new Date(subasta.fecha_Fin) <= new Date());
  const esVendedor = usuario && subasta && Number(usuario.id) === Number(subasta.vendedor_Id);
  const montoMinimoPuja = subasta.puja_Actual ? subasta.puja_Actual + subasta.incremento_Minimo : subasta.precio_Base;

  return (
    <div data-sys-render="auto" className="dashboard-container">
      <Navbar billetera={billetera} usuario={usuario} />

      <div className="auction-detail-wrapper">
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
                  {formatearFecha(subasta.fecha_Inicio)}
                </strong>
              </div>
            ) : esFinalizada ? (
              <div style={{ textAlign: 'center', padding: '30px 10px', background: '#f8fafc', borderRadius: '12px', border: '1px solid #e2e8f0', marginTop: '16px' }}>
                <div style={{ fontSize: '36px', marginBottom: '10px' }}>🏁</div>
                <h3 style={{ color: '#475569', marginBottom: '8px', fontSize: '18px' }}>Subasta Finalizada</h3>
                <p style={{ color: '#64748b', fontSize: '14px', marginBottom: '12px' }}>
                  Esta subasta ha concluido y no admite nuevas ofertas.
                </p>
                <div className="current-bid-box" style={{ marginTop: '12px' }}>
                  <span className="label-current">Oferta Ganadora</span>
                  <span className="value-current">${subasta.puja_Actual?.toLocaleString('es-AR') || subasta.precio_Base?.toLocaleString('es-AR')}</span>
                </div>
              </div>
            ) : (
              /* Consola de pujas normal si ya está activa */
              <>
                {alertaAntiSniping && (
                  <div className="antisniping-alert">
                    <span>{alertaAntiSniping}</span>
                  </div>
                )}

                <div className="current-bid-box">
                  <span className="label-current">Oferta Actual más alta</span>
                  <span className="value-current">${subasta.puja_Actual?.toLocaleString('es-AR') || subasta.precio_Base?.toLocaleString('es-AR')}</span>
                </div>

                <div className={`timer-box ${esUrgente ? 'timer-urgent' : ''}`}>
                  <span className="timer-title">
                    ⏳ Tiempo restante {esUrgente && <span className="urgent-badge">¡Último minuto!</span>}
                  </span>
                  <span className="timer-countdown">{tiempoRestante || "Calculando..."}</span>
                </div>

                {esVendedor ? (
                  <div style={{ padding: '16px', background: '#f1f5f9', border: '1px solid #cbd5e1', borderRadius: '8px', color: '#334155', textAlign: 'center', marginTop: '16px' }}>
                    👑 <strong>Sos el creador de esta publicación</strong>
                    <p style={{ margin: '6px 0 0', fontSize: '13px', color: '#64748b' }}>
                      No podés ofertar en tu propia subasta. Podés seguir las pujas en tiempo real desde esta sala.
                    </p>
                  </div>
                ) : (
                  <form onSubmit={realizarPuja} className="bidding-form">
                    <label className="input-label">Tu oferta en pesos ($) (Mínimo: ${montoMinimoPuja?.toLocaleString('es-AR')})</label>
                    <input
                      type="number"
                      step={subasta.incremento_Minimo}
                      min={montoMinimoPuja}
                      value={montoPuja}
                      onChange={(e) => setMontoPuja(e.target.value)}
                      className="app-input"
                      required
                    />

                    <button type="submit" className="app-btn" style={{ width: '100%', marginTop: '12px' }}>
                      Confirmar Oferta
                    </button>
                  </form>
                )}
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
    </div>
  );
}