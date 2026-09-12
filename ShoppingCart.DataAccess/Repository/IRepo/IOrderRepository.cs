using ShoppingCart.DataAccess.Entity;
using ShoppingCart.Utility.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DataAccess.Repository.IRepo
{
    public interface IOrderRepository
    {
        #region Order

        Task<OrderSummaryDto> GetFinalProductPricingAsync(int variantId, int quantity);

        Task<(ReturnMessageDto response, int? orderId)> AddOrderDetails(
            PlaceOrderRequest orderRequest);

        string GenerateId(string type);

        Task<List<SuccessfulOrderDto>> GetSuccessfulOrdersByUserIdAsync(
            Guid userId,
            int page = 1,
            int pageSize = 10);

        ReturnMessageDto AddDeliveryAddress(
            Guid userId,
            int addressId,
            int orderId);

        Task<(List<OrderDetailDto> Order, int TotalCount, Dictionary<string, int> StatusCounts)>
            GetAllOrders(int pageNumber, int pageSize);

        Task<(List<OrderDetailDto> Order, int TotalCount, Dictionary<string, int> StatusCounts)>
            GetFilteredOrders(
                int pageNumber,
                int pageSize,
                string searchTerm = null,
                List<string> customerName = null,
                List<string> OrderNumber = null,
                List<string> OrderDate = null,
                List<string> Status = null);

        Task<SuccessfulOrderDto?> GetOrderDetailsByOrderIdAndUserIdAsync(
            int orderId,
            Guid userId);

        #endregion

        #region Shipping

        Task<Shipping> ShipOrderAsync(OrderShippingDTO shippingdto);

        Task<ReturnMessageDto> BulkShipOrdersAsync(
            List<OrderShippingDTO> dtos);

        Task<Shipping> ReturnShipOrderAsync(
            ReturnShippingDTO dto);

        #endregion
    }
}
