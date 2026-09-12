using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DataAccess.Entity
{
    public class ReturnNotification
    {
        public int Id { get; set; }  // Primary key
        public int ReturnRequestId { get; set; }  // Link to ReturnRequest
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string Type { get; set; }  // Return / Replace
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ReturnRequest ReturnRequest { get; set; }
    }
}
