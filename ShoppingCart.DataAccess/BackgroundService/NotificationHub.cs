using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ShoppingCart.DataAccess.BackgroundService
{
    public class NotificationHub : Hub
    {
        // Admins join this group to receive notifications
        public async Task JoinAdminGroup()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Admin");
        }
    }
}
