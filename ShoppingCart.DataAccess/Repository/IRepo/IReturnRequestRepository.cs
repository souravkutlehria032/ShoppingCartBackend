using ShoppingCart.DataAccess.Entity;
using ShoppingCart.Utility.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DataAccess.Repository.IRepo
{
    public interface IReturnRequestRepository
    {
        Task<ReturnRequest> CreateReturnRequestAsync(
            CreateReturnRequestDto dto,
            Guid userId);

        Task<List<ReturnRequest>> GetUserReturnRequestsAsync(
            Guid userId);

        Task<ReturnRequest?> GetByIdAsync(
            int requestId,
            Guid userId);
    }
}
