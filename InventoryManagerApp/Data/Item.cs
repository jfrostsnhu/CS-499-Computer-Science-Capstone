using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagerApp.Data
{
    public class Item
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(25)]
        public string SKU { get; set; } = string.Empty;
        [MaxLength(14)]
        public string? UPC { get; set; }
        [MaxLength(100)]
        public string Description { get; set; } = string.Empty;
        [MaxLength(100)]
        public string Vendor { get; set; } = string.Empty;
        [Required]
        [Precision(18,2)]
        public decimal Cost {  get; set; }
        [Required]
        [Precision(18, 2)]
        public decimal Price { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Invalid quantity")]
        public int Quantity { get; set; }
        [DisplayName("Last Updated")]
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}
