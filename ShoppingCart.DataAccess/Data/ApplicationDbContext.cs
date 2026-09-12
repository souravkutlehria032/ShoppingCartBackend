using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.DataAccess.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Product> Product { get; set; }
        public DbSet<Fabric> Fabric { get; set; }
        public DbSet<Offer> Offer { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<ProductImage> ProductImage { get; set; }
        public DbSet<ProductVariant> ProductVariant { get; set; }
        public DbSet<Color> Color { get; set; }
        public DbSet<ProductSize> ProductSize { get; set; }
        public DbSet<Cart> Cart { get; set; }
        public DbSet<CartItem> CartItem { get; set; }
        public DbSet<ProductRating> ProductRating { get; set; }
        public DbSet<ReviewImage> ReviewImage { get; set; }
        public DbSet<UserAddress> UserAddress { get; set; }
        public DbSet<VendorAddress> VendorAddress { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<WishlistItem> WishlistItems { get; set; }
        public DbSet<OrderItem> OrderItem { get; set; }
        public DbSet<Payment> Payment { get; set; }
        public DbSet<Shipping> Shipping { get; set; }
        public DbSet<ProductReviewReply> ProductReviewReply { get; set; }
        public DbSet<Invoice> Invoice { get; set; }
        public DbSet<ReturnRequest> ReturnRequest { get; set; }
        public DbSet<Visitorlog> VisitLogs { get; set; }
        public DbSet<ReturnNotification> ReturnNotifications { get; set; }
        public DbSet<Notification> Notification { get; set; }

        public DbSet<Contact> Contact { get; set; }
        public DbSet<ContactReply> ContactReply { get; set; }
        public DbSet<NewsletterSubscriber> NewsletterSubscribers { get; set; }
        public DbSet<ProductAvailabilityRequest> ProductAvailabilityRequests { get; set; }
        public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Product to Vendor (AspNetUsers) relation
            builder.Entity<Product>()
                .HasOne(p => p.Vendor)
                .WithMany()
                .HasForeignKey(p => p.VendorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Offer>(entity =>
            {
                entity.Property(e => e.DiscountPercentage).HasPrecision(18, 2);
                entity.Property(e => e.FlatDiscountAmount).HasPrecision(18, 2);
            });

            builder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Product>()
                .HasOne(p => p.Fabric)
                .WithMany(f => f.Products)
                .HasForeignKey(p => p.FabricId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Product>()
                .HasOne(p => p.Offer)
                .WithMany(o => o.Products)
                .HasForeignKey(p => p.OfferId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Order>(entity =>
            {
                entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
            });

            builder.Entity<OrderItem>(entity =>
            {
                entity.Property(e => e.Discount).HasPrecision(18, 2);
                entity.Property(e => e.OfferDiscount).HasPrecision(18, 2);
                entity.Property(e => e.ShippingFee).HasPrecision(18, 2);
                entity.Property(e => e.TotalPrice).HasPrecision(18, 2);
                entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
            });

            builder.Entity<ProductVariant>(entity =>
            {
                entity.Property(e => e.DiscountPercentage).HasPrecision(18, 2);
                entity.Property(e => e.Price).HasPrecision(18, 2);
            });

            builder.Entity<ProductImage>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.Images)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProductVariant>()
                .HasOne(pv => pv.Product)
                .WithMany(p => p.Variants)
                .HasForeignKey(pv => pv.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProductVariant>()
                .HasOne(pv => pv.Color)
                .WithMany(c => c.ProductVariants)
                .HasForeignKey(pv => pv.ColorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Category>()
                .HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId);

            builder.Entity<Fabric>()
                .HasMany(f => f.Products)
                .WithOne(p => p.Fabric)
                .HasForeignKey(p => p.FabricId);

            builder.Entity<Color>()
                .HasMany(c => c.ProductVariants)
                .WithOne(pv => pv.Color)
                .HasForeignKey(pv => pv.ColorId);

            builder.Entity<Cart>()
                .HasMany(c => c.CartItems)
                .WithOne(ci => ci.Cart)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany()
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CartItem>()
                .HasOne(ci => ci.ProductVariant)
                .WithMany()
                .HasForeignKey(ci => ci.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ProductRating>()
                .HasMany(pr => pr.RatingImages)
                .WithOne(pri => pri.ProductRating)
                .HasForeignKey(pri => pri.ProductRatingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProductRating>()
                .HasOne(pr => pr.Product)
                .WithMany(p => p.Ratings)
                .HasForeignKey(pr => pr.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProductRating>()
                .HasOne(pr => pr.User)
                .WithMany()
                .HasForeignKey(pr => pr.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Addresses)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId);

            builder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<OrderItem>()
                .HasOne(oi => oi.ProductVariant)
                .WithMany()
                .HasForeignKey(oi => oi.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Order>()
                .HasOne(o => o.Shipping)
                .WithOne(s => s.Order)
                .HasForeignKey<Shipping>(s => s.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Order>()
                .HasOne(o => o.Address)
                .WithMany()
                .HasForeignKey(o => o.AddressId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Payment>(entity =>
            {
                entity.HasOne(p => p.Order)
                    .WithMany()
                    .HasForeignKey(p => p.OrderId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<Shipping>()
                .HasOne(s => s.Order)
                .WithOne(o => o.Shipping)
                .HasForeignKey<Order>(o => o.ShippingId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<VendorAddress>()
                .HasOne(va => va.Vendor)
                .WithOne()
                .HasForeignKey<VendorAddress>(va => va.VendorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProductVariant>()
                .HasOne(pv => pv.ProductSize)
                .WithMany(ps => ps.ProductVariants)
                .HasForeignKey(pv => pv.ProductSizeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
