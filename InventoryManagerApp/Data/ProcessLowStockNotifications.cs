using System.Collections.Concurrent;

namespace InventoryManagerApp.Data
{
    public class ProcessLowStockNotifications
    {
        private readonly NotificationsAVLTree _avl;
        private readonly ConcurrentDictionary<Guid, Item> _lowStockItems;
        private readonly int _lowStockThreshold;

        public ProcessLowStockNotifications(NotificationsAVLTree avl, ConcurrentDictionary<Guid, Item> lowStockItems, int lowStockThreshold)
        {
            _avl = avl ?? throw new ArgumentNullException(nameof(avl));
            _lowStockItems = lowStockItems ?? throw new ArgumentNullException(nameof(lowStockItems));
            _lowStockThreshold = lowStockThreshold;
        }

        // Check if items need to be added or removed from AVL
        public void CheckItems(Item item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            if (item.Quantity <= _lowStockThreshold)
            {
                // If the items quantity is less than or equal to the threshold add it
                _avl.Insert(item);
                _lowStockItems[item.Id] = item;
            }
            else
            {
                // Otherwise, remove it
                _avl.Remove(item);
                _lowStockItems.TryRemove(item.Id, out _);
            }
        }
    }
}
