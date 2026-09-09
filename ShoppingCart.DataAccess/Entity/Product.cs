using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static ShoppingCart.Utility.Enums.Enums;

namespace ShoppingCart.DataAccess.Entity
{
    public class Product
    {
        public int ProductId { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [Required]
        [StringLength(255)]
        public string BrandName { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [Required]
        public string MaterialComposition { get; set; } // Example: "80% Cotton, 20% Polyester"

        [Required]
        [StringLength(500)]
        public string CareInstructions { get; set; } // Example: "Machine Wash Cold"

        [Required]
        public Season Season { get; set; } // Example: Summer, Winter, etc.

        [Required]
        public Occasion Occasion { get; set; } // Example: Casual, Party, etc.

        public bool IsReturnable { get; set; } = true; // Default to returnable
        public int ReturnDays { get; set; } = 7;

        public bool IsVisible { get; set; } = true;

        public DateTime CreatedAt { get; set; }

        public int? OfferId { get; set; }
        public virtual Offer Offer { get; set; }

        // Navigation properties
        [ForeignKey("CategoryId")]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        [JsonIgnore]
        public virtual ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();

        public int FabricId { get; set; }
        public virtual Fabric Fabric { get; set; }

        [ForeignKey("VendorId")]
        public Guid VendorId { get; set; }
        public virtual ApplicationUser Vendor { get; set; }

        public virtual ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
        public virtual ICollection<ProductRating> Ratings { get; set; } = new List<ProductRating>();
    }
}
