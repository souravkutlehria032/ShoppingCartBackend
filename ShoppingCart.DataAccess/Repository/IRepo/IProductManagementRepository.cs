using ShoppingCart.DataAccess.Entity;
using ShoppingCart.Utility.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DataAccess.Repository.IRepo
{
    public interface IProductManagementRepository
    {
        #region ProductAddition

        Task<Product> AddProductAsync(CreateProductDto productDto, string vendorId);

        //Task<Product> UpdateProductAsync(int productId, CreateProductDto productDto, string vendorId);

        Task<ReturnMessageDto> UpdateProductAsync(
            int productId,
            CreateProductDto productDto,
            string vendorId);

        Task<Fabric> GetOrCreateFabricAsync(string fabricName);

        Task<Color> GetOrCreateColorAsync(string colorName, string Hexcode);

        ReturnMessageDto DisableProductAsync(int productId);

        ReturnMessageDto EnableProductAsync(int productId);

        ReturnMessageDto DeleteProductColorAsync(int productId, int colorId);

        Task<ProductDetailsDto> GetProductDetailsAsync(int productId, string userId);

        Task<List<string>> GetSearchSuggestionsAsync(string searchTerm);

        Task<ReturnMessageDto> CreateRequestAsync(ProductAvailabilityRequestDto dto);

        Task<(List<ProductDetailsDto> Products, int TotalCount)>
            GetAllProductsAsync(int pageNumber, int pageSize);

        Task<(List<ProductDetailsDto> Products, int TotalCount)>
            GetFilteredProductsAsync(
                int pageNumber = 1,
                int pageSize = 15,
                List<string> categories = null,
                List<string> genders = null,
                List<string> colors = null,
                List<string> sizes = null,
                List<string> fabrics = null,
                decimal? minPrice = null,
                decimal? maxPrice = null,
                string searchTerm = null,
                bool? isVisible = null,
                bool isAdmin = false,
                bool? isCategoryEnabled = null);

        Task<(List<ProductDetailsDto> Products, int TotalCount)>
            GetAllProductsByOfferAsync(int OfferId, int pageNumber, int pageSize);

        Task<ReturnMessageDto> DeleteProductAsync(int productId);

        #endregion

        #region Dashboard

        Task<DashboardDto> GetDashboardDataAsync();

        Task<DailyReportDto> GetReportAsync(
            DateTime? startDate = null,
            DateTime? endDate = null,
            int? month = null,
            int? year = null);

        #endregion
    }
}
