using Application.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SubastaYa.Workers
{
    public class AdjudicacionWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AdjudicacionWorker> _logger;

        public AdjudicacionWorker(IServiceProvider serviceProvider, ILogger<AdjudicacionWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Iniciando el Worker de Adjudicación de Subastas");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using(var scope = _serviceProvider.CreateScope())
                    {
                        var adjudicarHandler = scope.ServiceProvider.GetRequiredService<IAdjudicarSubastasCommandHandler>();
                        await adjudicarHandler.Handle();
                    }

                    _logger.LogInformation($"[{DateTime.Now}] worker crequeo subastas exitosamente");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ocurrio un error en el worker de Adjudicación: {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}
