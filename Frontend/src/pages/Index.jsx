import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { appsettings } from '../settings/appsettings';
import Navbar from '../components/Navbar';
import AuctionCard from '../components/AuctionCard';
import '../styles/Index.css';

export default function Index() {
  const navigate = useNavigate();
  const [usuario, setUsuario] = useState(null);
  const [billetera, setBilletera] = useState(null);
  const [subastas, setSubastas] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const userStorage = localStorage.getItem("usuario");
    if (!userStorage) {
      navigate('/login');
      return;
    }
    
    const userData = JSON.parse(userStorage);
    setUsuario(userData);

    const cargarDatosDashboard = async () => {
      try {
        const [resBilletera, resSubastas] = await Promise.all([
          fetch(`${appsettings.apiUrl}wallets/${userData.id}/balance`),
          fetch(`${appsettings.apiUrl}auctions?estado=ACTIVA`)
        ]);

        if (resBilletera.ok) setBilletera(await resBilletera.json());
        if (resSubastas.ok) setSubastas(await resSubastas.json());
      } catch (error) {
        console.error("Error cargando el dashboard:", error);
      } finally {
        setLoading(false);
      }
    };

    cargarDatosDashboard();
  }, [navigate]);

  if (loading) return <div className="loading-spinner">Cargando subastas...</div>;

  return (
    <div className="dashboard-container">
      <Navbar billetera={billetera} usuario={usuario} />

      <div className="category-filters">
        <button className="filter-pill">💻 Tecnología</button>
        <button className="filter-pill">🚗 Vehículos</button>
        <button className="filter-pill">👕 Indumentaria</button>
        <button className="filter-pill">🏺 Coleccionables</button>
      </div>

      <main className="auctions-main">
        <h2 className="section-title">Próximas a finalizar</h2>
        <div className="auctions-grid">
          {subastas.length === 0 ? (
            <p>No hay subastas activas en este momento.</p>
          ) : (
            subastas.map((subasta) => (
              
              <AuctionCard key={subasta.id} subasta={subasta} />
            ))
          )}
        </div>
      </main>
    </div>
  );
}