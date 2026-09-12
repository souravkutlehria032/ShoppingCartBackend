using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DataAccess.Entity
{
    public class NewsletterSubscriber
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public DateTime SubscribedOn { get; set; } = DateTime.UtcNow;
        public int? ProductVariantId { get; set; }
    }
}
