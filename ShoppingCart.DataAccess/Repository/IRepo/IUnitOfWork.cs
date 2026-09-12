using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DataAccess.Repository.IRepo
{
    public interface IUnitOfWork
    {
        IAuthManagementRepository Auth { get; }
        IProductManagementRepository Product { get; }
        IVendorRepository Vendor { get; }
        IUserRepository User { get; }
        IOrderRepository Order { get; }
        IPaymentRepository Payment { get; }
        IReturnRequestRepository ReturnRequest { get; }

        void Save();
    }
}
