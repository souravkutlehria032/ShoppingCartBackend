using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DataAccess.Entity
{
    public class Offer
    {
        public int OfferId { get; set; }

        [Required]
        [StringLength(100)]
        public string OfferName { get; set; }

        [StringLength(500)]
        public string OfferDescription { get; set; }

        public bool IsPercentageDiscount { get; set; }

        [Range(0, 100)]
        public decimal? DiscountPercentage { get; set; }

        public string OfferBannerUrl { get; set; }

        public decimal? FlatDiscountAmount { get; set; }

        public bool IsActive { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
