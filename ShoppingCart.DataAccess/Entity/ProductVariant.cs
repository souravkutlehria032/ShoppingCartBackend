using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ShoppingCart.DataAccess.Entity
{
    public class ProductVariant
    {
        public int ProductVariantId { get; set; }

        // Foreign key for Product
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        // Foreign key for Color
        public int ColorId { get; set; }
        public virtual Color Color { get; set; }

        // Foreign key for ProductSize
        public int ProductSizeId { get; set; }
        public virtual ProductSize ProductSize { get; set; }

        public decimal? TaxPercentage { get; set; }
        public int StockQuantity { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; } // Variant-specific price

        [Range(0, 100)]
        public decimal? DiscountPercentage { get; set; } // Variant-specific discount

        // A collection of images specific to this variant's color
        [JsonIgnore]
        public virtual ICollection<ProductImage> VariantImages { get; set; } = new List<ProductImage>();

        public decimal FinalPrice
        {
            get
            {
                decimal discountAmount = 0;

                // Apply variant-specific discount if available
                if (DiscountPercentage.HasValue)
                {
                    discountAmount = Price * (DiscountPercentage.Value / 100);
                }

                return Price - discountAmount;
            }
        }
    }
}
