import { useNavigate } from 'react-router-dom';
import '../styles/AuctionCard.css';

export default function AuctionCard({ subasta }) {
  const navigate = useNavigate();

  return (
    <div className="auction-card" onClick={() => navigate(`/subasta/${subasta.id}`)}>
      <div className="card-image-container">
        <img 
          src={subasta.url_Imagen}
          alt={subasta.titulo}
          className="card-image"
          //plan de contigencia: si la URL falla o esta vacía.
          onError={(e) => { e.target.style.display = 'none'; }}
        />
      </div>
      <div className="card-content">
        <h3 className="card-title">{subasta.titulo}</h3>
        <span className="card-category">{subasta.categoria}</span>
        <div className="card-footer">
          <span className="card-price">
            ${subasta.puja_Actual?.toLocaleString('es-AR') || subasta.precio_Base.toLocaleString('es-AR')}
          </span>
          <span className="card-time">⏳ Termina pronto</span>
        </div>
      </div>
    </div>
  );
}