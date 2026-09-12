using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ShoppingCart.Utility.Enums.Enums;

namespace ShoppingCart.Utility.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public bool? IsSuspended { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool? LockoutEnabled { get; set; }
        public int? AccessFailedCount { get; set; }
        public IFormFile? ProfilePicture { get; set; }
    }

    public class ContactUsDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ContactWithRepliesDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ContactReplyDto> Replies { get; set; }
    }

    public class ContactReplyDto
    {
        public int Id { get; set; }
        public int ContactId { get; set; }
        public string ReplyMessage { get; set; }
        public string RepliedBy { get; set; }
        public DateTime RepliedAt { get; set; }
    }

    public class AddressDto
    {
        [Required]
        public string AreaandStreet { get; set; }

        [StringLength(30)]
        public string? City { get; set; }

        [StringLength(30)]
        public string? State { get; set; }

        [Required]
        [StringLength(100)]
        public string Country { get; set; }

        [Required]
        [StringLength(6)]
        public string PostCode { get; set; }

        [StringLength(255)]
        public string? Landmark { get; set; }

        [Required]
        [Phone]
        [StringLength(13)]
        public string PhoneNumber { get; set; }

        [Phone]
        [StringLength(13)]
        public string? AlternatePhoneNumber { get; set; }

        public bool IsDefault { get; set; }

        public bool? IsDeleted { get; set; } = false;
    }

    public class VendorAddressDto
    {
        [Required]
        [StringLength(255)]
        public string ShopName { get; set; }

        [Required]
        [StringLength(50)]
        public string GSTNumber { get; set; }

        [Required]
        [StringLength(255)]
        public string AreaandStreet { get; set; }

        [Required]
        [StringLength(100)]
        public string? City { get; set; }

        [Required]
        [StringLength(100)]
        public string? State { get; set; }

        [Required]
        [StringLength(100)]
        public string Country { get; set; }

        [Required]
        [StringLength(10)]
        public string PostCode { get; set; }

        [StringLength(255)]
        public string? Landmark { get; set; }

        public bool IsDefault { get; set; } = true;

        [Required]
        [Phone]
        [StringLength(13)]
        public string PhoneNumber { get; set; }

        [Phone]
        [StringLength(13)]
        public string? AlternatePhoneNumber { get; set; }

        public VendorAddressType VendorAddressType { get; set; }
    }

    public class WishlistResponse
    {
        public int WishlistItemId { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public decimal? Price { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public string? MainImageUrl { get; set; }
        public int? VariantId { get; set; }
        public bool? InStock { get; set; }
        public bool IsInWishlist { get; set; }
    }

    public class GetVendorAddressDto
    {
        public string? ShopName { get; set; }
        public string? GSTNumber { get; set; }
        public string? AreaandStreet { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostCode { get; set; }
        public string? Landmark { get; set; }
        public bool IsDefault { get; set; }
        public string? PhoneNumber { get; set; }
        public string? AlternatePhoneNumber { get; set; }
        public VendorAddressType? VendorAddressType { get; set; }
    }

    public class CartDto
    {
        public Guid UserId { get; set; }
        public int productId { get; set; }
        public int VariantId { get; set; }
        public int ColorId { get; set; }
        public int ProductSize { get; set; }
    }

    public class CartItemDto
    {
        public Guid UserId { get; set; }
        public int Quantity { get; set; }
        public int ProductId { get; set; }
        public int VariantId { get; set; }
        public bool IsUpdateFromCart { get; set; } = false;
    }

    public class CartDetailsDto
    {
        public int CartId { get; set; }
        public Guid UserId { get; set; }
        public List<CartItemsDto> CartItems { get; set; } = new();
    }

    public class CartItemsDto
    {
        public int CartItemId { get; set; }
        public int ProductId { get; set; }
        public int VariantId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductImage { get; set; } = string.Empty;
        public decimal FinalPrice { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountPercentage { get; set; }
        public int? OfferId { get; set; }
        public decimal? TaxPercentage { get; set; }
        public string SelectedSize { get; set; } = string.Empty;
        public string SelectedColor { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int StockAvailability { get; set; }
        public decimal Subtotal { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class RemoveCartItemDto
    {
        public int ProductId { get; set; }
        public int VariantId { get; set; }
    }

    public class SizeChart
    {
        public string SizeName { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public string Chest { get; set; }
        public string Waist { get; set; }
    }

    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class ColorDto
    {
        public int ColorId { get; set; }
        public string ColorName { get; set; }
        public string? HexCode { get; set; }
    }

    public class FabricDto
    {
        public int FabricId { get; set; }
        public string FabricName { get; set; }
    }

    public class ProductSizeDto
    {
        public int ProductSizeId { get; set; }
        public string SizeName { get; set; }
    }

    public class FiltrationDataDto
    {
        public IEnumerable<CategoryDto> Categories { get; set; }
        public IEnumerable<ColorDto> Colors { get; set; }
        public IEnumerable<FabricDto> Fabrics { get; set; }
        public IEnumerable<ProductSizeDto> Sizes { get; set; }
        public IEnumerable<string> Genders { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public string SearchTerm { get; set; }
    }

    public class GeoJsResponse
    {
        public string country { get; set; }
        public string country_code { get; set; }
        public string city { get; set; }
        public string region { get; set; }
        public string ip { get; set; }
    }

    public class Location
    {
        public Country Country { get; set; }
    }

    public class Country
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class CurrencyInfoDto
    {
        public string Country { get; set; } = string.Empty;
        public string CurrencyCode { get; set; } = string.Empty;
        public string CurrencySymbol { get; set; } = string.Empty;
        public decimal ConversionRate { get; set; }
    }

    public class CreateReturnRequestDto
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public ReturnType Type { get; set; }
        public string Reason { get; set; }
        public decimal? RefundAmount { get; set; }
    }

    public class NotificationDto
    {
        public int Id { get; set; }
        public string RecipientRole { get; set; } = "Admin";
        public string OrderNumber { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string ReturnType { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public DateTime RequestedAt { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PagedNotificationsDto
    {
        public IEnumerable<NotificationDto> Notifications { get; set; } =
            Enumerable.Empty<NotificationDto>();

        public int TotalCount { get; set; }
        public int UnreadCount { get; set; }
    }
}
