using ShoppingCart.DataAccess.Entity;
using ShoppingCart.Utility.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DataAccess.Repository.IRepo
{
    public interface IVendorRepository
    {
        #region CategoriesSection

        Task<ReturnMessageDto> CreateCategoryAsync(AddCategoryDto createCategoryDto);

        void GetCategoryByIdAsync(int categoryId);

        Task<List<Category>> GetAllCategoriesAsync();

        Task<ReturnMessageDto> UpdateCategoryAsync(
            int categoryId,
            UpdateCategoryDto updateCategoryDto);

        ReturnMessageDto DisableCategory(int categoryId);

        ReturnMessageDto DeleteCategory(int categoryId);

        ReturnMessageDto EnableCategory(int categoryId);

        #endregion

        #region Vendor Address

        Task<ReturnMessageDto> AddVendorAddress(
            VendorAddressDto userDto,
            string userId);

        GetVendorAddressDto GetVendorAddressById(string vendorId);

        #endregion

        #region Product

        Task<(List<ProductDetailsDto> Products, int TotalCount)>
            GetAllProductsForAdminAsync(int pageNumber, int pageSize);

        Task<List<ProductSize>> GetAllSizesAsync();

        #endregion

        #region Offers

        Task<ReturnMessageDto> CreateOfferAsync(OfferDto offerDto);

        Task<List<Offer>> GetAllOffersAsync();

        Task<ReturnMessageDto> UpdateOfferAsync(
            int offerId,
            UpdateOfferDto offerDto);

        ReturnMessageDto DeleteOffer(int offerId);

        #endregion

        #region Review

        Task<List<ProductReviewWithRepliesDto>> GetAllRepliesWithDetailsAsync();

        Task<ReturnMessageDto> DeleteReviewAsync(int reviewId, bool isAdmin);

        Task<ReturnMessageDto> DeleteReplyAsync(int replyId, bool isAdmin);

        #endregion

        #region notifcation

        Task<Notification> AddAsync(
            Notification notification,
            CancellationToken ct = default);

        Task<(IEnumerable<Notification> Items, int TotalCount, int UnreadCount)>
            GetForAdminAsync(
                int pageNumber,
                int pageSize,
                bool? isRead = null,
                CancellationToken ct = default);

        Task<Notification?> GetByIdAsync(
            int id,
            CancellationToken ct = default);

        Task<bool> MarkAsReadAsync(
            int id,
            CancellationToken ct = default);

        Task<int> MarkAllAsReadForAdminAsync(
            CancellationToken ct = default);

        Task<bool> DeleteAsync(
            int id,
            CancellationToken ct = default);

        #endregion
    }
}
