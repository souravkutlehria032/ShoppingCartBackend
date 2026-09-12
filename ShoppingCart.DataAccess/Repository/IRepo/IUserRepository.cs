using ShoppingCart.DataAccess.Entity;
using ShoppingCart.Utility.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DataAccess.Repository.IRepo
{
    public interface IUserRepository
    {
        Task<ReturnMessageDto> UpdateUser(UserDto userDto);
        Task<ApplicationUser> GetUserByIdAsync(string userId);
        Task<ReturnMessageDto> AddUserAddress(AddressDto userDto, string userId);

        Task<ReturnMessageDto> UpdateUserAddress(AddressDto userDto, string userId, int addressId);
        Task<List<UserAddress>> GetUserAddress(string userId);
        Task<List<UserDto>> GetAllUsersAsync();
        ReturnMessageDto DeleteAddress(int addressId, string userId);
        Task<ReturnMessageDto> CartItem(CartItemDto cartItemDto);
        Task<CartDetailsDto?> GetCartDetailsAsync(Guid userId);
        Task<ReturnMessageDto> RemoveItemFromCartAsync(Guid userId, int productId, int variantId);
        Task<bool> LockUserAsync(string userId);
        Task<bool> UnlockUserAsync(string userId);
        Task<List<Color>> GetAllColorAsync();
        Task<List<Fabric>> GetAllFabricAsync();
        Task<List<ProductSize>> GetAllSizesAsync();
        Task<(decimal MinPrice, decimal MaxPrice)> GetPriceRangeAsync();
        Task LogVisitAsync(string ipAddress);
        Task<int> GetTotalVisitsAsync();
        Task<List<Category>> GetAllCategoriesAsync(bool isAdmin);

        Task<bool> AddToWishlist(string userId, int productId, int? variantId);
        Task<bool> RemoveFromWishlist(string userId, int productId, int? variantId);
        Task<List<WishlistResponse>> GetUserWishlist(string userId);
        Task<bool> IsInWishlist(string userId, int productId, int? variantId);

        // Reviews
        Task<ProductRating> AddReviewAsync(AddProductReviewDto dto, Guid userId);
        Task<bool> UpdateReviewAsync(int id, UpdateProductReviewDto dto, Guid userId);
        Task<bool> DeleteReviewAsync(int id, Guid userId, bool isAdmin = false);
        Task<List<ProductReviewDto>> GetReviewsForProductAsync(int productId);

        // Review Replies
        Task AddReplyAsync(ProductReviewReply reply);
        Task<List<ProductReviewReply>> GetRepliesByReviewIdAsync(int reviewId);
        Task<ProductReviewReply> GetReplyByIdAsync(int replyId);
        Task<bool> UpdateReplyAsync(UpdateProductReviewReplyDto dto, Guid userId);
        Task<bool> DeleteReplyAsync(int replyId, Guid userId, bool isAdmin);

        Task<ReturnMessageDto> AddSubscriberAsync(string email, int? variantId);
        Task<List<NewsletterSubscriber>> GetAllSubscribersAsync();

        Task<ContactUsDto> AddContactAsync(ContactUsDto dto);
        Task<IEnumerable<ContactUsDto>> GetAllContactsAsync();
        Task<IEnumerable<ContactUsDto>> GetcontactByUserIdAsync(string userId);

        Task<ContactReplyDto> AddReplyAsync(int contactId, string replyMessage, string adminUser);
        Task<IEnumerable<ContactReplyDto>> GetRepliesByContactIdAsync(int contactId);
        Task<IEnumerable<ContactWithRepliesDto>> GetAllContactsWithRepliesAsync();
    }
}
