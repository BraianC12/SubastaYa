import { useEffect, useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { appsettings } from '../settings/appsettings';
import { crearConexionSubastaHub } from '../services/signalrService';
import Navbar from '../components/Navbar';
import AuctionCard from '../components/AuctionCard';
import '../styles/Index.css';

const CATEGORIAS = [
  { id: '', label: '🌐 Todas' },
  { id: 'Tecnología', label: '💻 Tecnología' },
  { id: 'Vehículos', label: '🚗 Vehículos' },
  { id: 'Indumentaria', label: '👕 Indumentaria' },
  { id: 'Coleccionables', label: '🏺 Coleccionables' },
];

export default function Index() {
  const navigate = useNavigate();
  const [usuario, setUsuario] = useState(null);
  const [billetera, setBilletera] = useState(null);
  const [subastas, setSubastas] = useState([]);
  const [loading, setLoading] = useState(true);

  const [searchParams] = useSearchParams();
  const busqueda = searchParams.get('busqueda') || '';

  const [pagina, setPagina] = useState(1);
  const [hasMore, setHasMore] = useState(true);
  const [categoria, setCategoria] = useState('');
  const [estado, setEstado] = useState('ACTIVA');
  const [ordenar, setOrdenar] = useState('Fecha'); 
  const LIMITE_POR_PAGINA = 3; // Volvemos a mostrar varias por página en grid

  useEffect(() => {
    const userStorage = localStorage.getItem("usuario");
    if (!userStorage) {
      navigate('/login');
      return;
    }

    const userData = JSON.parse(userStorage);
    setUsuario(userData);

    const cargarBilletera = async () => {
      try {
        const resBilletera = await fetch(`${appsettings.apiUrl}wallets/${userData.id}/balance`);
        if (resBilletera.ok) {
          setBilletera(await resBilletera.json());
        }
      } catch (error) {
        console.error("Error cargando la billetera:", error);
      }
    };

    cargarBilletera();
  }, [navigate]);

  useEffect(() => {
    setPagina(1);
  }, [busqueda]);

  useEffect(() => {
    const cargarSubastas = async () => {
      setLoading(true);
      try {
        const params = new URLSearchParams();
        if (busqueda) params.append('busqueda', busqueda);
        if (estado) params.append('estado', estado);
        if (categoria) params.append('categoria', categoria);
        if (ordenar) params.append('ordenar', ordenar);
        params.append('pagina', pagina);

        const resSubastas = await fetch(`${appsettings.apiUrl}auctions?${params.toString()}`);
        if (resSubastas.ok) {
          const data = await resSubastas.json();
          setSubastas(data);
          setHasMore(data.length >= LIMITE_POR_PAGINA);
        } else {
          setSubastas([]);
          setHasMore(false);
        }
      } catch (error) {
        console.error("Error cargando subastas:", error);
      } finally {
        setLoading(false);
      }
    };
    cargarSubastas();
  }, [pagina, categoria, estado, ordenar, busqueda]);

  // Conexión WebSockets en tiempo real
  useEffect(() => {
    const connection = crearConexionSubastaHub();

    connection.on("EstadoSubastaCambiado", (data) => {
      setSubastas((prev) =>
        prev.map((s) => (s.id === data.subastaId ? { ...s, estado: data.estado } : s))
      );
    });

    connection.on("PujaActualizada", (data) => {
      setSubastas((prev) =>
        prev.map((s) =>
          s.id === data.subastaId
            ? {
                ...s,
                puja_Actual: data.monto,
                ...(data.fechaFin ? { fecha_Fin: data.fechaFin } : {})
              }
            : s
        )
      );
    });

    connection.start().catch((err) => console.error("[SignalR Index] Error:", err));

    return () => {
      connection.stop().catch(() => {});
    };
  }, []);

  const handlePaginaAnterior = () => {
    if (pagina > 1) {
      setPagina(prev => prev - 1);
      window.scrollTo({ top: 0, behavior: 'smooth' });
    }
  };

  const handlePaginaSiguiente = () => {
    if (hasMore) {
      setPagina(prev => prev + 1);
      window.scrollTo({ top: 0, behavior: 'smooth' });
    }
  };

  // Color dinámico según el estado seleccionado para el indicador visual
  const getEstadoColorClass = () => {
    if (estado === 'ACTIVA') return 'dot-activa';
    if (estado === 'FINALIZADA') return 'dot-finalizada';
    if (estado === 'CANCELADA') return 'dot-cancelada';
    return 'dot-todos';
  };

  return (
    <div className="dashboard-container-dinamico">
      <Navbar billetera={billetera} usuario={usuario} />

      {/* CONTENEDOR DE CONTROLES FIJO Y DINÁMICO */}
      <div className="sticky-filters-container">
        <div className="filters-wrapper">
          
          {/* Categorías */}
          <div className="category-filters">
            {CATEGORIAS.map((cat) => (
              <button
                key={cat.id}
                className={`filter-pill ${categoria === cat.id ? 'active' : ''}`}
                onClick={() => { setCategoria(cat.id); setPagina(1); }}
              >
                {cat.label}
              </button>
            ))}
          </div>

          <div className="filter-divider"></div>

          {/* Selectores con indicador de color dinámico */}
          <div className="select-filters">
            <div className={`select-wrapper indicator-${estado.toLowerCase() || 'todos'}`}>
              <span className={`status-dot ${getEstadoColorClass()}`}></span>
              <select
                className="modern-select"
                value={estado}
                onChange={(e) => { setEstado(e.target.value); setPagina(1); }}
              >
                <option value="">🌐 Todos los estados</option>
                <option value="ACTIVA">🟢 Activas</option>
                <option value="FINALIZADA">🔴 Finalizadas</option>
                <option value="CANCELADA">⚪ Canceladas</option>
              </select>
            </div>

            <div className="select-wrapper">
              <span className="select-icon">⏳</span>
              <select
                className="modern-select"
                value={ordenar}
                onChange={(e) => { setOrdenar(e.target.value); setPagina(1); }}
              >
                <option value="Fecha">Próximas a finalizar</option>
                <option value="Precio">Mayor precio</option>
              </select>
            </div>
          </div>

        </div>
      </div>

      <main className="auctions-main">
        <div className="header-section-top">
          <h2 className="section-title">
            {busqueda ? `Resultados para "${busqueda}"` : (categoria ? `Subastas en ${categoria}` : 'Explorar Subastas')}
          </h2>

          {/* PAGINACIÓN SUPERIOR VISIBLE Y CÓMODA */}
          <div className="pagination-top-container">
            <button
              className="pagination-btn-top"
              onClick={handlePaginaAnterior}
              disabled={pagina === 1 || loading}
            >
              ← Anterior
            </button>
            <span className="pagination-info-top">
              Página <strong>{pagina}</strong>
            </span>
            <button
              className="pagination-btn-top"
              onClick={handlePaginaSiguiente}
              disabled={!hasMore || loading}
            >
              Siguiente →
            </button>
          </div>
        </div>

        {loading ? (
          <div className="loading-spinner">Cargando subastas...</div>
        ) : (
          <div className="auctions-grid">
            {subastas.length === 0 ? (
              <p className="no-auctions-text">No hay subastas disponibles en este momento.</p>
            ) : (
              subastas.map((subasta) => (
                <AuctionCard key={subasta.id} subasta={subasta} />
              ))
            )}
          </div>
        )}
      </main>
    </div>
  );
}