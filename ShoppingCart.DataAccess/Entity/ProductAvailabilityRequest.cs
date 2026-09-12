using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DataAccess.Entity
{
    public class ProductAvailabilityRequest
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int? ProductSizeId { get; set; }
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public bool Notified { get; set; } = false;
        public Product Product { get; set; }
        public ProductSize ProductSize { get; set; }
    }
}
