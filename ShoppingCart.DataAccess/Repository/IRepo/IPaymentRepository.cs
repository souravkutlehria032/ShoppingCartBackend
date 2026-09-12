using ShoppingCart.DataAccess.Entity;
using ShoppingCart.Utility.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DataAccess.Repository.IRepo
{
    public interface IPaymentRepository
    {
        Task<ReturnMessageDto> CreateOrderAsync(PaymentDtos paymentRequest);

        Task<ReturnMessageDto> VerifyPayment(
            RazorPaymentResponse paymentResponse);

        Task<ReturnMessageDto> IssueRefundAsync(
            RefundRequestDto refundRequest);

        Task<Invoice> GenerateInvoiceAsync(
            Order order,
            Payment payment);

        public byte[] GenerateInvoicePdf(
            Entity.Order order,
            Entity.Payment payment,
            Entity.Invoice invoice);

        string GenerateId(string type);

        public Task<ReturnMessageDto> CancelPaymentAsync(int orderId);
    }
}
