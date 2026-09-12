using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DataAccess.Entity
{
    public class ContactReply
    {
        public int Id { get; set; }
        public int ContactMessageId { get; set; }
        public string ReplyMessage { get; set; }
        public string RepliedBy { get; set; }
        public DateTime RepliedAt { get; set; }

        public Contact ContactMessage { get; set; }
    }
}
