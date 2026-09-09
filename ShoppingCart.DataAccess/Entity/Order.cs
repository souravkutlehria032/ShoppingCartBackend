using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ShoppingCart.Utility.Enums.Enums;

namespace ShoppingCart.DataAccess.Entity
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        public string OrderNumber { get; set; }

        [ForeignKey("UserId")]
        public Guid UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public OrderStatus Status { get; set; }

        public decimal TotalAmount { get; set; }

        public string PaymentMethod { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public ICollection<ReturnRequest> ReturnRequests { get; set; }

        [ForeignKey("ShippingId")]
        public int? ShippingId { get; set; }

        public virtual Shipping Shipping { get; set; }

        [ForeignKey("AddressId")]
        public int? AddressId { get; set; }

        public virtual UserAddress Address { get; set; }
    }
}
