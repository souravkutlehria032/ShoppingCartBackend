using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DataAccess.Entity
{
    public class Notification
    {
        public int Id { get; set; }

        // Who is this for? You can keep this "Admin" for now.
        public string RecipientRole { get; set; } = "Admin";

        // Optional: tie to a specific admin or vendor if you add auth later
        public Guid? RecipientUserId { get; set; }

        // Payload based on what you already send via SignalR
        public string OrderNumber { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string ReturnType { get; set; } = string.Empty;   // "Return" | "Replace" etc.
        public string Reason { get; set; } = string.Empty;

        // When the return was requested (your business event time)
        public DateTime RequestedAt { get; set; }

        // Notification metadata
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
