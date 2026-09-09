using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ShoppingCart.Utility.Enums.Enums;

namespace ShoppingCart.DataAccess.Entity
{
    public class ReturnRequest
    {
        public int ReturnRequestId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public ReturnType Type { get; set; }
        public string Reason { get; set; } = string.Empty;
        public decimal? RefundAmount { get; set; }
        public bool IsRefunded { get; set; }
        public ReturnStatus Return_ReplaceStatus { get; set; }
        public DateTime RequestedAt { get; set; }

        public Order Order { get; set; }
        public Product Product { get; set; }
    }
}
