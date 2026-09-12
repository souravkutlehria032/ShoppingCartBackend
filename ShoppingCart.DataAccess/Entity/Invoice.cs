using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DataAccess.Entity
{
    public class Invoice
    {
        [Key]
        public int InvoiceId { get; set; }

        [ForeignKey("OrderId")]
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }

        public string InvoiceNumber { get; set; }

        public DateTime IssueDate { get; set; }

        public DateTime DueDate { get; set; }

        public string FilePath { get; set; }

        public byte[] FileContent { get; set; }

        [ForeignKey("PaymentId")]
        public int PaymentId { get; set; }
        public virtual Payment Payment { get; set; }
    }
}
