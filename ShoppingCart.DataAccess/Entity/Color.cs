using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DataAccess.Entity
{
    public class Color
    {
        public int ColorId { get; set; }

        [Required]
        [StringLength(100)]
        public string ColorName { get; set; }

        [StringLength(7)]
        public string? HexCode { get; set; }

        // Navigation property to the products that have this color
        public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
    }
}
