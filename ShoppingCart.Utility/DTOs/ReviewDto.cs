using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Utility.DTOs
{
    public class AddProductReviewDto
    {
        [Required]
        public int ProductId { get; set; }

        public int ProductVariantId { get; set; }

        [Range(1, 5)]
        public decimal RatingValue { get; set; }

        [MaxLength(1000)]
        public string Review { get; set; }

        public List<IFormFile>? Images { get; set; }
    }

    public class UpdateProductReviewDto
    {
        [Range(1, 5)]
        public decimal? RatingValue { get; set; }

        [MaxLength(1000)]
        public string? Review { get; set; }

        public List<IFormFile>? Images { get; set; }
        public List<string>? RemovedImages { get; set; }
    }

    public class ProductReviewDto
    {
        public int Id { get; set; }
        public decimal RatingValue { get; set; }
        public string Review { get; set; }
        public string UserName { get; set; }
        public DateTime RatedAt { get; set; }
        public Guid UserId { get; set; }

        public List<string> ImageUrls { get; set; }

        public List<ProductReviewReplyDto> Replies { get; set; } = new();
    }

    public class ProductReviewReplyAdminDto
    {
        public int ReplyId { get; set; }
        public string ReplyText { get; set; }
        public DateTime? RepliedAt { get; set; }
        public string ReplyByUserName { get; set; }

        public int ReviewId { get; set; }
        public string ReviewText { get; set; }
        public decimal Rating { get; set; }
        public DateTime ReviewedAt { get; set; }
        public string ReviewByUserName { get; set; }

        public string Productimg { get; set; }
        public string ProductName { get; set; }
    }

    public class AddProductReviewReplyDto
    {
        public int ProductRatingId { get; set; }
        public string ReplyText { get; set; }
    }

    public class UpdateProductReviewReplyDto
    {
        public int Id { get; set; }
        public string ReplyText { get; set; }
    }

    public class ProductReviewReplyDto
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string ReplyText { get; set; }
        public DateTime? RepliedAt { get; set; }
    }

    public class ProductReviewWithRepliesDto
    {
        public int ReviewId { get; set; }
        public string ReviewText { get; set; }
        public decimal Rating { get; set; }
        public DateTime ReviewedAt { get; set; }
        public string ReviewByUserName { get; set; }

        public string ProductName { get; set; }
        public string Productimg { get; set; }

        public List<ProductReviewReplyDto> Replies { get; set; }
    }
}
