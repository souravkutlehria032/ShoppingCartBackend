using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ShoppingCart.Utility.Enums.Enums;

namespace ShoppingCart.Utility.DTOs
{
    public class PlaceOrderRequest
    {
        public Guid UserId { get; set; }
        public string PaymentMethod { get; set; }
        public List<OrderItemRequest> Items { get; set; }
    }

    public class OrderItemRequest
    {
        public int ProductVariantId { get; set; }
        public int Quantity { get; set; }
    }

    public class ReturnRequestDto
    {
        public int ReturnRequestId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string Reason { get; set; }
        public decimal? RefundAmount { get; set; }
        public bool IsRefunded { get; set; }
        public string Type { get; set; }
        public ReturnStatus Return_Status { get; set; }
        public DateTime RequestedAt { get; set; }
    }

    public class OrderDetailDto
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public List<ReturnRequestDto>? ReturnRequests { get; set; }
        public OrderUserDTO User { get; set; }
        public DateTime ChangedAt { get; set; }
        public OrderShippingDTO Shipping { get; set; }
        public OrderAddressDTO Address { get; set; }
        public List<AdminOrderItemDTO> Items { get; set; } = new List<AdminOrderItemDTO>();
    }

    public class OrderUserDTO
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class OrderShippingDTO
    {
        public int ShippingId { get; set; }
        public int OrderId { get; set; }
        public string? TrackingNumber { get; set; }
        public DateTime? EstimatedDelivery { get; set; }
        public DateTime? ShippedDate { get; set; }
        public string ShippedStatus { get; set; }
        public string Carrier { get; set; }
        public string orderNumber { get; set; }
        public string? ReturnTrackingNumber { get; set; }
        public string? ReturnCarrier { get; set; }
        public DateTime? ReturnPickupDate { get; set; }
        public DateTime? ReturnDeliveredDate { get; set; }
        public ReturnStatus ReturnStatus { get; set; }

        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }

        public string PickupAddress { get; set; } = "Your Warehouse Address";

        public string DeliveryAddress { get; set; }
        public string DeliveryCity { get; set; }
        public string DeliveryState { get; set; }
        public string DeliveryPostalCode { get; set; }
        public string DeliveryCountry { get; set; }
        public string DeliveryPhone { get; set; }

        public decimal Weight { get; set; } = 0.5m;

        public List<OrderProductDTO> Products { get; set; } =
            new List<OrderProductDTO>();
    }

    public class OrderProductDTO
    {
        public string Name { get; set; }
        public string SKU { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class ReturnShippingDTO
    {
        public int OrderId { get; set; }
        public int ReturnRequestId { get; set; }
        public DateTime? PickupDate { get; set; }
        public decimal? Weight { get; set; } = 0.5m;
        public string? WarehouseAddress { get; set; }
        public List<OrderProductDTO>? Products { get; set; }
    }

    public class OrderAddressDTO
    {
        public string AreaandStreet { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class AdminOrderItemDTO
    {
        public int OrderItemId { get; set; }
        public int ProductVariantId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal OfferDiscount { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal TotalPrice { get; set; }
        public AdminPaymentInfoDto PaymentInfo { get; set; } =
            new AdminPaymentInfoDto();

        public string BrandName { get; set; }
        public string Category { get; set; }
        public bool IsReturnable { get; set; }
        public int ReturnDays { get; set; }
    }

    public class AdminPaymentInfoDto
    {
        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentStatus { get; set; }
        public DateTime PaymentDate { get; set; }
        public string TransactionReference { get; set; } = string.Empty;
        public string? GatewayPaymentId { get; set; }
        public decimal? RefundAmount { get; set; }
        public DateTime? RefundDate { get; set; }
        public string? RefundReason { get; set; }
        public string? RefundReference { get; set; }
    }
}
