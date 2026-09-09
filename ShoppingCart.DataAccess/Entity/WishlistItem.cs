using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DataAccess.Entity
{
    public class WishlistItem
    {
        public int WishlistItemId { get; set; }

        public string UserId { get; set; } // This should match your user ID type (string for Identity, int for custom)

        public int ProductId { get; set; }

        public int? ProductVariantId { get; set; } // Nullable if variant is optional

        public DateTime AddedDate { get; set; } = DateTime.UtcNow;

        public bool IsInWishlist { get; set; }

        // Navigation properties
        public virtual Product Product { get; set; }

        public virtual ProductVariant ProductVariant { get; set; }
    }
}
