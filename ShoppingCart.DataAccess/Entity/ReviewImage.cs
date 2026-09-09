using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DataAccess.Entity
{
    public class ReviewImage
    {
        public int Id { get; set; }

        [ForeignKey("ProductRatingId")]
        public int ProductRatingId { get; set; }

        public ProductRating ProductRating { get; set; }

        public string ImageUrl { get; set; }
    }
}
