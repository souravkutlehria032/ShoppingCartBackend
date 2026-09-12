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
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        [ForeignKey("OrderId")]
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }

        public PaymentStatus Status { get; set; }

        public string PaymentMethod { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        public string? GatewayReference { get; set; }
        public string? GatewayPaymentId { get; set; }

        public decimal? RefundAmount { get; set; }
        public DateTime? RefundDate { get; set; }
        public string? RefundReason { get; set; }
        public string? RefundReference { get; set; }
    }
}
