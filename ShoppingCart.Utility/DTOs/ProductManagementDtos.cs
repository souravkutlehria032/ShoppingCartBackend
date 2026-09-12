using Microsoft.AspNetCore.Http;
using ShoppingCart.Utility.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ShoppingCart.Utility.Enums.Enums;

namespace ShoppingCart.Utility.DTOs
{
    public class AddCategoryDto
    {
        public string CategoryName { get; set; }
        public string? Description { get; set; }
        public IFormFile CategoryImage { get; set; }
    }

    public class UpdateCategoryDto
    {
        public string? CategoryName { get; set; }
        public string? Description { get; set; }
        public IFormFile? CategoryImage { get; set; }
    }

    public class CreateProductDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string BrandName { get; set; }

        public string Description { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [Required]
        public string MaterialComposition { get; set; }

        [Required]
        [StringLength(500)]
        public string CareInstructions { get; set; }

        [Required]
        public Season Season { get; set; }

        [Required]
        public Occasion Occasion { get; set; }

        public bool IsReturnable { get; set; } = true;
        public int ReturnDays { get; set; } = 7;

        public int CategoryId { get; set; }
        public int? OfferId { get; set; }

        [Required]
        public string FabricName { get; set; }

        [NonZeroStockQuantity(ErrorMessage = "All sizes must have a stock quantity greater than zero.")]
        public List<CreateProductVariantDto> Variants { get; set; } =
            new List<CreateProductVariantDto>();

        public List<int>? RemovedVariantIds { get; set; } = new();
    }

    public class ProductAvailabilityRequestDto
    {
        public string ProductId { get; set; }
        public string ProductSize { get; set; }
        public string UserId { get; set; }
    }

    public class CreateProductVariantDto
    {
        public int? ProductVariantId { get; set; }

        public string ColorName { get; set; }
        public string HexCode { get; set; }

        public List<SizeQuantityDto> SizesAndQuantities { get; set; } =
            new List<SizeQuantityDto>();

        public List<IFormFile> VariantImages { get; set; } =
            new List<IFormFile>();

        public List<string> ExistingImages { get; set; } = new();
        public List<string> RemovedImages { get; set; } = new();
    }

    public class SizeQuantityDto
    {
        public int ProductSizeId { get; set; }
        public int StockQuantity { get; set; }
        public int? ProductVariantId { get; set; }

        [Required]
        public decimal Price { get; set; }

        public decimal? DiscountPercentage { get; set; }
    }

    public class ProductDetailsDto
    {
        public int ProductId { get; set; }
        public string? Name { get; set; }
        public string? BrandName { get; set; }
        public string? Description { get; set; }
        public string? FabricName { get; set; }
        public bool IsInWishlist { get; set; }
        public int? CategoryId { get; set; }
        public int? OfferId { get; set; }
        public string? CreatedAt { get; set; }
        public Gender Gender { get; set; }
        public string CategoryName { get; set; }
        public bool? IsCategoryEnabled { get; set; }
        public string? MaterialComposition { get; set; }
        public string? CareInstructions { get; set; }
        public Season? Season { get; set; }
        public Occasion? Occasion { get; set; }
        public bool IsReturnable { get; set; }
        public int ReturnDays { get; set; } = 7;
        public decimal? Rating { get; set; }
        public bool IsVisible { get; set; }
        public ProductImageDto CoverImage { get; set; }
        public List<ProductImageDto> Images { get; set; }
        public List<ProductVariantDto> Variants { get; set; }
    }

    public class ProductImageDto
    {
        public int? ImageId { get; set; }
        public string? ImageUrl { get; set; }
        public string? ColorName { get; set; }
    }

    public class ProductVariantDto
    {
        public int ProductVariantId { get; set; }
        public string ProductSize { get; set; }
        public int StockQuantity { get; set; }
        public int ColorId { get; set; }
        public string ColorName { get; set; }
        public string HexCode { get; set; }
        public decimal? DiscountPercentage { get; set; }

        public decimal Price { get; set; }
        public decimal? TaxPercentage { get; set; }

        public decimal FinalPrice { get; set; }
    }

    public class OrderSummaryDto
    {
        public decimal ProductTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal OfferDiscount { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal GrandTotal { get; set; }
    }

    public class OfferDto
    {
        public string OfferName { get; set; }
        public string OfferDescription { get; set; }
        public IFormFile OfferBanner { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? FlatDiscountAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class UpdateOfferDto
    {
        public string? OfferName { get; set; }
        public string? OfferDescription { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public IFormFile? OfferBanner { get; set; }
        public decimal? FlatDiscountAmount { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime StartDate { get; set; }
    }

    public class ProductSearchParameters
    {
        public string? SearchTerm { get; set; }
        public string? CategoryName { get; set; }
        public string? BrandName { get; set; }
        public Season? Season { get; set; }
        public Occasion? Occasion { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 15;
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; }
    }
}
