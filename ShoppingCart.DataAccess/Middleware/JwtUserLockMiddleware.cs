using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using ShoppingCart.DataAccess.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DataAccess.Middleware
{
    public class JwtUserLockMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtUserLockMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(
            HttpContext context,
            UserManager<ApplicationUser> userManager)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var user = await userManager.GetUserAsync(context.User);

                if (user != null)
                {
                    if (!user.IsActive ||
                        (user.LockoutEnd.HasValue &&
                         user.LockoutEnd > DateTimeOffset.Now))
                    {
                        context.Response.StatusCode = 401;

                        await context.Response.WriteAsync(
                            "Account locked or deactivated.");

                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}
