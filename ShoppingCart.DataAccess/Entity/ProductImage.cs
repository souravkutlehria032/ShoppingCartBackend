using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DataAccess.Entity
{
    public class ProductImage
    {
        [Key]
        public int ImageId { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        [ForeignKey("ProductId")]
        public int ProductId { get; set; }

        public virtual Product Product { get; set; }

        // Foreign key for Color, so images can be associated with a specific color
        [ForeignKey("ColorId")]
        public int? ColorId { get; set; }

        public virtual Color Color { get; set; }

        // Optional: If the image is specific to a variant (Color+Size)
        public int? ProductVariantId { get; set; }

        public virtual ProductVariant ProductVariant { get; set; }
    }
}
