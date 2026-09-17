import * as signalR from '@microsoft/signalr';
import { appsettings } from '../settings/appsettings';

/**
 * Crea una instancia de conexión con el Hub de SignalR de Subastas
 */
export const crearConexionSubastaHub = () => {
  return new signalR.HubConnectionBuilder()
    .withUrl(appsettings.hubUrl, {
      skipNegotiation: false,
      transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.LongPolling
    })
    .withAutomaticReconnect([0, 2000, 5000, 10000])
    .configureLogging(signalR.LogLevel.Warning)
    .build();
};

/**
 * Se une a la sala de una subasta específica
 */
export const unirseASalaSubasta = async (connection, subastaId) => {
  if (connection && connection.state === signalR.HubConnectionState.Connected) {
    try {
      await connection.invoke("UnirseSubasta", String(subastaId));
    } catch (err) {
      console.error(`[SignalR] Error al unirse a la sala de la subasta ${subastaId}:`, err);
    }
  }
};

/**
 * Sale de la sala de una subasta específica
 */
export const salirDeSalaSubasta = async (connection, subastaId) => {
  if (connection && connection.state === signalR.HubConnectionState.Connected) {
    try {
      await connection.invoke("SalirDeSubasta", String(subastaId));
    } catch (err) {
      console.error(`[SignalR] Error al salir de la sala de la subasta ${subastaId}:`, err);
    }
  }
};

