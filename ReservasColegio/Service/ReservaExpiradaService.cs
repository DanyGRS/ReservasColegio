using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReservasColegio.Context;
using ReservasColegio.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ReservasColegio.Services
{
    public class ReservaExpiradaService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ReservaExpiradaService> _logger;
        private readonly TimeSpan _intervalo = TimeSpan.FromMinutes(30); 
        public ReservaExpiradaService(IServiceProvider serviceProvider, ILogger<ReservaExpiradaService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var agora = DateTime.Now;

                    var reservasParaExpirar = await db.Reservas
                        .Where(r => r.Status == StatusReserva.Pendente || r.Status == StatusReserva.Aprovada)
                        .ToListAsync();

                    reservasParaExpirar = reservasParaExpirar
                        .Where(r => r.DataReserva.Add(r.HoraFim.TimeOfDay) < agora)
                        .ToList();


                    foreach (var reserva in reservasParaExpirar)
                    {
                        reserva.Status = StatusReserva.Expirada;
                        _logger.LogInformation($"Reserva #{reserva.Id} marcada como Expirada.");
                    }

                    if (reservasParaExpirar.Any())
                    {
                        await db.SaveChangesAsync();
                    }
                }

                await Task.Delay(_intervalo, stoppingToken);
            }
        }
    }
}
