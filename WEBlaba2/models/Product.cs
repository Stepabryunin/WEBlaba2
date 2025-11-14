using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WEBlaba2.models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        public string ImageUrl { get; set; }

        public string Category { get; set; }

        public string PageUrl { get; set; }
        public int count {  get; set; }

        // Навигационное свойство для корзины
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}