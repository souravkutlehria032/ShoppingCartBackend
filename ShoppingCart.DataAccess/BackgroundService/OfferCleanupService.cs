using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ShoppingCart.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Utility.Services
{
    public class OfferCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<OfferCleanupService> _logger;

        public OfferCleanupService(
            IServiceScopeFactory scopeFactory,
            ILogger<OfferCleanupService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();

                    var context = scope.ServiceProvider
                        .GetRequiredService<ApplicationDbContext>();

                    var now = DateTime.Now;

                    var expiredOffers = context.Offer
                        .Where(o => o.IsActive && o.EndDate < now)
                        .ToList();

                    foreach (var offer in expiredOffers)
                    {
                        offer.IsActive = false;

                        var products = context.Product
                            .Where(p => p.OfferId == offer.OfferId)
                            .ToList();

                        foreach (var product in products)
                        {
                            product.OfferId = null;
                        }
                    }

                    if (expiredOffers.Any())
                    {
                        await context.SaveChangesAsync();

                        _logger.LogInformation(
                            "Expired offers cleaned at {time}",
                            now);
                    }

                    // Wait 1 hour
                    await Task.Delay(
                        TimeSpan.FromHours(1),
                        stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error cleaning expired offers");

                    await Task.Delay(
                        TimeSpan.FromMinutes(5),
                        stoppingToken);
                }
            }
        }
    }
}
