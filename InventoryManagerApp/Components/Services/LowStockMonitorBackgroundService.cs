
namespace InventoryManagerApp.Components.Services
{
    /* LowStockMonitorService extends BackgroundService   *
     *     to perform the Task execution of the the       *
     * monitoring service for the low stock notifications */
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
