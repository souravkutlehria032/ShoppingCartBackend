using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DataAccess.Entity
{
    public class UserRefreshToken
    {
        public int Id { get; set; }
        public Guid UserId { get; set; } // changed from string to Guid
        public string RefreshTokenHash { get; set; }
        public DateTime ExpiryTime { get; set; }
        public string DeviceInfo { get; set; } // Optional, e.g., browser name or device ID
    }
}
