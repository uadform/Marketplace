using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.BackgroundServices
{
    public class UnpaidOrderCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public UnpaidOrderCleanupService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
                    await orderService.DeleteUnpaidOrdersAsync();
                }
                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
                await Console.Out.WriteLineAsync("Deleted unpaid orders");
            }
        }
    }
}
