using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DataAccess.Entity
{
    public class Category
    {
        public int CategoryId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public string CategoryImage { get; set; }

        public string? Description { get; set; }

        public bool IsDeleted { get; set; } = false;

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
