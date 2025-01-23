using InventoryManagerApp.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace InventoryManagerApp.Components.Services
{
    public class LowStockMonitorService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private readonly ConcurrentDictionary<Guid, Item> _lowStockItems = new();
        private readonly NotificationsAVLTree _avl = new();
        private readonly ProcessLowStockNotifications _processItemNotifications;
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private DateTime _lastRunTime = DateTime.UtcNow.AddMinutes(-1);
        private readonly ILogger<LowStockMonitorService> _logger;

        // The quantity that should be considered low stock
        private const int LowStockThreshold = 10;

        // The delay between task runs
        private const int TaskDelay = 5000;

        // Expose items as an IEnumerable
        public IEnumerable<Item> LowStockItems => _lowStockItems.Values;

        /*       Initializes a new instance of the LowStockMonitorService class            *
         * PARAM = dbContextFactory : The factory for creating database contexts           *
         *       PARAM = logger : The logger for logging information and errors            */
        public LowStockMonitorService(IDbContextFactory<ApplicationDbContext> dbContextFactory,
                                      ILogger<LowStockMonitorService> logger)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            _processItemNotifications = new ProcessLowStockNotifications(_avl, _lowStockItems, LowStockThreshold);
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            LoadNotificationsFromDatabase();
        }

        private void LoadNotificationsFromDatabase()
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            var notifications = dbContext.LowStockNotifications.ToList();

            foreach (var notification in notifications)
            {
                var item = dbContext.Item.FirstOrDefault(i => i.Id == notification.ItemId);
                if (item != null)
                {
                    _avl.Insert(item);
                    _lowStockItems[item.Id] = item;
                }
            }
        }

        // Synchronize AVL with the database
        private async Task SyncDatabaseAsync()
        {
            using var context = _dbContextFactory.CreateDbContext();
            var existingNotifications = context.LowStockNotifications.ToList();

            // Add new notifications
            foreach (var item in _lowStockItems.Values)
            {
                if (!existingNotifications.Any(n => n.ItemId == item.Id))
                {
                    context.LowStockNotifications.Add(new LowStockNotification
                    {
                        ItemId = item.Id,
                        LastUpdated = DateTime.UtcNow
                    });
                }
            }

            // Remove outdated notifications
            foreach (var notification in existingNotifications)
            {
                if (!_lowStockItems.ContainsKey(notification.ItemId))
                {
                    context.LowStockNotifications.Remove(notification);
                }
            }

            await context.SaveChangesAsync();
        }

        /*    Routinely checks for low stock items      *
         *    then adds or removes items from a list    *
         *    using a binary search tree algorithm      */
        public async Task StartMonitoring(CancellationToken stoppingToken)
        {
            // Ensure cancellation token requested
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    // Store currentRunTime before performing check
                    var currentRunTime = DateTime.UtcNow;
                    _logger.LogInformation($"Task run started at {currentRunTime}");

                    // Set dbContext
                    using var dbContext = _dbContextFactory.CreateDbContext();

                    // Get items that have been updated since last task run
                    var updatedItems = await dbContext.Item
                        .Where(i => i.LastUpdated > _lastRunTime)
                        .ToListAsync(stoppingToken);

                    // Skip loop if there are no changes to make
                    if (_lowStockItems.IsEmpty && !updatedItems.Any())
                    {
                        _logger.LogInformation("No changes to update.");
                        continue;
                    }

                    var updatedItemIds = new HashSet<Guid>();

                    // For each item found
                    foreach (var item in updatedItems)
                    {
                        // Add itemId to HashSet for tracking changes in the bst
                        updatedItemIds.Add(item.Id);
                        // If the items quantity is <= threshold, add item to bst
                        _processItemNotifications.CheckItems(item);
                    }

                    // Retain valid low-stock items that weren't updated
                    foreach (var existingItem in _lowStockItems.Values)
                    {
                        // Skip items that have already been checked
                        if (updatedItemIds.Contains(existingItem.Id)) continue;

                        if (existingItem.Quantity > LowStockThreshold)
                        {
                            _avl.Remove(existingItem);
                            _lowStockItems.TryRemove(existingItem.Id, out _);
                        }
                    }

                    // Update last task runtime and update the datebasse
                    _lastRunTime = currentRunTime;

                    SyncDatabaseAsync();
                    _logger.LogInformation($"Task run completed at {currentRunTime}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred during task run - {_lastRunTime}");
                }
                try
                {
                    // Task delay schedules job
                    await Task.Delay(TaskDelay, stoppingToken);
                }
                catch (TaskCanceledException)
                {

                    _logger.LogCritical("The task has been cancelled");
                    break;
                }
            }
        }

    }
}
