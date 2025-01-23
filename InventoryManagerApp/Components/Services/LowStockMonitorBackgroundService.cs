
namespace InventoryManagerApp.Components.Services
{
    public class LowStockMonitorBackgroundService : BackgroundService
    {
        private readonly LowStockMonitorService _monitorService;

        public LowStockMonitorBackgroundService(LowStockMonitorService monitorService)
        {
            _monitorService = monitorService ?? throw new ArgumentNullException(nameof(monitorService));
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return _monitorService.StartMonitoring(stoppingToken);
        }
    }
}
