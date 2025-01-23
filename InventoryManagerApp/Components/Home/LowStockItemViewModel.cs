namespace InventoryManagerApp.Components.Home
{
    // View Model for Low Stock Items for Home page
    public class LowStockItemViewModel
    {
        public Guid Id { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string? UPC { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Vendor { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
