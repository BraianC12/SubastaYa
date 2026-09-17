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
  const [ordenar, setOrdenar] = useState('Fecha'); // 'Fecha' o 'Precio'
  const LIMITE_POR_PAGINA = 3;

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
        // Construcción dinámica de parámetros de búsqueda
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

  // Conexión WebSockets en tiempo real para actualizar cards en el catálogo
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

  const handleCategoriaChange = (nuevaCategoria) => {
    setCategoria(nuevaCategoria);
    setPagina(1);
  };

  const handleEstadoChange = (e) => {
    setEstado(e.target.value);
    setPagina(1);
  };

  const handleOrdenarChange = (e) => {
    setOrdenar(e.target.value);
    setPagina(1);
  };

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

  return (
    <div className="dashboard-container">
      <Navbar billetera={billetera} usuario={usuario} />

      <div className="filters-section">
        {/* Filtro de Categorías */}
        <div className="category-filters">
          {CATEGORIAS.map((cat) => (
            <button
              key={cat.id}
              className={`filter-pill ${categoria === cat.id ? 'active' : ''}`}
              onClick={() => handleCategoriaChange(cat.id)}
            >
              {cat.label}
            </button>
          ))}
        </div>
        {/* Filtros desplegables: Estado y Criterio de Orden */}
        <div className="select-filters">
          <div className="filter-group">
            <label htmlFor="select-estado">Estado:</label>
            <select
              id="select-estado"
              className="filter-select"
              value={estado}
              onChange={handleEstadoChange}
            >
              <option value="">Todos los estados</option>
              <option value="ACTIVA">🟢 Activas</option>
              <option value="FINALIZADA">🔴 Finalizadas</option>
              <option value="CANCELADA">⚪ Canceladas</option>
            </select>
          </div>
          <div className="filter-group">
            <label htmlFor="select-orden">Ordenar por:</label>
            <select
              id="select-orden"
              className="filter-select"
              value={ordenar}
              onChange={handleOrdenarChange}
            >
              <option value="Fecha">⏳ Próximas a finalizar</option>
              <option value="Precio">💰 Mayor precio</option>
            </select>
          </div>
        </div>
      </div>
      <main className="auctions-main">
        <h2 className="section-title">
          {busqueda ? `Resultados para "${busqueda}"` : (categoria ? `Subastas en ${categoria}` : 'Explorar Subastas')}
        </h2>

        {loading ? (
          <div className="loading-spinner">Cargando subastas...</div>
        ) : (
          <>
            <div className="auctions-grid">
              {subastas.length === 0 ? (
                <p>No hay subastas activas en este momento.</p>
              ) : (
                subastas.map((subasta) => (
                  <AuctionCard key={subasta.id} subasta={subasta} />
                ))
              )}
            </div>

            <div className="pagination-container">
              <button
                className="pagination-btn"
                onClick={handlePaginaAnterior}
                disabled={pagina === 1 || loading}
              >
                ← Anterior
              </button>
              <span className="pagination-info">
                Página <strong>{pagina}</strong>
              </span>
              <button
                className="pagination-btn"
                onClick={handlePaginaSiguiente}
                disabled={!hasMore || loading}
              >
                Siguiente →
              </button>
            </div>
          </>
        )}
      </main>
    </div>
  );
}