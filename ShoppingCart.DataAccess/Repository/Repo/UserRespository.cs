using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using QuestPDF.Infrastructure;
using ShoppingCart.DataAccess.BackgroundService;
using ShoppingCart.DataAccess.Data;
using ShoppingCart.DataAccess.Entity;
using ShoppingCart.DataAccess.Repository.IRepo;
using ShoppingCart.Utility.DTOs;
using ShoppingCart.Utility.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ShoppingCart.Utility.Enums.Enums;

namespace ShoppingCart.DataAccess.Repository.Repo
{
    public class UserRespository : IUserRepository
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly IHubContext<NotificationHub> _hubContext;

        public UserRespository(
            IWebHostEnvironment hostEnvironment,
            UserManager<ApplicationUser> userManager,
            IHubContext<NotificationHub> hubContext,
            IEmailService emailService,
            IConfiguration configuration,
            ApplicationDbContext dbcontext)
        {
            _hostEnvironment = hostEnvironment;
            _userManager = userManager;
            _emailService = emailService;
            _dbcontext = dbcontext;
            _hubContext = hubContext;
            _configuration = configuration;
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<ContactUsDto> AddContactAsync(ContactUsDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            try
            {
                if (string.IsNullOrWhiteSpace(dto.Name))
                    throw new ArgumentException("Name is required");

                if (string.IsNullOrWhiteSpace(dto.Email))
                    throw new ArgumentException("Email is required");

                if (string.IsNullOrWhiteSpace(dto.Subject))
                    throw new ArgumentException("Subject is required");

                if (string.IsNullOrWhiteSpace(dto.Message))
                    throw new ArgumentException("Message is required");

                var entity = new Contact
                {
                    UserId = dto.UserId,
                    Name = dto.Name,
                    Email = dto.Email,
                    Subject = dto.Subject,
                    Message = dto.Message,
                    CreatedAt = DateTime.UtcNow
                };

                await _dbcontext.Contact.AddAsync(entity);
                await _dbcontext.SaveChangesAsync();

                var notification = new Notification
                {
                    RecipientRole = "Admin",
                    Reason = $"New contact message from {dto.Name} related to {dto.Subject}",
                    RequestedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                };

                _dbcontext.Notification.Add(notification);
                await _dbcontext.SaveChangesAsync();

                await _hubContext.Clients.Group("Admin")
                    .SendAsync("ReceiveContactMessage", new
                    {
                        id = notification.Id,
                        name = entity.Name,
                        email = entity.Email,
                        subject = entity.Subject,
                        message = entity.Message,
                        createdAt = entity.CreatedAt,
                        isRead = notification.IsRead
                    });

                return new ContactUsDto
                {
                    Id = entity.Id,
                    UserId = entity.UserId,
                    Name = entity.Name,
                    Email = entity.Email,
                    Subject = entity.Subject,
                    Message = entity.Message,
                    CreatedAt = entity.CreatedAt
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public async Task AddReplyAsync(ProductReviewReply reply)
        {
            var review = await _dbcontext.ProductRating
                .FirstOrDefaultAsync(r => r.Id == reply.ProductRatingId);

            if (review == null)
                throw new ArgumentException("Review not found.");

            await _dbcontext.ProductReviewReply.AddAsync(reply);
            await _dbcontext.SaveChangesAsync();
        }

        private string GenerateContactReplyEmailBody(string userName, string subject, string originalMessage, string replyMessage)
        {
            return $@"
<div style='font-family: Poppins, Arial, sans-serif; background: #f0f8ff; padding: 20px; border-radius: 8px; max-width: 600px; margin: auto; box-shadow: 0 4px 12px rgba(0,0,0,0.1);'>
    <h2 style='color: #1e3a8a;'>📩 Reply to Your Contact Request</h2>

    <p style='font-size: 16px; color: #333;'>
        Dear <strong>{userName}</strong>,
    </p>

    <p style='font-size: 14px; color: #333;'>
        Thank you for reaching out regarding <strong>{subject}</strong>. Below is our response to your message:
    </p>

    <div style='background:#f9fafb; padding: 12px; border-left: 4px solid #3b82f6; margin-bottom: 16px;'>
        <p style='margin:0; font-size: 14px;'><em>Your message:</em></p>
        <p style='margin:0; font-size: 14px; color:#555;'>{originalMessage}</p>
    </div>

    <div style='background:#ecfdf5; padding: 12px; border-left: 4px solid #10b981;'>
        <p style='margin:0; font-size: 14px;'><em>Our reply:</em></p>
        <p style='margin:0; font-size: 14px; color:#333;'>{replyMessage}</p>
    </div>

    <p style='font-size: 14px; color: #333; margin-top: 16px;'>
        If you have any further questions, feel free to reply to this email.
    </p>

    <hr style='border:none; border-top: 1px solid #eee; margin:20px 0;'/>

    <p style='font-size: 12px; color: #999;'>
        This is an automated message from our support system. Please do not reply directly to this email.
    </p>
</div>";
        }

        public async Task<ContactReplyDto> AddReplyAsync(int contactId, string replyMessage, string adminUser)
        {
            var contact = await _dbcontext.Contact.FindAsync(contactId);

            if (contact == null)
                throw new ArgumentException("Contact message not found");

            var reply = new ContactReply
            {
                ContactMessageId = contactId,
                ReplyMessage = replyMessage,
                RepliedBy = adminUser,
                RepliedAt = DateTime.UtcNow
            };

            await _dbcontext.ContactReply.AddAsync(reply);

            contact.IsReplied = true;
            _dbcontext.Contact.Update(contact);

            await _dbcontext.SaveChangesAsync();

            _ = Task.Run(async () =>
            {
                try
                {
                    string emailBody = GenerateContactReplyEmailBody(
                        contact.Name,
                        contact.Subject,
                        contact.Message,
                        reply.ReplyMessage);

                    await _emailService.SendEmailAsync(
                        contact.Email,
                        $"Reply: {contact.Subject}",
                        emailBody);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Email sending failed: {ex.Message}");
                }
            });

            return new ContactReplyDto
            {
                Id = reply.Id,
                ContactId = contactId,
                ReplyMessage = reply.ReplyMessage,
                RepliedBy = reply.RepliedBy,
                RepliedAt = reply.RepliedAt
            };
        }

        private async Task<bool> HasUserPurchasedProductAsync(Guid userId, int productId)
        {
            return await _dbcontext.OrderItem
                .Where(oi =>
                    oi.Order.UserId == userId &&
                    oi.Order.Status == OrderStatus.Delivered &&
                    oi.ProductVariant.ProductId == productId)
                .AnyAsync();
        }

        private async Task<string> SaveReviewImageAsync(IFormFile imageFile)
        {
            if (imageFile == null)
                return null;

            string uploadsFolder = Path.Combine(
                _hostEnvironment.WebRootPath,
                "ReviewImages");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
            string filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return Path.Combine("ReviewImages", fileName);
        }

        public async Task<ProductRating> AddReviewAsync(AddProductReviewDto dto, Guid userId)
        {
            var productExists = await _dbcontext.Product
                .AnyAsync(p => p.ProductId == dto.ProductId);

            if (!productExists)
                throw new ArgumentException("Producet not found.");

            var alreadyReviewed = await _dbcontext.ProductRating
                .AnyAsync(r =>
                    r.ProductId == dto.ProductId &&
                    r.UserId == userId);

            if (alreadyReviewed)
                throw new InvalidOperationException(
                    "You have already reviewed this product.");

            var hasPurchased = await HasUserPurchasedProductAsync(
                userId,
                dto.ProductId);

            if (!hasPurchased)
                throw new UnauthorizedAccessException(
                    "You   only review products that have been delivered.");

            var rating = new ProductRating
            {
                ProductId = dto.ProductId,
                UserId = userId,
                RatingValue = dto.RatingValue,
                Review = dto.Review,
                RatedAt = DateTime.UtcNow
            };

            _dbcontext.ProductRating.Add(rating);
            await _dbcontext.SaveChangesAsync();

            if (dto.Images != null && dto.Images.Any())
            {
                var reviewImages = new List<ReviewImage>();

                foreach (var image in dto.Images)
                {
                    var imagePath = await SaveReviewImageAsync(image);

                    if (imagePath != null)
                    {
                        reviewImages.Add(new ReviewImage
                        {
                            ProductRatingId = rating.Id,
                            ImageUrl = imagePath
                        });
                    }
                }

                if (reviewImages.Any())
                {
                    _dbcontext.ReviewImage.AddRange(reviewImages);
                    await _dbcontext.SaveChangesAsync();
                }
            }

            return rating;
        }

        public async Task<ReturnMessageDto> AddSubscriberAsync(
    string email,
    int? variantId)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return new ReturnMessageDto
                {
                    Succeeded = false,
                    Message = "Email is required."
                };
            }

            email = email.Trim().ToLowerInvariant();

            try
            {
                var exists = await _dbcontext.NewsletterSubscribers
                    .AnyAsync(x => x.Email == email);

                if (exists)
                {
                    return new ReturnMessageDto
                    {
                        Succeeded = false,
                        Message =
                            "You are already subscribed to our newsletter."
                    };
                }

                _dbcontext.NewsletterSubscribers.Add(
                    new NewsletterSubscriber
                    {
                        Email = email,
                        ProductVariantId = variantId
                    });

                await _dbcontext.SaveChangesAsync();

                string body = GenerateNewsletterEmailBody();

                await _emailService.SendEmailAsync(
                    email,
                    "🎉 Welcome to Kids Shopping Newsletter",
                    body);

                return new ReturnMessageDto
                {
                    Succeeded = true,
                    Message = "Subscribed successfully!"
                };
            }
            catch (Exception ex)
            {
                return new ReturnMessageDto
                {
                    Succeeded = false,
                    Message =
                        "Failed to subscribe to newsletter. " +
                        ex.Message
                };
            }
        }
        private string GenerateNewsletterEmailBody()
        {
            return @"
<div style='font-family: Poppins, Arial, sans-serif; background: linear-gradient(to right, #e3f2fd, #ffffff); padding: 30px; border-radius: 12px; max-width: 650px; margin: auto; box-shadow: 0 4px 12px rgba(0,0,0,0.1);'>

    <div style='text-align: center; margin-bottom: 25px;'>
        <img src='cid:AppLogo' alt='Kids Shopping Logo' style='width: 100px; border-radius: 8px;' />
    </div>

    <h2 style='text-align: center; color: #1976d2;'>Thanks for Subscribing! 🎉</h2>

    <p style='font-size: 16px; color: #555; text-align: center; max-width: 90%; margin: auto;'>
        You're now part of the <strong>Kids Shopping</strong> family. We’re excited to bring you the latest product updates, exclusive offers, and style tips for your little ones.
    </p>

    <div style='background: #e8f0fe; border-radius: 8px; padding: 20px; text-align: center; margin: 30px auto; max-width: 350px;'>
        <p style='font-size: 16px; color: #333;'>Stay tuned! New arrivals and discounts are on the way 👶🛒</p>
    </div>

    <hr style='margin: 30px 0; border: none; border-top: 1px solid #ccc;' />

    <p style='font-size: 14px; color: #555; text-align: center;'>
        If you have any questions or suggestions, feel free to reach out to us at
        <a href='mailto:support@kidsshopping.com' style='color: #1976d2;'>support@kidsshopping.com</a>.
    </p>

    <p style='font-size: 14px; color: #777; text-align: center; margin-top: 20px;'>
        Happy Shopping!<br />
        <strong>The Kids Shopping Team 👕👗</strong>
    </p>

</div>";
        }

        public async Task<bool> AddToWishlist(string userId, int productId, int? variantId)
        {
            var productExists = await _dbcontext.Product
                .AnyAsync(p => p.ProductId == productId);

            if (!productExists)
                return false;

            if (variantId.HasValue)
            {
                var variantExists = await _dbcontext.ProductVariant
                    .AnyAsync(v =>
                        v.ProductVariantId == variantId &&
                        v.ProductId == productId);

                if (!variantExists)
                    return false;
            }

            var existingItem = await _dbcontext.WishlistItems
                .FirstOrDefaultAsync(w =>
                    w.UserId == userId &&
                    w.ProductId == productId &&
                    w.ProductVariantId == variantId);

            if (existingItem != null)
                return true;

            var wishlistItem = new WishlistItem
            {
                UserId = userId,
                ProductId = productId,
                ProductVariantId = variantId,
                IsInWishlist = true,
                AddedDate = DateTime.UtcNow
            };

            _dbcontext.WishlistItems.Add(wishlistItem);
            await _dbcontext.SaveChangesAsync();

            return true;
        }

        public async Task<ReturnMessageDto> AddUserAddress(AddressDto userDto, string userId)
        {
            var findUser = await _userManager.FindByIdAsync(userId);
            if (findUser == null)
            {
                return new ReturnMessageDto
                {
                    Succeeded = false,
                    Message = "Something went wrong. Please try again later"
                };
            }

            // If the new address is set as default, clear other defaults
            if (userDto.IsDefault)
            {
                var existingDefaultAddresses = _dbcontext.UserAddress
                    .Where(x => x.UserId == Guid.Parse(userId) && x.IsDefault)
                    .ToList();

                foreach (var address in existingDefaultAddresses)
                {
                    address.IsDefault = false;
                }
            }

            var userAddress = new UserAddress
            {
                UserId = Guid.Parse(userId),
                City = userDto.City,
                State = userDto.State,
                PostCode = userDto.PostCode,
                AreaandStreet = userDto.AreaandStreet,
                Country = userDto.Country,
                PhoneNumber = userDto.PhoneNumber,
                AlternatePhoneNumber = userDto.AlternatePhoneNumber,
                Landmark = userDto.Landmark,
                IsDefault = userDto.IsDefault
            };

            _dbcontext.UserAddress.Add(userAddress);
            await _dbcontext.SaveChangesAsync();

            return new ReturnMessageDto
            {
                Succeeded = true,
                Message = "Address added successfully"
            };
        }

        public async Task<ReturnMessageDto> CartItem(CartItemDto cartItemDto)
        {
            var findProduct = _dbcontext.Product.Find(cartItemDto.ProductId);

            if (findProduct == null)
                return new ReturnMessageDto
                {
                    Succeeded = false,
                    Message = "Product not found"
                };

            var findUser = await _userManager.FindByIdAsync(cartItemDto.UserId.ToString());

            if (findUser == null)
                return new ReturnMessageDto
                {
                    Succeeded = false,
                    Message = "User not found"
                };

            var findProductVariant = _dbcontext.ProductVariant
                .FirstOrDefault(c =>
                    c.ProductVariantId == cartItemDto.VariantId &&
                    c.ProductId == cartItemDto.ProductId);

            if (findProductVariant == null)
                return new ReturnMessageDto
                {
                    Succeeded = false,
                    Message = "Product variant not found"
                };

            var findUserCart = _dbcontext.Cart
                .FirstOrDefault(x => x.UserId == cartItemDto.UserId);

            if (findUserCart == null)
            {
                var newCart = new Cart
                {
                    UserId = cartItemDto.UserId
                };

                _dbcontext.Cart.Add(newCart);
                await _dbcontext.SaveChangesAsync();

                findUserCart = newCart;
            }

            var findCartItem = await _dbcontext.CartItem
                .FirstOrDefaultAsync(x =>
                    x.ProductVariantId == cartItemDto.VariantId &&
                    x.CartId == findUserCart.CartId);

            if (findCartItem != null)
            {
                bool isUpdateFromCart = cartItemDto.IsUpdateFromCart;

                int newQuantity = isUpdateFromCart
                    ? cartItemDto.Quantity
                    : findCartItem.Quantity + cartItemDto.Quantity;

                if (newQuantity > findProductVariant.StockQuantity)
                {
                    return new ReturnMessageDto
                    {
                        Succeeded = false,
                        Message = isUpdateFromCart
                            ? $"Only {findProductVariant.StockQuantity} items available in stock"
                            : $"Only {findProductVariant.StockQuantity - findCartItem.Quantity} more items available in stock"
                    };
                }

                findCartItem.Quantity = newQuantity;

                await _dbcontext.SaveChangesAsync();

                return new ReturnMessageDto
                {
                    Succeeded = true,
                    Message = isUpdateFromCart
                        ? "Cart item quantity updated"
                        : "Cart item quantity increased"
                };
            }

            if (cartItemDto.Quantity > findProductVariant.StockQuantity)
            {
                return new ReturnMessageDto
                {
                    Succeeded = false,
                    Message = $"Only {findProductVariant.StockQuantity} items available in stock"
                };
            }

            var cartItem = new CartItem
            {
                ProductId = cartItemDto.ProductId,
                Quantity = cartItemDto.Quantity,
                ProductVariantId = cartItemDto.VariantId,
                CartId = findUserCart.CartId,
            };

            _dbcontext.CartItem.Add(cartItem);
            await _dbcontext.SaveChangesAsync();

            return new ReturnMessageDto
            {
                Succeeded = true,
                Message = "Item added to cart"
            };
        }

        public ReturnMessageDto DeleteAddress(int addressId, string userId)
        {
            var findaddress = _dbcontext.UserAddress.Find(addressId);

            if (findaddress == null)
            {
                return new ReturnMessageDto
                {
                    Succeeded = false,
                    Message = "Address not found"
                };
            }

            var hasActiveOrders = _dbcontext.Order.Any(o =>
                o.AddressId == addressId && o.Status != OrderStatus.Delivered);

            if (hasActiveOrders)
            {
                return new ReturnMessageDto
                {
                    Succeeded = false,
                    Message = "Address cannot be deleted because it is linked to an active order."
                };
            }

            if (findaddress.IsDefault)
            {
                var userAddresses = _dbcontext.UserAddress
                    .Where(x => x.UserId == Guid.Parse(userId)
                             && x.AddressId != addressId
                             && (x.IsDeleted ?? false) == false)
                    .ToList();

                if (userAddresses.Any())
                {
                    userAddresses.First().IsDefault = true;
                }
            }

            findaddress.IsDeleted = true;
            _dbcontext.SaveChanges();

            return new ReturnMessageDto
            {
                Succeeded = true,
                Message = "Address deleted successfully"
            };
        }

        public async Task<bool> DeleteReplyAsync(int replyId, Guid userId, bool isAdmin)
        {
            try
            {
                var reply = await _dbcontext.ProductReviewReply
                    .FirstOrDefaultAsync(r => r.Id == replyId);

                if (reply == null)
                    return false;

                if (reply.UserId != userId && !isAdmin)
                    throw new UnauthorizedAccessException(
                        "Unauthorized to delete this reply.");

                _dbcontext.ProductReviewReply.Remove(reply);
                await _dbcontext.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteReviewAsync(int id, Guid userId, bool isAdmin = false)
        {
            var review = await _dbcontext.ProductRating
                .Include(r => r.RatingImages)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null)
                return false;

            if (!isAdmin && review.UserId != userId)
                throw new UnauthorizedAccessException(
                    "Unauthorized to delete this review.");

            _dbcontext.ReviewImage.RemoveRange(review.RatingImages);
            _dbcontext.ProductRating.Remove(review);

            await _dbcontext.SaveChangesAsync();

            return true;
        }

        public async Task<List<Category>> GetAllCategoriesAsync(bool isAdmin)
        {
            var query = _dbcontext.Category.AsQueryable();

            if (!isAdmin)
            {
                // User sees only enabled categories
                query = query.Where(c => c.IsDeleted == false);
            }

            // Admin sees all categories (no filtering on IsDeleted)

            return await query.ToListAsync();
        }

        public async Task<List<Entity.Color>> GetAllColorAsync()
        {
            var colorsList = await _dbcontext.Color
                .ToListAsync();

            return colorsList;
        }

        public async Task<IEnumerable<ContactUsDto>> GetAllContactsAsync()
        {
            try
            {
                var entities = await _dbcontext.Contact
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();

                return entities.Select(c => new ContactUsDto
                {
                    Id = c.Id,
                    UserId = c.UserId,
                    Name = c.Name,
                    Email = c.Email,
                    Subject = c.Subject,
                    Message = c.Message,
                    CreatedAt = c.CreatedAt
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    "An error occurred while retrieving all contact messages.",
                    ex);
            }
        }

        public async Task<IEnumerable<ContactWithRepliesDto>> GetAllContactsWithRepliesAsync()
        {
            var contacts = await _dbcontext.Contact
                .Include(c => c.Replies)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return contacts.Select(c => new ContactWithRepliesDto
            {
                Id = c.Id,
                UserId = c.UserId,
                Name = c.Name,
                Email = c.Email,
                Subject = c.Subject,
                Message = c.Message,
                CreatedAt = c.CreatedAt,
                Replies = c.Replies.Select(r => new ContactReplyDto
                {
                    Id = r.Id,
                    ContactId = r.ContactMessageId,
                    ReplyMessage = r.ReplyMessage,
                    RepliedBy = r.RepliedBy,
                    RepliedAt = r.RepliedAt
                }).ToList()
            }).ToList();
        }

        public async Task<List<Fabric>> GetAllFabricAsync()
        {
            var fabricsList = await _dbcontext.Fabric
                 .ToListAsync();

            return fabricsList;
        }

        public async Task<List<ProductSize>> GetAllSizesAsync()
        {
            var sizeList = await _dbcontext.ProductSize.ToListAsync();
            return sizeList;
        }

        public async Task<List<NewsletterSubscriber>> GetAllSubscribersAsync()
        {
            return await _dbcontext.NewsletterSubscribers.ToListAsync();
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var usersInUserRole = await _userManager.GetUsersInRoleAsync("User");

            var userDtos = usersInUserRole.Select(u => new UserDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                ProfilePictureUrl = u.ProfilePictureUrl,
                IsActive = u.IsActive,
                IsSuspended = u.IsSuspended,
                IsDeleted = u.IsDeleted,
                AccessFailedCount = u.AccessFailedCount,
                LockoutEnabled = u.LockoutEnabled,
                LockoutEnd = u.LockoutEnd
            }).ToList();

            return userDtos;
        }

        public async Task<CartDetailsDto?> GetCartDetailsAsync(Guid userId)
        {
            var cart = await _dbcontext.Cart
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p.Category)
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.ProductVariant)
                        .ThenInclude(pv => pv.Color)
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.ProductVariant)
                        .ThenInclude(pv => pv.VariantImages)
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.ProductVariant)
                        .ThenInclude(pv => pv.ProductSize)
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p.Images)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return new CartDetailsDto
                {
                    UserId = userId,
                    CartItems = new List<CartItemsDto>()
                };
            }

            return new CartDetailsDto
            {
                CartId = cart.CartId,
                UserId = cart.UserId,
                CartItems = cart.CartItems.Select(ci => new CartItemsDto
                {
                    CartItemId = ci.Id,
                    ProductId = ci.ProductId,
                    ProductName = ci.Product?.BrandName ?? "N/A",
                    VariantId = ci.ProductVariantId,
                    OfferId = ci.Product.OfferId,
                    ProductImage = ci.Product?.Images?
                        .Where(img => img.ColorId == ci.ProductVariant.ColorId)
                        .Select(img => img.ImageUrl)
                        .FirstOrDefault() ?? string.Empty,
                    FinalPrice = ci.ProductVariant?.FinalPrice ?? 0,
                    Price = ci.ProductVariant.Price,
                    SelectedSize = ci.ProductVariant?.ProductSize?.SizeName ?? "Unknown",
                    SelectedColor = ci.ProductVariant?.Color?.ColorName ?? "N/A",
                    DiscountPercentage = ci.ProductVariant.DiscountPercentage ?? 0,
                    Quantity = ci.Quantity,
                    StockAvailability = ci.ProductVariant?.StockQuantity ?? 0,
                    Subtotal = (ci.ProductVariant?.FinalPrice ?? 0) * ci.Quantity,
                    TaxPercentage = ci.ProductVariant.TaxPercentage ?? 0,
                    IsAvailable = (ci.Product?.IsVisible == true) &&
                                  (ci.Product?.Category?.IsDeleted == false)
                }).ToList()
            };
        }

        public async Task<IEnumerable<ContactUsDto>> GetcontactByUserIdAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("UserId is required");

            try
            {
                var entities = await _dbcontext.Contact
                    .Where(c => c.UserId == userId)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();

                return entities.Select(c => new ContactUsDto
                {
                    Id = c.Id,
                    UserId = c.UserId,
                    Name = c.Name,
                    Email = c.Email,
                    Subject = c.Subject,
                    Message = c.Message,
                    CreatedAt = c.CreatedAt
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    $"An error occurred while retrieving contact messages for user {userId}.",
                    ex);
            }
        }

        public async Task<(decimal MinPrice, decimal MaxPrice)> GetPriceRangeAsync()
        {
            var minPrice = await _dbcontext.ProductVariant.MinAsync(p => p.Price);
            var maxPrice = await _dbcontext.ProductVariant.MaxAsync(p => p.Price);
            return (minPrice, maxPrice);
        }

        public async Task<IEnumerable<ContactReplyDto>> GetRepliesByContactIdAsync(int contactId)
        {
            var replies = await _dbcontext.ContactReply
                .Where(r => r.ContactMessageId == contactId)
                .OrderBy(r => r.RepliedAt)
                .ToListAsync();

            return replies.Select(r => new ContactReplyDto
            {
                Id = r.Id,
                ContactId = r.ContactMessageId,
                ReplyMessage = r.ReplyMessage,
                RepliedBy = r.RepliedBy,
                RepliedAt = r.RepliedAt
            }).ToList();
        }

        public async Task<List<ProductReviewReply>> GetRepliesByReviewIdAsync(int reviewId)
        {
            return await _dbcontext.ProductReviewReply
                .Where(r => r.ProductRatingId == reviewId)
                .Include(r => r.User)
                .ToListAsync();
        }

        public async Task<ProductReviewReply> GetReplyByIdAsync(int replyId)
        {
            return await _dbcontext.ProductReviewReply.Include(r => r.User).FirstOrDefaultAsync(r => r.Id == replyId);
        }

        public async Task<List<ProductReviewDto>> GetReviewsForProductAsync(int productId)
        {
            var reviews = await _dbcontext.ProductRating
                .Where(r =>
                    r.ProductId == productId &&
                    !string.IsNullOrWhiteSpace(r.Review))
                .Include(r => r.User)
                .Include(r => r.RatingImages)
                .OrderByDescending(r => r.RatedAt)
                .ToListAsync();

            return reviews.Select(r => new ProductReviewDto
            {
                Id = r.Id,
                RatingValue = r.RatingValue,
                Review = r.Review,
                RatedAt = r.RatedAt,
                UserName = r.User.FirstName + " " + r.User.LastName,
                UserId = r.UserId,
                ImageUrls = r.RatingImages
                    .Select(i => i.ImageUrl)
                    .ToList()
            }).ToList();
        }

        public async Task<int> GetTotalVisitsAsync()
        {
            return await _dbcontext.VisitLogs.CountAsync();
        }

        public async Task<List<UserAddress>> GetUserAddress(string userId)
        {
            var addressList = await _dbcontext.UserAddress.Where(x => x.UserId == Guid.Parse(userId) && x.IsDeleted == false).ToListAsync();
            if (addressList == null)
            {
                return null;
            }
            return addressList;
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            var findUser = await _userManager.FindByIdAsync(userId);

            if (findUser == null)
            {
                return null;
            }

            return findUser;
        }

        public async Task<List<WishlistResponse>> GetUserWishlist(string userId)
        {
            try
            {
                return await _dbcontext.WishlistItems
                    .Where(w => w.UserId == userId)
                    .Include(w => w.Product)
                        .ThenInclude(p => p.Images)
                    .Include(w => w.Product)
                        .ThenInclude(p => p.Variants)
                            .ThenInclude(v => v.Color)
                    .Include(w => w.Product)
                        .ThenInclude(p => p.Variants)
                            .ThenInclude(v => v.ProductSize)
                    .Include(w => w.ProductVariant)
                    .Select(w => new WishlistResponse
                    {
                        WishlistItemId = w.WishlistItemId,
                        ProductId = w.Product.ProductId,
                        ProductName = w.Product.Name,
                        VariantId = w.ProductVariant.ProductVariantId,
                        Price = w.ProductVariant != null
                            ? w.ProductVariant.Price
                            : w.Product.Variants.FirstOrDefault() != null
                                ? w.Product.Variants.FirstOrDefault().Price
                                : 0,
                        DiscountedPrice = w.ProductVariant != null
                            ? w.ProductVariant.FinalPrice
                            : w.Product.Variants.FirstOrDefault() != null
                                ? w.Product.Variants.FirstOrDefault().FinalPrice
                                : 0,
                        MainImageUrl = w.Product.Images.FirstOrDefault().ImageUrl,
                        IsInWishlist = w.IsInWishlist,
                        InStock = w.ProductVariant != null
                            ? w.ProductVariant.StockQuantity > 0
                            : w.Product.Variants.Any(v => v.StockQuantity > 0)
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetUserWishlist: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return new List<WishlistResponse>();
            }
        }

        public async Task<bool> IsInWishlist(string userId, int productId, int? variantId)
        {
            return await _dbcontext.WishlistItems.AnyAsync(w => w.UserId == userId && w.ProductId == productId && w.ProductVariantId == variantId);
        }

        public async Task<bool> LockUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return false;

            // Lock user for 7 days from now
            var lockoutEnd = DateTimeOffset.UtcNow.AddDays(7);

            // Enable lockout if not already enabled
            if (!await _userManager.GetLockoutEnabledAsync(user))
            {
                await _userManager.SetLockoutEnabledAsync(user, true);
            }

            // Set lockout end date
            var lockoutResult =
                await _userManager.SetLockoutEndDateAsync(user, lockoutEnd);

            // Reset failed count to prevent immediate auto-unlock
            await _userManager.ResetAccessFailedCountAsync(user);

            user.IsSuspended = true;
            user.IsActive = false;

            var updateResult = await _userManager.UpdateAsync(user);

            return lockoutResult.Succeeded && updateResult.Succeeded;
        }

        public async Task LogVisitAsync(string ipAddress)
        {
            var log = new Visitorlog
            {
                IpAddress = ipAddress,
                VisitTime = DateTime.UtcNow
            };

            _dbcontext.VisitLogs.Add(log);
            await _dbcontext.SaveChangesAsync();
        }

        public async Task<bool> RemoveFromWishlist(string userId, int productId, int? variantId)
        {
            WishlistItem item;

            if (variantId.HasValue)
            {
                item = await _dbcontext.WishlistItems.FirstOrDefaultAsync(w => w.UserId == userId && w.ProductVariantId == variantId.Value);
            }
            else
            {
                item = await _dbcontext.WishlistItems.FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);
            }

            if (item == null)
                return false;

            _dbcontext.WishlistItems.Remove(item);
            await _dbcontext.SaveChangesAsync();

            return true;
        }

        public async Task<ReturnMessageDto> RemoveItemFromCartAsync(Guid userId, int productId, int variantId)
        {
            var item = await _dbcontext.CartItem
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci =>
                    ci.Cart.UserId == userId &&
                    ci.ProductId == productId &&
                    ci.ProductVariantId == variantId);

            if (item == null)
            {
                return new ReturnMessageDto
                {
                    Succeeded = false,
                    Message = "Item not found in cart."
                };
            }

            _dbcontext.CartItem.Remove(item);
            await _dbcontext.SaveChangesAsync();

            return new ReturnMessageDto
            {
                Succeeded = true,
                Message = "Item removed successfully."
            };
        }

        public async Task<bool> UnlockUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return false;

            var unlockResult =
                await _userManager.SetLockoutEndDateAsync(user, null);

            var resetFailCountResult =
                await _userManager.ResetAccessFailedCountAsync(user);

            user.IsSuspended = false;
            user.IsActive = true;

            var updateResult = await _userManager.UpdateAsync(user);

            return unlockResult.Succeeded &&
                   resetFailCountResult.Succeeded &&
                   updateResult.Succeeded;
        }

        public async Task<bool> UpdateReplyAsync(UpdateProductReviewReplyDto dto, Guid userId)
        {
            try
            {
                var reply = await _dbcontext.ProductReviewReply
                    .FirstOrDefaultAsync(r => r.Id == dto.Id);

                if (reply == null)
                    return false;

                if (reply.UserId != userId)
                    throw new UnauthorizedAccessException(
                        "You are not authorized to update this reply.");

                reply.ReplyText = dto.ReplyText;
                reply.RepliedAt = DateTime.UtcNow;

                _dbcontext.ProductReviewReply.Update(reply);
                await _dbcontext.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateReviewAsync(int id, UpdateProductReviewDto dto, Guid userId)
        {
            var review = await _dbcontext.ProductRating
                .Include(r => r.RatingImages)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null)
                throw new KeyNotFoundException("Review not found.");

            if (review.UserId != userId)
                throw new UnauthorizedAccessException(
                    "You can only update your own review.");

            if (dto.RatingValue.HasValue)
                review.RatingValue = dto.RatingValue.Value;

            if (!string.IsNullOrWhiteSpace(dto.Review))
                review.Review = dto.Review;

            review.RatedAt = DateTime.UtcNow;

            if (dto.RemovedImages != null && dto.RemovedImages.Any())
            {
                var imagesToRemove = review.RatingImages
                    .Where(img => dto.RemovedImages
                        .Any(r => r.EndsWith(
                            img.ImageUrl,
                            StringComparison.OrdinalIgnoreCase)))
                    .ToList();

                if (imagesToRemove.Any())
                    _dbcontext.ReviewImage.RemoveRange(imagesToRemove);
            }

            if (dto.Images != null && dto.Images.Any())
            {
                var newImages = new List<ReviewImage>();

                foreach (var image in dto.Images)
                {
                    var imagePath = await SaveReviewImageAsync(image);

                    if (imagePath != null)
                    {
                        newImages.Add(new ReviewImage
                        {
                            ProductRatingId = review.Id,
                            ImageUrl = imagePath
                        });
                    }
                }

                if (newImages.Any())
                    _dbcontext.ReviewImage.AddRange(newImages);
            }

            await _dbcontext.SaveChangesAsync();

            return true;
        }

        public async Task<ReturnMessageDto> UpdateUser(UserDto userDto)
        {
            if (userDto == null)
            {
                return new ReturnMessageDto
                {
                    Succeeded = false,
                    Message = "Invalid data."
                };
            }

            var user = await _userManager.FindByEmailAsync(userDto.Email);

            if (user == null)
            {
                return new ReturnMessageDto
                {
                    Succeeded = false,
                    Message = "User not found."
                };
            }

            // Ensure "Images" directory exists
            string imagesFolderPath = Path.Combine(
                _hostEnvironment.WebRootPath,
                "Images");

            if (!Directory.Exists(imagesFolderPath))
            {
                Directory.CreateDirectory(imagesFolderPath);
            }

            // Delete old profile picture if a new one is provided
            if (userDto.ProfilePicture != null &&
                !string.IsNullOrEmpty(user.ProfilePictureUrl))
            {
                string oldPath = Path.Combine(
                    _hostEnvironment.WebRootPath,
                    "Images",
                    user.ProfilePictureUrl);

                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }

                // Save new profile picture
                user.ProfilePictureUrl =
                    await SaveProfilePictureAsync(userDto.ProfilePicture);
            }

            if (user.ProfilePictureUrl == null &&
                string.IsNullOrEmpty(user.ProfilePictureUrl))
            {
                user.ProfilePictureUrl =
                    await SaveProfilePictureAsync(userDto.ProfilePicture);
            }

            // Update other user properties
            user.FirstName = userDto.FirstName;
            user.LastName = userDto.LastName;
            user.PhoneNumber = userDto.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return new ReturnMessageDto
                {
                    Succeeded = true,
                    Message = "User updated successfully"
                };
            }
            else
            {
                return new ReturnMessageDto
                {
                    Succeeded = false,
                    Message = string.Join(
                        ", ",
                        result.Errors.Select(e => e.Description))
                };
            }
        }

        private async Task<string> SaveProfilePictureAsync(IFormFile profilePictureFile)
        {
            if (profilePictureFile == null)
            {
                return null;
            }

            string imageName =
                Guid.NewGuid().ToString() +
                Path.GetExtension(profilePictureFile.FileName);

            var imagePath = Path.Combine(
                _hostEnvironment.WebRootPath,
                "Images",
                imageName);

            using (var fileStream =
                   new FileStream(imagePath, FileMode.Create))
            {
                await profilePictureFile.CopyToAsync(fileStream);
            }

            return imageName;
        }

        public async Task<ReturnMessageDto> UpdateUserAddress(AddressDto userDto, string userId, int addressId)
        {
            var findaddress = await _dbcontext.UserAddress.FindAsync(addressId);

            if (findaddress == null)
            {
                return new ReturnMessageDto
                {
                    Succeeded = false,
                    Message = "Address not found"
                };
            }

            // If the incoming address is marked as default, make others non-default
            if (userDto.IsDefault)
            {
                var alternateAddresses = _dbcontext.UserAddress
                    .Where(x => x.UserId == Guid.Parse(userId) && x.IsDefault)
                    .ToList();

                foreach (var address in alternateAddresses)
                {
                    address.IsDefault = false;
                }
            }

            findaddress.City = userDto.City;
            findaddress.State = userDto.State;
            findaddress.PostCode = userDto.PostCode;
            findaddress.AreaandStreet = userDto.AreaandStreet;
            findaddress.Country = userDto.Country;
            findaddress.PhoneNumber = userDto.PhoneNumber;
            findaddress.AlternatePhoneNumber = userDto.AlternatePhoneNumber;
            findaddress.Landmark = userDto.Landmark;
            findaddress.IsDefault = userDto.IsDefault;

            _dbcontext.UserAddress.Update(findaddress);
            await _dbcontext.SaveChangesAsync();

            return new ReturnMessageDto
            {
                Succeeded = true,
                Message = "Address updated successfully"
            };
        }
    }
}
