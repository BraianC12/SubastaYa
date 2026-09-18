import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import '../styles/AuctionCard.css';

export default function AuctionCard({ subasta }) {
  const navigate = useNavigate();
  const [tiempoRestante, setTiempoRestante] = useState('');
  const [esCritico, setEsCritico] = useState(false);

  useEffect(() => {
    if (!subasta || !subasta.fecha_Fin) return;

    const actualizarContador = () => {
      const ahora = new Date().getTime();

      if (subasta.estado === 'FINALIZADA' || subasta.estado === 'DESIERTA' || subasta.estado === 'CANCELADA') {
        setTiempoRestante("Finalizada");
        setEsCritico(false);
        return;
      }

      if (subasta.estado === 'PROGRAMADA' && subasta.fecha_Inicio) {
        const fechaInicio = new Date(subasta.fecha_Inicio).getTime();
        const diferenciaInicio = fechaInicio - ahora;

        if (diferenciaInicio > 0) {
          setEsCritico(false);
          const totalSegundos = Math.floor(diferenciaInicio / 1000);
          const totalMinutos = Math.floor(totalSegundos / 60);
          const totalHoras = Math.floor(totalMinutos / 60);
          const dias = Math.floor(totalHoras / 24);
          const horas = totalHoras % 24;
          const minutos = totalMinutos % 60;
          const segundos = totalSegundos % 60;

          if (dias > 0) {
            setTiempoRestante(`Inicia en: ${dias}d ${horas}h ${minutos}m ${segundos}s`);
          } else if (horas > 0) {
            setTiempoRestante(`Inicia en: ${horas}h ${minutos}m ${segundos}s`);
          } else {
            setTiempoRestante(`Inicia en: ${minutos}m ${segundos}s`);
          }
          return;
        }
      }

      const fechaFin = new Date(subasta.fecha_Fin).getTime();
      const diferencia = fechaFin - ahora;

      if (diferencia <= 0) {
        setTiempoRestante("Finalizada");
        setEsCritico(false);
        return;
      }
      
      // Si falta 1 minuto (60.000 ms) o menos, marcamos como crítico (rojo)
      setEsCritico(diferencia <= 60000);

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

  // Determinamos qué clase CSS aplicar a la pastilla del tiempo
  const badgeClass = esCritico ? 'urgent' : (tiempoRestante === 'Finalizada' ? 'finished' : 'active');
  const icon = tiempoRestante === 'Finalizada' ? '🏁' : '⏳';

  return (
    <div className="auction-card" onClick={() => navigate(`/subasta/${subasta.id}`)}>
      <div className="card-image-container">
        <img 
          src={subasta.url_Imagen}
          alt={subasta.titulo}
          className="card-image"
          onError={(e) => { e.target.style.display = 'none'; }}
        />
      </div>
      <div className="card-content">
        <h3 className="card-title">{subasta.titulo}</h3>
        <span className="card-category">{subasta.categoria}</span>
        
        {/* NUEVO FOOTER MODERNO */}
        <div className="card-footer-modern">
          <div className="card-price-wrapper">
            <span className="price-value">
              ${subasta.puja_Actual?.toLocaleString('es-AR') || subasta.precio_Base.toLocaleString('es-AR')}
            </span>
          </div>

          <div className={`card-time-badge ${badgeClass}`}>
            <span className="time-icon">{icon}</span>
            <span className="time-text">{tiempoRestante || "Calculando..."}</span>
          </div>
        </div>
        
      </div>
    </div>
  );
}