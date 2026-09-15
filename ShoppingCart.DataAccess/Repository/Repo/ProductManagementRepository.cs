using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ShoppingCart.DataAccess.Data;
using ShoppingCart.DataAccess.Entity;
using ShoppingCart.DataAccess.Repository.IRepo;
using ShoppingCart.Utility.DTOs;
using ShoppingCart.Utility.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DataAccess.Repository.Repo
{
    public class ProductManagementRepository : IProductManagementRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ILogger _logger;

        public ProductManagementRepository(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IEmailService emailService,
            IWebHostEnvironment hostEnvironment,
            ILogger logger)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
            _hostEnvironment = hostEnvironment;
            _logger = logger;
        }

        public Task<Product> AddProductAsync(CreateProductDto productDto, string vendorId)
        {
            throw new NotImplementedException();
        }

        public Task<ReturnMessageDto> CreateRequestAsync(ProductAvailabilityRequestDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<ReturnMessageDto> DeleteProductAsync(int productId)
        {
            throw new NotImplementedException();
        }

        public ReturnMessageDto DeleteProductColorAsync(int productId, int colorId)
        {
            throw new NotImplementedException();
        }

        public ReturnMessageDto DisableProductAsync(int productId)
        {
            throw new NotImplementedException();
        }

        public ReturnMessageDto EnableProductAsync(int productId)
        {
            throw new NotImplementedException();
        }

        public Task<(List<ProductDetailsDto> Products, int TotalCount)> GetAllProductsAsync(int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<(List<ProductDetailsDto> Products, int TotalCount)> GetAllProductsByOfferAsync(int OfferId, int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<DashboardDto> GetDashboardDataAsync()
        {
            throw new NotImplementedException();
        }

        public Task<(List<ProductDetailsDto> Products, int TotalCount)> GetFilteredProductsAsync(int pageNumber = 1, int pageSize = 15, List<string> categories = null, List<string> genders = null, List<string> colors = null, List<string> sizes = null, List<string> fabrics = null, decimal? minPrice = null, decimal? maxPrice = null, string searchTerm = null, bool? isVisible = null, bool isAdmin = false, bool? isCategoryEnabled = null)
        {
            throw new NotImplementedException();
        }

        public Task<Color> GetOrCreateColorAsync(string colorName, string Hexcode)
        {
            throw new NotImplementedException();
        }

        public Task<Fabric> GetOrCreateFabricAsync(string fabricName)
        {
            throw new NotImplementedException();
        }

        public Task<ProductDetailsDto> GetProductDetailsAsync(int productId, string userId)
        {
            throw new NotImplementedException();
        }

        public Task<DailyReportDto> GetReportAsync(DateTime? startDate = null, DateTime? endDate = null, int? month = null, int? year = null)
        {
            throw new NotImplementedException();
        }

        public Task<List<string>> GetSearchSuggestionsAsync(string searchTerm)
        {
            throw new NotImplementedException();
        }

        public Task<ReturnMessageDto> UpdateProductAsync(int productId, CreateProductDto productDto, string vendorId)
        {
            throw new NotImplementedException();
        }
    }
}
