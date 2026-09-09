using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DataAccess.Entity
{
    public class Fabric
    {
        public int fabricId { get; set; }
        public string fabricName { get; set; }
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
