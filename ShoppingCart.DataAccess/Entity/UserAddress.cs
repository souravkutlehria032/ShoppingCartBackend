using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DataAccess.Entity
{
    public class UserAddress
    {
        [Key]
        public int AddressId { get; set; }

        // Foreign key to associate with the ApplicationUser (User)
        [ForeignKey("UserId")]
        public Guid UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [Required]
        [StringLength(255)]
        public string AreaandStreet { get; set; }

        [Required]
        [StringLength(100)]
        public string? City { get; set; }

        [Required]
        [StringLength(100)]
        public string? State { get; set; }

        [Required]
        [StringLength(100)]
        public string Country { get; set; }

        [Required]
        [StringLength(10)]
        public string PostCode { get; set; }

        [StringLength(255)]
        public string? Landmark { get; set; }

        public bool IsDefault { get; set; } = true;
        public bool? IsDeleted { get; set; } = false;

        [Required]
        [Phone]
        [StringLength(17)]
        public string PhoneNumber { get; set; }

        [Phone]
        [StringLength(13)]
        public string? AlternatePhoneNumber { get; set; }
    }
}
