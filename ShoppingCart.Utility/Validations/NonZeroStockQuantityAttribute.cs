using ShoppingCart.Utility.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Utility.Validations
{
    public class NonZeroStockQuantityAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(
            object value,
            ValidationContext validationContext)
        {
            var productVariants = value as List<CreateProductVariantDto>;

            if (productVariants == null || !productVariants.Any())
            {
                return new ValidationResult(
                    "Product must have at least one variant with stock.");
            }

            foreach (var variant in productVariants)
            {
                foreach (var sizeQuantity in variant.SizesAndQuantities)
                {
                    if (sizeQuantity.StockQuantity <= 0)
                    {
                        return new ValidationResult(
                            "Stock quantity must be greater than zero for each color and size.");
                    }
                }
            }

            return ValidationResult.Success;
        }
    }
}
