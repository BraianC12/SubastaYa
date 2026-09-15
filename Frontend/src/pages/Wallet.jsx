import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import Navbar from '../components/Navbar';
import { appsettings } from '../settings/appsettings';
import '../styles/Wallet.css';

export default function Wallet() {

  const navigate = useNavigate();
  const [usuario, setUsuario] = useState(null);
  const [billetera, setBilletera] = useState({
    saldo_Total: 0,
    saldo_Retenido: 0,
    saldo_Disponible: 0
  });

  const [montoRecarga, setMontoRecarga] = useState('');
  const [metodoPago, setMetodoPago] = useState('TRANSFERENCIA');
  const [movimientos, setMovimientos] = useState([]);
  const [filtroTipo, setFiltroTipo] = useState('TODOS');
  const [cargando, setCargando] = useState(true);
  const [procesandoRecarga, setProcesandoRecarga] = useState(false);
  const [mensajeExito, setMensajeExito] = useState('');

  const montosRapidos = [10000, 25000, 50000, 100000];

  useEffect(() => {
    const userStorage = localStorage.getItem('usuario');
    if (!userStorage) {
      navigate('/login');
      return;
    }
    const userData = JSON.parse(userStorage);
    setUsuario(userData);
    cargarDatosBilletera(userData.id);
  }, [navigate]);

  const cargarDatosBilletera = async (usuarioId) => {
    try {
      setCargando(true);
      const resBalance = await fetch(`${appsettings.apiUrl}wallets/${usuarioId}/balance`);
      if (resBalance.ok) {
        const data = await resBalance.json();
        setBilletera(data);

        // Cargar historial de transacciones con el id de la billetera
        const billeteraId = data.id || usuarioId;
        try {
          const resMov = await fetch(`${appsettings.apiUrl}wallets/${billeteraId}/transactions`);
          if (resMov.ok) {
            const transaccionesData = await resMov.json();
            setMovimientos(Array.isArray(transaccionesData) ? transaccionesData : []);
          } else {
            setMovimientos([]);
          }
        } catch {
          setMovimientos([]);
        }
      }
    } catch (error) {
      console.error('Error al cargar billetera:', error);
    } finally {
      setCargando(false);
    }
  };

  const handleRecargar = async (e) => {
    e.preventDefault();
    const montoNum = parseFloat(montoRecarga);
    if (!montoNum || montoNum <= 0) return;

    try {
      setProcesandoRecarga(true);
      const response = await fetch(`${appsettings.apiUrl}wallets/deposit`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          usuario_Id: usuario.id,
          monto: montoNum
        })
      });

      if (response.ok) {
        setMensajeExito(`¡Se acreditaron $${montoNum.toLocaleString('es-AR')} exitosamente!`);
        setMontoRecarga('');
        await cargarDatosBilletera(usuario.id);
        setTimeout(() => setMensajeExito(''), 4000);
      }
    } catch (error) {
      console.error('Error en depósito:', error);
    } finally {
      setProcesandoRecarga(false);
    }
  };

  const renderTipoInfo = (tipo, subastaId) => {
    switch (tipo?.toUpperCase()) {
      case 'DEPOSITO':
        return {
          badgeClass: 'badge-deposito',
          badgeText: subastaId ? '🟢 Cobro Venta' : '🟢 Depósito',
          concepto: subastaId ? 'Ingreso por subasta adjudicada' : 'Acreditación de saldo simulado',
          montoSign: '+',
          amountClass: 'amount-positive',
          statusClass: 'status-completed',
          statusText: 'Acreditado'
        };
      case 'VENTA':
        return {
          badgeClass: 'badge-deposito',
          badgeText: '🟢 Venta',
          concepto: 'Cobro por subasta adjudicada',
          montoSign: '+',
          amountClass: 'amount-positive',
          statusClass: 'status-completed',
          statusText: 'Acreditado'
        };
      case 'RETENCION':
        return {
          badgeClass: 'badge-retencion',
          badgeText: '🟡 Retención',
          concepto: 'Garantía por puja líder',
          montoSign: '-',
          amountClass: 'amount-held',
          statusClass: 'status-holding',
          statusText: 'En Garantía'
        };
      case 'LIBERACION':
        return {
          badgeClass: 'badge-liberacion',
          badgeText: '🔵 Liberación',
          concepto: 'Reintegro por puja superada',
          montoSign: '+',
          amountClass: 'amount-released',
          statusClass: 'status-released',
          statusText: 'Liberado'
        };
      case 'DEBITO_FINAL':
      case 'DEBITO':
      case 'COMPRA':
        return {
          badgeClass: 'badge-debito',
          badgeText: '🔴 Débito',
          concepto: 'Pago por subasta adjudicada',
          montoSign: '-',
          amountClass: 'amount-debited',
          statusClass: 'status-debited',
          statusText: 'Debitado'
        };
      default:
        return {
          badgeClass: 'badge-neutral',
          badgeText: tipo || 'Movimiento',
          concepto: 'Operación de billetera',
          montoSign: '',
          amountClass: '',
          statusClass: 'status-neutral',
          statusText: 'Completado'
        };
    }
  };

  const formatearFecha = (fechaStr) => {
    if (!fechaStr) return '-';
    const d = new Date(fechaStr);
    return d.toLocaleString('es-AR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  };

  const movimientosFiltrados = movimientos.filter((m) => {
    const tipo = m.tipo?.toUpperCase() || '';
    if (filtroTipo === 'DEPOSITOS') return tipo === 'DEPOSITO' || tipo === 'VENTA';
    if (filtroTipo === 'RETENCIONES') return tipo === 'RETENCION';
    if (filtroTipo === 'LIBERACIONES') return tipo === 'LIBERACION';
    if (filtroTipo === 'DÉBITOS') return tipo.startsWith('DEBIT') || tipo === 'COMPRA';
    return true;
  });

  const porcentajeRetenido = billetera.saldo_Total > 0
    ? (billetera.saldo_Retenido / billetera.saldo_Total) * 100
    : 0;

  return (
    <div className="dashboard-container">
      <Navbar billetera={billetera} usuario={usuario} />

      <div className="wallet-wrapper">
        <div className="wallet-nav-back">
          <button className="back-link-btn" onClick={() => navigate('/index')}>
            ← Volver al catálogo principal
          </button>
        </div>

        <header className="wallet-header">
          <div>
            <h1 className="wallet-title">Billetera Virtual</h1>
            <p className="wallet-subtitle">
              Gestiona tus fondos, recarga saldo simulado y consulta el detalle de tus garantías en subastas.
            </p>
          </div>
        </header>

        {/* PANEL DE SALDOS */}
        <section className="wallet-kpi-grid">
          <div className="wallet-card card-disponible">
            <div className="card-top">
              <span className="card-label">Saldo Disponible</span>
              <span className="badge badge-success">Listo para pujar</span>
            </div>
            <div className="card-amount">
              ${billetera.saldo_Disponible?.toLocaleString('es-AR')}
            </div>
            <p className="card-hint">Total menos fondos retenidos en garantías</p>
          </div>

          <div className="wallet-card card-retenido">
            <div className="card-top">
              <span className="card-label">Saldo Retenido / Garantía</span>
              <span className="badge badge-warning">En Subastas</span>
            </div>
            <div className="card-amount">
              ${billetera.saldo_Retenido?.toLocaleString('es-AR')}
            </div>
            <p className="card-hint">Bloqueado mientras seas el postor líder</p>
          </div>

          <div className="wallet-card card-total">
            <div className="card-top">
              <span className="card-label">Saldo Total</span>
              <span className="badge badge-neutral">Fondos Totales</span>
            </div>
            <div className="card-amount">
              ${billetera.saldo_Total?.toLocaleString('es-AR')}
            </div>
            <p className="card-hint">Patrimonio total depositado en la cuenta</p>
          </div>
        </section>

        {/* Barra de proporción de saldo */}
        {billetera.saldo_Total > 0 && (
          <div className="balance-progress-container">
            <div className="balance-progress-bar">
              <div
                className="progress-segment segment-disponible"
                style={{ width: `${100 - porcentajeRetenido}%` }}
                title={`Disponible: ${(100 - porcentajeRetenido).toFixed(1)}%`}
              />
              <div
                className="progress-segment segment-retenido"
                style={{ width: `${porcentajeRetenido}%` }}
                title={`Retenido en garantía: ${porcentajeRetenido.toFixed(1)}%`}
              />
            </div>
            <div className="progress-labels">
              <span>Disponible: {(100 - porcentajeRetenido).toFixed(0)}%</span>
              <span>Retenido en garantía: {porcentajeRetenido.toFixed(0)}%</span>
            </div>
          </div>
        )}

        {/* FORMULARIO DE CARGA SIMULADA */}
        <section className="wallet-deposit-section">
          <div className="deposit-card">
            <div className="deposit-header">
              <div className="deposit-icon">💳</div>
              <div>
                <h2>Carga de Saldo Simulada</h2>
                <p>Acredita dinero de prueba ficticio para comenzar a ofertar en subastas.</p>
              </div>
            </div>

            {mensajeExito && <div className="alert-success-banner">{mensajeExito}</div>}

            <form onSubmit={handleRecargar} className="deposit-form">
              <label className="form-label">Seleccionar un monto rápido:</label>
              <div className="quick-amounts">
                {montosRapidos.map((monto) => (
                  <button
                    key={monto}
                    type="button"
                    className={`quick-amount-btn ${montoRecarga === monto.toString() ? 'active' : ''}`}
                    onClick={() => setMontoRecarga(monto.toString())}
                  >
                    +${monto.toLocaleString('es-AR')}
                  </button>
                ))}
              </div>

              <div className="custom-input-group">
                <label className="form-label">O ingresa un monto personalizado:</label>
                <div className="input-prefix-wrapper">
                  <span className="currency-prefix">$</span>
                  <input
                    type="number"
                    min="100"
                    placeholder="Ej: 50000"
                    className="app-input deposit-input"
                    value={montoRecarga}
                    onChange={(e) => setMontoRecarga(e.target.value)}
                    required
                  />
                </div>
              </div>

              <div className="form-actions">
                <button
                  type="submit"
                  className="app-btn-primary btn-deposit"
                  disabled={procesandoRecarga || !montoRecarga || parseFloat(montoRecarga) <= 0}
                >
                  {procesandoRecarga ? 'Acreditando...' : '⚡ Acreditar Saldo Simulado'}
                </button>
              </div>
            </form>
          </div>
        </section>

        {/* HISTORIAL DE MOVIMIENTOS */}
        <section className="wallet-history-section">
          <div className="history-header">
            <h2>Historial de Movimientos</h2>
            <div className="filter-chips">
              {['TODOS', 'DEPOSITOS', 'RETENCIONES', 'LIBERACIONES', 'DÉBITOS'].map(f => (
                <button
                  key={f}
                  className={`filter-chip ${filtroTipo === f ? 'active' : ''}`}
                  onClick={() => setFiltroTipo(f)}
                >
                  {f}
                </button>
              ))}
            </div>
          </div>

          <div className="table-responsive">
            <table className="transactions-table">
              <thead>
                <tr>
                  <th>Fecha & Hora</th>
                  <th>Tipo / Concepto</th>
                  <th>Subasta Ref.</th>
                  <th>Monto</th>
                  <th>Estado</th>
                </tr>
              </thead>
              <tbody>
                {movimientosFiltrados.length === 0 ? (
                  <tr>
                    <td colSpan="5" className="empty-history-cell">
                      No se encontraron movimientos registrados en esta billetera.
                    </td>
                  </tr>
                ) : (
                  movimientosFiltrados.map((mov) => {
                    const info = renderTipoInfo(mov.tipo, mov.subasta_Id);
                    return (
                      <tr key={mov.id}>
                        <td>{formatearFecha(mov.fecha)}</td>
                        <td>
                          <div className="tx-concept">
                            <span className={`tx-badge ${info.badgeClass}`}>{info.badgeText}</span>
                            <span className="tx-desc">{info.concepto}</span>
                          </div>
                        </td>
                        <td>
                          {mov.subasta_Id ? (
                            <button
                              type="button"
                              className="subasta-link-btn"
                              onClick={() => navigate(`/subasta/${mov.subasta_Id}`)}
                            >
                              #{mov.subasta_Id} {mov.subasta_Titulo || 'Subasta'}
                            </button>
                          ) : (
                            <span className="text-muted">-</span>
                          )}
                        </td>
                        <td className={info.amountClass}>
                          {info.montoSign}${mov.monto?.toLocaleString('es-AR')}
                        </td>
                        <td>
                          <span className={`status-pill ${info.statusClass}`}>{info.statusText}</span>
                        </td>
                      </tr>
                    );
                  })
                )}
              </tbody>
            </table>
          </div>
        </section>
      </div>
    </div>
  );
}