using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Utility.DTOs
{
    public class PaymentDtos
    {
        public decimal Amount { get; set; }
        public Guid UserId { get; set; }
        public int OrderId { get; set; }
    }

    public class RazorPaymentResponse
    {
        public string razor_order_id { get; set; }
        public string razor_payment_id { get; set; }
        public string razor_signature { get; set; }
        public int orderId { get; set; }
    }

    public class CancelPaymentRequest
    {
        public int OrderId { get; set; }
    }

    public class SuccessfulOrderDto
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public OrderShippingDTO Shipping { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItemDto> OrderItems { get; set; } =
            new List<OrderItemDto>();
        public PaymentInfoDto PaymentInfo { get; set; } =
            new PaymentInfoDto();
        public string PaymentStatus { get; set; }
        public InvoiceDto? Invoice { get; set; }
    }

    public class InvoiceDto
    {
        public string InvoiceNumber { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public string FilePath { get; set; }

        public bool HasInvoice => !string.IsNullOrEmpty(FilePath);
    }

    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public bool IsReturnable { get; set; }
        public int Returndays { get; set; }
        public decimal TotalPrice { get; set; }
        public List<string> ImageUrls { get; set; } =
            new List<string>();
    }

    public class PaymentInfoDto
    {
        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentStatus { get; set; }
        public DateTime PaymentDate { get; set; }
        public string TransactionReference { get; set; } = string.Empty;
        public string? GatewayPaymentId { get; set; }
        public string? RefundReference { get; set; }
        public DateTime? RefundDate { get; set; }
    }

    public class RefundRequestDto
    {
        public int OrderId { get; set; }
        public decimal? Amount { get; set; }
        public string Reason { get; set; } = "Customer Request";
        public bool RestockItems { get; set; } = true;
    }
}
