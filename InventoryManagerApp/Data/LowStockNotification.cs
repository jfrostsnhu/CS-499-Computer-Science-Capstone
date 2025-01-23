using System.ComponentModel.DataAnnotations;

namespace InventoryManagerApp.Data
{
    // Entity Model for LowStock Notification
    public class LowStockNotification
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid ItemId { get; set; }
        [Required]
        public DateTime LastUpdated { get; set; }

        public Item? Item { get; set; }
    }
}
