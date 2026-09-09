using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DataAccess.Entity
{
    public class ProductReviewReply
    {
        public int Id { get; set; }

        public int ProductRatingId { get; set; }

        public ProductRating ProductRating { get; set; }

        public Guid UserId { get; set; }

        public ApplicationUser User { get; set; }

        public string ReplyText { get; set; }

        public DateTime RepliedAt { get; set; } = DateTime.UtcNow;
    }
}
