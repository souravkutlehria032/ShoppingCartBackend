using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DataAccess.Entity
{
    public class ProductRating
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }

        public decimal RatingValue { get; set; }

        public string Review { get; set; }

        public DateTime RatedAt { get; set; } = DateTime.Now;

        public ICollection<ReviewImage> RatingImages { get; set; } = new List<ReviewImage>();

        public ICollection<ProductReviewReply> Replies { get; set; } = new List<ProductReviewReply>();
    }
}
