using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

//url que recibe las pujas
string apiUrl = "https://localhost:7117/api/auctions/6/bids";

//configuracion de ataque
int usuariosSimultaneos = 1000;
Console.WriteLine($"Iniciando ataque: {usuariosSimultaneos} usuarios pujando al mismo milisegundo...");

using var httpClient = new HttpClient();
var tareas = new List<Task<HttpResponseMessage>>();
var cronometro = Stopwatch.StartNew();

//preparamos las peticiones
for (int i = 1; i <= usuariosSimultaneos; i++)
{
    // Simulamos que distintos usuarios (ID 1 al 10) intentan pujar $2000 a la Subasta 1
    string jsonPayload = $@"{{
        ""subasta_Id"": 6,
        ""comprador_Id"": 2,
        ""monto"": 50060.00
    }}";

    var contenido = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
    tareas.Add(httpClient.PostAsync(apiUrl, contenido));
}

// disparamos todas juntas
var resultados = await Task.WhenAll(tareas);
cronometro.Stop();

// evaluamos los resultados
int exitosas = resultados.Count(r => r.IsSuccessStatusCode);
int fallidas = resultados.Count(r => !r.IsSuccessStatusCode);

Console.WriteLine("\n=== RESULTADOS DE LA LÍNEA BASE ===");
Console.WriteLine($"Tiempo de respuesta del server: {cronometro.ElapsedMilliseconds} ms");
Console.WriteLine($"Pujas que el servidor aceptó (HTTP 200/201): {exitosas}");
Console.WriteLine($"Pujas que el servidor rechazó: {fallidas}");

if (exitosas > 1)
{
    Console.WriteLine("\n⚠️ ALERTA: Tu sistema actual permitió que entren múltiples pujas al mismo tiempo.");
    Console.WriteLine("Si revisás la base de datos, el último que sobreescribió el registro pisó a los demás.");
}
else if (exitosas == 1)
{
    Console.WriteLine("\n✅ INCREÍBLE: Entró solo 1. Tu sistema ya está manejando la concurrencia.");
}

// Obtenemos una de las peticiones fallidas para ver por qué rebotó
var falloDeEjemplo = resultados.FirstOrDefault(r => !r.IsSuccessStatusCode);
if (falloDeEjemplo != null)
{
    Console.WriteLine($"\n--- DETALLE DEL RECHAZO ---");
    Console.WriteLine($"Código HTTP: {(int)falloDeEjemplo.StatusCode} ({falloDeEjemplo.StatusCode})");
    string mensajeError = await falloDeEjemplo.Content.ReadAsStringAsync();
    Console.WriteLine($"Mensaje del backend: {mensajeError}");
}