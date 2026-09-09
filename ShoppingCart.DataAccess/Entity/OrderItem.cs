using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DataAccess.Entity
{
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }

        [ForeignKey("OrderId")]
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }

        [ForeignKey("ProductVariantId")]
        public int ProductVariantId { get; set; }
        public virtual ProductVariant ProductVariant { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Discount { get; set; }
        public decimal OfferDiscount { get; set; }
        public decimal ShippingFee { get; set; }

        public decimal TotalPrice { get; set; }
    }
}
