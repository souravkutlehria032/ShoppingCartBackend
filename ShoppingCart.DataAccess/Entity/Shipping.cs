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
    public class Shipping
    {
        [Key]
        public int ShippingId { get; set; }

        [ForeignKey("OrderId")]
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }

        public int? ReturnRequestId { get; set; }

        public string? TrackingNumber { get; set; }

        public ShippingType Type { get; set; }

        public ShippingStatus Status { get; set; }

        public DateTime? ShippedDate { get; set; }
        public DateTime? DeliveredDate { get; set; }

        public string Carrier { get; set; }

        public string? ReturnTrackingNumber { get; set; }
        public string? ReturnCarrier { get; set; }
        public DateTime? ReturnPickupDate { get; set; }
        public DateTime? ReturnDeliveredDate { get; set; }

        public ReturnStatus ReturnStatus { get; set; }
    }
}
