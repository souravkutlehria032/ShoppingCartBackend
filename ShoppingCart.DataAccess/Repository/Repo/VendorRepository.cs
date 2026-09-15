using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
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
    public class VendorRepository : IVendorRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IEmailService _emailService;

        public VendorRepository(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IEmailService emailService,
            IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
            _hostEnvironment = hostEnvironment;
        }

        public Task<Notification> AddAsync(Notification notification, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<ReturnMessageDto> AddVendorAddress(VendorAddressDto userDto, string userId)
        {
            throw new NotImplementedException();
        }

        public Task<ReturnMessageDto> CreateCategoryAsync(AddCategoryDto createCategoryDto)
        {
            throw new NotImplementedException();
        }

        public Task<ReturnMessageDto> CreateOfferAsync(OfferDto offerDto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public ReturnMessageDto DeleteCategory(int categoryId)
        {
            throw new NotImplementedException();
        }

        public ReturnMessageDto DeleteOffer(int offerId)
        {
            throw new NotImplementedException();
        }

        public Task<ReturnMessageDto> DeleteReplyAsync(int replyId, bool isAdmin)
        {
            throw new NotImplementedException();
        }

        public Task<ReturnMessageDto> DeleteReviewAsync(int reviewId, bool isAdmin)
        {
            throw new NotImplementedException();
        }

        public ReturnMessageDto DisableCategory(int categoryId)
        {
            throw new NotImplementedException();
        }

        public ReturnMessageDto EnableCategory(int categoryId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Category>> GetAllCategoriesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<Offer>> GetAllOffersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<(List<ProductDetailsDto> Products, int TotalCount)> GetAllProductsForAdminAsync(int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<List<ProductReviewWithRepliesDto>> GetAllRepliesWithDetailsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<ProductSize>> GetAllSizesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Notification?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public void GetCategoryByIdAsync(int categoryId)
        {
            throw new NotImplementedException();
        }

        public Task<(IEnumerable<Notification> Items, int TotalCount, int UnreadCount)> GetForAdminAsync(int pageNumber, int pageSize, bool? isRead = null, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public GetVendorAddressDto GetVendorAddressById(string vendorId)
        {
            throw new NotImplementedException();
        }

        public Task<int> MarkAllAsReadForAdminAsync(CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> MarkAsReadAsync(int id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<ReturnMessageDto> UpdateCategoryAsync(int categoryId, UpdateCategoryDto updateCategoryDto)
        {
            throw new NotImplementedException();
        }

        public Task<ReturnMessageDto> UpdateOfferAsync(int offerId, UpdateOfferDto offerDto)
        {
            throw new NotImplementedException();
        }
    }
}
