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
    public class VendorAddress
    {
        [Key]
        public int VendorAddressId { get; set; }

        [ForeignKey("VendorId")]
        public Guid VendorId { get; set; }
        public virtual ApplicationUser Vendor { get; set; }

        [Required]
        [StringLength(255)]
        public string ShopName { get; set; }

        [Required]
        [StringLength(50)]
        public string GSTNumber { get; set; }

        [Required]
        [StringLength(255)]
        public string AreaandStreet { get; set; }

        [Required]
        [StringLength(100)]
        public string City { get; set; }

        [Required]
        [StringLength(100)]
        public string State { get; set; }

        [Required]
        [StringLength(100)]
        public string Country { get; set; }

        [Required]
        [StringLength(10)]
        public string PostCode { get; set; }

        [StringLength(255)]
        public string? Landmark { get; set; }

        public bool IsDefault { get; set; } = true;

        [Required]
        [Phone]
        [StringLength(13)]
        public string PhoneNumber { get; set; }

        [Phone]
        [StringLength(13)]
        public string? AlternatePhoneNumber { get; set; }

        public VendorAddressType VendorAddressType { get; set; }
    }
}
