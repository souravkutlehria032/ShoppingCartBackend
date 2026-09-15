using Microsoft.AspNetCore.SignalR;
using ShoppingCart.DataAccess.BackgroundService;
using ShoppingCart.DataAccess.Data;
using ShoppingCart.DataAccess.Entity;
using ShoppingCart.DataAccess.Repository.IRepo;
using ShoppingCart.Utility.DTOs;
using ShoppingCart.Utility.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DataAccess.Repository.Repo
{
    public class ReturnRequestRepository : IReturnRequestRepository
    {
        private readonly ApplicationDbContext _dbcontext;
        private readonly IOrderRepository _orderRepository;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IProductManagementRepository _productRepository;
        private readonly IEmailService _emailService;

        public ReturnRequestRepository(
            ApplicationDbContext dbcontext,
        IOrderRepository orderRepository,
        IHubContext<NotificationHub> hubContext,
        IProductManagementRepository productRepository,
        IEmailService emailService)
        {
            _dbcontext = dbcontext;
            _orderRepository = orderRepository;
            _hubContext = hubContext;
            _productRepository = productRepository;
            _emailService = emailService;
        }

        public Task<ReturnRequest> CreateReturnRequestAsync(CreateReturnRequestDto dto, Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<ReturnRequest?> GetByIdAsync(int requestId, Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<ReturnRequest>> GetUserReturnRequestsAsync(Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}
