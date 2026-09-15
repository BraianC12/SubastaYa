import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import Navbar from '../components/Navbar';
import { appsettings } from '../settings/appsettings';
import '../styles/MyActivities.css';

export default function MyActivities() {
  const navigate = useNavigate();
  const [usuario, setUsuario] = useState(null);
  const [billetera, setBilletera] = useState(null);

  const [tabActiva, setTabActiva] = useState('pujas'); // 'pujas' | 'publicaciones'
  const [filtroPujas, setFiltroPujas] = useState('TODAS');
  const [filtroPublicaciones, setFiltroPublicaciones] = useState('TODAS');

  const [misPujas, setMisPujas] = useState([]);
  const [misPublicaciones, setMisPublicaciones] = useState([]);
  const [cargando, setCargando] = useState(true);

  useEffect(() => {
    const userStorage = localStorage.getItem('usuario');
    if (!userStorage) {
      navigate('/login');
      return;
    }
    const userData = JSON.parse(userStorage);
    setUsuario(userData);
    cargarDatos(userData.id);
  }, [navigate]);

  const cargarDatos = async (usuarioId) => {
    try {
      setCargando(true);

      // Cargar saldo de billetera para el Navbar
      try {
        const resBalance = await fetch(`${appsettings.apiUrl}wallets/${usuarioId}/balance`);
        if (resBalance.ok) {
          const balanceData = await resBalance.json();
          setBilletera(balanceData);
        }
      } catch (err) {
        console.error('Error cargando balance:', err);
      }

      // Cargar mis compras / pujas
      try {
        const resPujas = await fetch(`${appsettings.apiUrl}auctions/${usuarioId}/buyer`);
        if (resPujas.ok) {
          const dataPujas = await resPujas.json();
          setMisPujas(Array.isArray(dataPujas) ? dataPujas : []);
        }
      } catch (err) {
        console.error('Error cargando pujas:', err);
      }

      // Cargar mis publicaciones
      try {
        const resPubs = await fetch(`${appsettings.apiUrl}auctions/${usuarioId}/seller`);
        if (resPubs.ok) {
          const dataPubs = await resPubs.json();
          setMisPublicaciones(Array.isArray(dataPubs) ? dataPubs : []);
        }
      } catch (err) {
        console.error('Error cargando publicaciones:', err);
      }
    } finally {
      setCargando(false);
    }
  };

  // Cálculo de Métricas del Vendedor
  const metricasVendedor = {
    totalRecaudado: misPublicaciones
      .filter((p) => p.estado === 'FINALIZADA' && p.pujaMasAlta)
      .reduce((acc, curr) => acc + Number(curr.pujaMasAlta || 0), 0),
    ofertasEnJuego: misPublicaciones
      .filter((p) => p.estado === 'ACTIVA' && p.pujaMasAlta)
      .reduce((acc, curr) => acc + Number(curr.pujaMasAlta || 0), 0),
    activas: misPublicaciones.filter((p) => p.estado === 'ACTIVA').length,
    programadas: misPublicaciones.filter((p) => p.estado === 'PROGRAMADA').length,
    adjudicadas: misPublicaciones.filter((p) => p.estado === 'FINALIZADA' && p.ganadorId).length,
  };

  // Filtrado de Mis Pujas
  const pujasFiltradas = misPujas.filter((item) => {
    const estado = item.estado?.toUpperCase();
    const tuPuja = Number(item.tuPuja ?? item.tu_Puja ?? 0);
    const liderPuja = Number(item.pujaLiderActual ?? 0);
    const esLider = item.esLider || (estado === 'ACTIVA' && tuPuja > 0 && tuPuja >= liderPuja);
    const esGanador = item.esGanador || (estado === 'FINALIZADA' && tuPuja > 0 && tuPuja >= liderPuja);

    if (filtroPujas === 'TODAS') return true;
    if (filtroPujas === 'GANANDO') return estado === 'ACTIVA' && esLider;
    if (filtroPujas === 'SUPERADAS') return estado === 'ACTIVA' && !esLider;
    if (filtroPujas === 'GANADAS') return esGanador;
    if (filtroPujas === 'FINALIZADAS') return estado === 'FINALIZADA' || estado === 'DESIERTA';
    return true;
  });

  // Filtrado de Mis Publicaciones
  const publicacionesFiltradas = misPublicaciones.filter((item) => {
    const estado = item.estado?.toUpperCase();
    if (filtroPublicaciones === 'TODAS') return true;
    if (filtroPublicaciones === 'ACTIVAS') return estado === 'ACTIVA';
    if (filtroPublicaciones === 'PROGRAMADAS') return estado === 'PROGRAMADA';
    if (filtroPublicaciones === 'FINALIZADAS') return estado === 'FINALIZADA' && item.ganadorId;
    if (filtroPublicaciones === 'DESIERTAS') return estado === 'DESIERTA' || (estado === 'FINALIZADA' && !item.ganadorId);
    return true;
  });

  const formatearFecha = (fechaStr) => {
    if (!fechaStr) return '';
    const fecha = new Date(fechaStr);
    return fecha.toLocaleDateString('es-AR', {
      day: '2-digit',
      month: 'short',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  return (
    <div className="dashboard-container">
      <Navbar billetera={billetera} />

      <main className="activities-wrapper">
        <div className="activities-nav-back">
          <button className="btn-back" onClick={() => navigate('/index')}>
            ← Volver a Subastas
          </button>
        </div>

        <header className="activities-header">
          <div>
            <h1 className="activities-title">Mis Actividades</h1>
            <p className="activities-subtitle">
              Hola, <strong>{usuario?.nombre || 'Usuario'}</strong>. Consulta tus pujas, productos ganados y gestiona tus ventas.
            </p>
          </div>
        </header>

        {/* SELECTOR DE PESTAÑAS */}
        <nav className="activities-tabs-container" aria-label="Pestañas de actividad">
          <button
            className={`activity-tab-btn ${tabActiva === 'pujas' ? 'active' : ''}`}
            onClick={() => setTabActiva('pujas')}
          >
            🏷️ Mis Pujas <span className="tab-badge">{misPujas.length}</span>
          </button>
          <button
            className={`activity-tab-btn ${tabActiva === 'publicaciones' ? 'active' : ''}`}
            onClick={() => setTabActiva('publicaciones')}
          >
            📦 Mis Publicaciones <span className="tab-badge">{misPublicaciones.length}</span>
          </button>
        </nav>

        {cargando ? (
          <div className="activities-loading">
            <div className="spinner"></div>
            <p>Cargando tus actividades...</p>
          </div>
        ) : (
          <>
            {tabActiva === 'pujas' && (
              <section>
                {/* BARRA DE FILTROS */}
                <div className="filters-actions-bar">
                  <div className="pills-group">
                    {['TODAS', 'GANANDO', 'SUPERADAS', 'GANADAS', 'FINALIZADAS'].map((filtro) => (
                      <button
                        key={filtro}
                        className={`filter-pill ${filtroPujas === filtro ? 'active' : ''}`}
                        onClick={() => setFiltroPujas(filtro)}
                      >
                        {filtro === 'GANANDO' && '🟢 '}
                        {filtro === 'SUPERADAS' && '⚠️ '}
                        {filtro === 'GANADAS' && '🏆 '}
                        {filtro}
                      </button>
                    ))}
                  </div>
                </div>

                {/* LISTADO DE PUJAS */}
                {pujasFiltradas.length === 0 ? (
                  <div className="activities-empty">
                    <div className="empty-icon">🏷️</div>
                    <h3 className="empty-title">No hay subastas en esta sección</h3>
                    <p className="empty-desc">
                      {filtroPujas === 'TODAS'
                        ? 'Aún no has participado en ninguna subasta.'
                        : `No tienes pujas con el filtro "${filtroPujas}".`}
                    </p>
                    <button className="app-btn" onClick={() => navigate('/index')}>
                      Explorar Subastas Activas
                    </button>
                  </div>
                ) : (
                  <div className="activities-list">
                    {pujasFiltradas.map((item) => {
                      const estado = item.estado?.toUpperCase();
                      const estaActiva = estado === 'ACTIVA';
                      const tuPuja = Number(item.tuPuja ?? item.tu_Puja ?? 0);
                      const liderPuja = Number(item.pujaLiderActual ?? 0);
                      const esLider = item.esLider || (estaActiva && tuPuja > 0 && tuPuja >= liderPuja);
                      const esGanador = item.esGanador || (estado === 'FINALIZADA' && tuPuja > 0 && tuPuja >= liderPuja);
                      const esSuperado = estaActiva && !esLider;

                      return (
                        <article key={item.id} className="activity-card">
                          <div className="activity-card-left">
                            <div className="activity-img-wrapper">
                              <img
                                src={item.url_Imagen || 'https://via.placeholder.com/150'}
                                alt={item.titulo}
                                className="activity-thumb"
                                onError={(e) => {
                                  e.target.src = 'https://via.placeholder.com/150?text=Subasta';
                                }}
                              />
                            </div>
                            <div className="activity-details">
                              <span className="activity-category">{item.categoria || 'General'}</span>
                              <h3
                                className="activity-item-title"
                                onClick={() => navigate(`/subasta/${item.id}`)}
                                title={item.titulo}
                              >
                                {item.titulo}
                              </h3>
                              <div className="activity-meta">
                                <span>Cierre: {formatearFecha(item.fecha_Fin)}</span>
                              </div>

                              {/* Badges de Estado */}
                              <div>
                                {esGanador && <span className="badge badge-ganada">🏆 ¡Ganaste el producto!</span>}
                                {estaActiva && esLider && <span className="badge badge-ganando">🟢 Vas Ganando</span>}
                                {esSuperado && <span className="badge badge-superado">⚠️ Oferta Superada</span>}
                                {!estaActiva && !esGanador && (
                                  <span className="badge badge-perdida">⚪ Subasta Finalizada (Saldo liberado)</span>
                                )}
                              </div>
                            </div>
                          </div>

                          <div className="activity-card-right">
                            <div className="price-box">
                              <span className="price-label">Tu Puja</span>
                              <span className="price-value">${tuPuja.toLocaleString('es-AR')}</span>
                            </div>

                            <div className="price-box">
                              <span className="price-label">Puja Líder Actual</span>
                              <span className="price-value leader-price">
                                ${liderPuja.toLocaleString('es-AR')}
                              </span>
                            </div>

                            <div className="activity-actions">
                              {esSuperado ? (
                                <button
                                  className="btn-action-outbid"
                                  onClick={() => navigate(`/subasta/${item.id}`)}
                                >
                                  ⚡ Superar Puja
                                </button>
                              ) : (
                                <button
                                  className="btn-action-primary"
                                  onClick={() => navigate(`/subasta/${item.id}`)}
                                >
                                  {esGanador ? '🎉 Ver Compra' : 'Ver Subasta'}
                                </button>
                              )}
                            </div>
                          </div>
                        </article>
                      );
                    })}
                  </div>
                )}
              </section>
            )}

            {/* =========================================
                PESTAÑA 2: MIS PUBLICACIONES
               ========================================= */}
            {tabActiva === 'publicaciones' && (
              <section>
                {/* KPIS DE VENDEDOR */}
                <div className="seller-kpi-grid">
                  <div className="seller-kpi-card">
                    <div className="kpi-icon-wrap">💰</div>
                    <div className="kpi-data">
                      <span className="kpi-title">Total Recaudado</span>
                      <span className="kpi-amount text-success">
                        ${metricasVendedor.totalRecaudado.toLocaleString('es-AR')}
                      </span>
                    </div>
                  </div>

                  <div className="seller-kpi-card">
                    <div className="kpi-icon-wrap">⏳</div>
                    <div className="kpi-data">
                      <span className="kpi-title">Ofertas en Juego</span>
                      <span className="kpi-amount text-primary">
                        ${metricasVendedor.ofertasEnJuego.toLocaleString('es-AR')}
                      </span>
                    </div>
                  </div>

                  <div className="seller-kpi-card">
                    <div className="kpi-icon-wrap">📦</div>
                    <div className="kpi-data">
                      <span className="kpi-title">Subastas Activas</span>
                      <span className="kpi-amount">{metricasVendedor.activas}</span>
                    </div>
                  </div>

                  <div className="seller-kpi-card">
                    <div className="kpi-icon-wrap">⏰</div>
                    <div className="kpi-data">
                      <span className="kpi-title">Programadas</span>
                      <span className="kpi-amount">{metricasVendedor.programadas}</span>
                    </div>
                  </div>

                  <div className="seller-kpi-card">
                    <div className="kpi-icon-wrap">🏆</div>
                    <div className="kpi-data">
                      <span className="kpi-title">Adjudicadas</span>
                      <span className="kpi-amount">{metricasVendedor.adjudicadas}</span>
                    </div>
                  </div>
                </div>

                {/* BARRA DE FILTROS Y BOTÓN PUBLICAR */}
                <div className="filters-actions-bar">
                  <div className="pills-group">
                    {['TODAS', 'ACTIVAS', 'PROGRAMADAS', 'FINALIZADAS', 'DESIERTAS'].map((filtro) => (
                      <button
                        key={filtro}
                        className={`filter-pill ${filtroPublicaciones === filtro ? 'active' : ''}`}
                        onClick={() => setFiltroPublicaciones(filtro)}
                      >
                        {filtro}
                      </button>
                    ))}
                  </div>
                </div>

                {/* LISTADO DE PUBLICACIONES */}
                {publicacionesFiltradas.length === 0 ? (
                  <div className="activities-empty">
                    <div className="empty-icon">📦</div>
                    <h3 className="empty-title">No hay publicaciones</h3>
                    <p className="empty-desc">
                      {filtroPublicaciones === 'TODAS'
                        ? 'No has publicado ninguna subasta todavía.'
                        : `No tienes publicaciones con el filtro "${filtroPublicaciones}".`}
                    </p>
                    <button className="app-btn" onClick={() => navigate('/crear-subasta')}>
                      Crear mi Primera Subasta
                    </button>
                  </div>
                ) : (
                  <div className="activities-list">
                    {publicacionesFiltradas.map((item) => {
                      const estaActiva = item.estado === 'ACTIVA';
                      const estaAdjudicada = item.estado === 'FINALIZADA' && item.ganadorId;
                      const estaProgramada = item.estado === 'PROGRAMADA';
                      const estaDesierta = item.estado === 'DESIERTA' || (item.estado === 'FINALIZADA' && !item.ganadorId);

                      return (
                        <article key={item.id} className="activity-card">
                          <div className="activity-card-left">
                            <div className="activity-img-wrapper">
                              <img
                                src={item.url_Imagen || 'https://via.placeholder.com/150'}
                                alt={item.titulo}
                                className="activity-thumb"
                                onError={(e) => {
                                  e.target.src = 'https://via.placeholder.com/150?text=Subasta';
                                }}
                              />
                            </div>
                            <div className="activity-details">
                              <h3
                                className="activity-item-title"
                                onClick={() => navigate(`/subasta/${item.id}`)}
                                title={item.titulo}
                              >
                                {item.titulo}
                              </h3>
                              <div className="activity-meta">
                                <span>Inicio: {formatearFecha(item.fecha_Inicio)}</span>
                                <span>•</span>
                                <span>Fin: {formatearFecha(item.fecha_Fin)}</span>
                              </div>

                              {/* Badges de Estado y Adjudicación */}
                              <div>
                                {estaActiva && <span className="badge badge-activa">🔵 Activa (Recibiendo pujas)</span>}
                                {estaAdjudicada && (
                                  <span className="badge badge-adjudicada">
                                    🏆 Adjudicada a: {item.ganadorNombre || `Usuario #${item.ganadorId}`}
                                  </span>
                                )}
                                {estaProgramada && (
                                  <span className="badge badge-programada">
                                    ⏰ Programada (Inicia: {formatearFecha(item.fecha_Inicio)})
                                  </span>
                                )}
                                {estaDesierta && <span className="badge badge-desierta">⚪ Finalizada sin ofertas</span>}
                              </div>
                            </div>
                          </div>

                          <div className="activity-card-right">
                            <div className="price-box">
                              <span className="price-label">Precio Base</span>
                              <span className="price-value">${Number(item.precio_Base).toLocaleString('es-AR')}</span>
                            </div>

                            <div className="price-box">
                              <span className="price-label">
                                {estaAdjudicada ? 'Venta Final' : 'Mejor Oferta'} ({item.cantidadPujas} pujas)
                              </span>
                              <span className="price-value text-success">
                                $
                                {Number(
                                  item.montoVentaFinal || item.pujaMasAlta || item.precio_Base
                                ).toLocaleString('es-AR')}
                              </span>
                            </div>

                            <div className="activity-actions">
                              <button
                                className="btn-action-primary"
                                onClick={() => navigate(`/subasta/${item.id}`)}
                              >
                                Ver Subasta
                              </button>
                            </div>
                          </div>
                        </article>
                      );
                    })}
                  </div>
                )}
              </section>
            )}
          </>
        )}
      </main>
    </div>
  );
}
