using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using ShoppingCart.DataAccess.BackgroundService;
using ShoppingCart.DataAccess.Data;
using ShoppingCart.DataAccess.Entity;
using ShoppingCart.DataAccess.Repository.IRepo;
using ShoppingCart.Utility.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DataAccess.Repository.Repo
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OrderRepository> _logger;
        private readonly IHubContext<NotificationHub> _hubContext;

        public OrderRepository(
            ApplicationDbContext context,
            ILogger<OrderRepository> logger,
            IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _logger = logger;
            _hubContext = hubContext;
        }

        public ReturnMessageDto AddDeliveryAddress(Guid userId, int addressId, int orderId)
        {
            throw new NotImplementedException();
        }

        public Task<(ReturnMessageDto response, int? orderId)> AddOrderDetails(PlaceOrderRequest orderRequest)
        {
            throw new NotImplementedException();
        }

        public Task<ReturnMessageDto> BulkShipOrdersAsync(List<OrderShippingDTO> dtos)
        {
            throw new NotImplementedException();
        }

        public string GenerateId(string type)
        {
            throw new NotImplementedException();
        }

        public Task<(List<OrderDetailDto> Order, int TotalCount, Dictionary<string, int> StatusCounts)> GetAllOrders(int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<(List<OrderDetailDto> Order, int TotalCount, Dictionary<string, int> StatusCounts)> GetFilteredOrders(int pageNumber, int pageSize, string searchTerm = null, List<string> customerName = null, List<string> OrderNumber = null, List<string> OrderDate = null, List<string> Status = null)
        {
            throw new NotImplementedException();
        }

        public Task<OrderSummaryDto> GetFinalProductPricingAsync(int variantId, int quantity)
        {
            throw new NotImplementedException();
        }

        public Task<SuccessfulOrderDto?> GetOrderDetailsByOrderIdAndUserIdAsync(int orderId, Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<SuccessfulOrderDto>> GetSuccessfulOrdersByUserIdAsync(Guid userId, int page = 1, int pageSize = 10)
        {
            throw new NotImplementedException();
        }

        public Task<Shipping> ReturnShipOrderAsync(ReturnShippingDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<Shipping> ShipOrderAsync(OrderShippingDTO shippingdto)
        {
            throw new NotImplementedException();
        }
    }
}
