using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Razorpay.Api;
using ShoppingCart.DataAccess.BackgroundService;
using ShoppingCart.DataAccess.Data;
using ShoppingCart.DataAccess.Entity;
using ShoppingCart.DataAccess.Repository.IRepo;
using ShoppingCart.Utility.DTOs;
using ShoppingCart.Utility.Enums;
using ShoppingCart.Utility.Services;
using static ShoppingCart.Utility.Enums.Enums;

namespace ShoppingCart.DataAccess.Repository.Repo
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IConfiguration _configuration;
        private static readonly Random _random = new Random();

        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ApplicationDbContext _dbcontext;
        private readonly IEmailService _emailService;
        private readonly IHubContext<NotificationHub> _hubContext;
        public PaymentRepository(IConfiguration configuration, ApplicationDbContext dbcontext, IEmailService emailService, IWebHostEnvironment hostEnvironment, IHubContext<NotificationHub> hubContext)
        {
            _configuration = configuration;
            _dbcontext = dbcontext;
            _emailService = emailService;
            _hubContext = hubContext;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<ReturnMessageDto> CreateOrderAsync(PaymentDtos paymentRequest)
        {
            try
            {
                // Validate order exists and belongs to user
                var order = await _dbcontext.Order
                    .FirstOrDefaultAsync(o => o.OrderId == paymentRequest.OrderId &&
                                             o.UserId == paymentRequest.UserId);

                if (order == null)
                {
                    return new ReturnMessageDto
                    {
                        Succeeded = false,
                        Message = "Order not found or doesn't belong to user"
                    };
                }

                // Initialize Razorpay client
                RazorpayClient client = new RazorpayClient(
                    _configuration["Razorpay:Key"],
                    _configuration["Razorpay:SecretKey"]
                );

                // Create Razorpay order
                Dictionary<string, object> parameters = new Dictionary<string, object> {
                    {"amount", order.TotalAmount * 100}, // Use order total instead of request amount
                    {"currency", "INR"},
                    {"receipt", order.OrderId.ToString()},
                    {"notes", new Dictionary<string, string> {
                        {"orderId", order.OrderId.ToString()}
                    }}
                };

                Razorpay.Api.Order razorpayOrder = client.Order.Create(parameters);

                // Create payment record in database
                var payment = new Entity.Payment
                {
                    OrderId = order.OrderId,
                    Status = PaymentStatus.Pending,
                    PaymentMethod = "Razorpay",
                    PaymentDate = DateTime.UtcNow,
                    GatewayReference = razorpayOrder["id"].ToString()
                };

                _dbcontext.Payment.Add(payment);
                await _dbcontext.SaveChangesAsync();

                return new ReturnMessageDto
                {
                    Succeeded = true,
                    Message = "Payment order created successfully",
                    Token = razorpayOrder["id"].ToString(),
                    Data = new { OrderId = order.OrderNumber }
                };
            }
            catch (Exception ex)
            {
                return new ReturnMessageDto
                {
                    Succeeded = false,
                    Message = $"Failed to create payment order: {ex.Message}"
                };
            }
        }


        public async Task<ReturnMessageDto> VerifyPayment(RazorPaymentResponse paymentResponse)
        {
            try
            {
                if (paymentResponse == null)
                    throw new ArgumentNullException(nameof(paymentResponse));

                if (string.IsNullOrEmpty(paymentResponse.razor_order_id))
                    throw new ArgumentException("Razorpay order ID is required");

                if (string.IsNullOrEmpty(paymentResponse.razor_payment_id))
                    throw new ArgumentException("Razorpay payment ID is required");

                if (string.IsNullOrEmpty(paymentResponse.razor_signature))
                    throw new ArgumentException("Razorpay signature is required");

                RazorpayClient client = new RazorpayClient(
                    _configuration["Razorpay:Key"],
                    _configuration["Razorpay:SecretKey"]
                );

                Dictionary<string, string> parameters = new Dictionary<string, string> {
            {"razorpay_order_id", paymentResponse.razor_order_id},
            {"razorpay_payment_id", paymentResponse.razor_payment_id},
            {"razorpay_signature", paymentResponse.razor_signature}
        };

                Utils.verifyPaymentSignature(parameters);
                var payment = await _dbcontext.Payment
            .Include(p => p.Order)
                .ThenInclude(o => o.User)
            .Include(p => p.Order)
                .ThenInclude(o => o.Address)
            .Include(p => p.Order)
                .ThenInclude(o => o.OrderItems)
                    .ThenInclude(oi => oi.ProductVariant)
            .ThenInclude(pv => pv.Product)
            .Include(p => p.Order)
                .ThenInclude(o => o.OrderItems)
                    .ThenInclude(oi => oi.ProductVariant)
            .ThenInclude(pv => pv.Color)
            .Include(p => p.Order)
                .ThenInclude(o => o.OrderItems)
                    .ThenInclude(oi => oi.ProductVariant)
            .ThenInclude(pv => pv.ProductSize)
            .FirstOrDefaultAsync(p => p.GatewayReference == paymentResponse.razor_order_id);


                if (payment == null)
                {
                    return new ReturnMessageDto
                    {
                        Succeeded = false,
                        Message = "Payment record not found"
                    };
                }

                // Update Payment
                payment.Status = PaymentStatus.Completed;
                payment.GatewayPaymentId = paymentResponse.razor_payment_id;


                // Update Order if exists
                if (payment.OrderId != null)
                {
                    var order = await _dbcontext.Order
                         .Include(o => o.Shipping)
                         .Include(o => o.OrderItems)
                             .ThenInclude(oi => oi.ProductVariant)
                                 .ThenInclude(pv => pv.Product)
                                     .ThenInclude(p => p.Images)  // Load all product images
                         .Include(o => o.OrderItems)
                             .ThenInclude(oi => oi.ProductVariant)
                                 .ThenInclude(pv => pv.Color)     // Load color info
                         .FirstOrDefaultAsync(o => o.OrderId == payment.OrderId);

                    // Manually set variant images by filtering product images by color
                    foreach (var item in order.OrderItems)
                    {
                        if (item.ProductVariant != null && item.ProductVariant.Product != null)
                        {
                            item.ProductVariant.VariantImages = item.ProductVariant.Product.Images
                                .Where(img => img.ColorId == item.ProductVariant.ColorId)
                                .ToList();
                        }
                    }


                    if (order != null)
                    {
                        if (order.Status == OrderStatus.Pending)
                        {
                            order.Status = OrderStatus.Confirmed;
                        }




                        if (order.ShippingId != null)
                        {
                            order.Shipping.Status = ShippingStatus.Shipped;
                            order.Shipping.ShippedDate = DateTime.UtcNow;
                        }
                        int lowStockThreshold = 5;

                        foreach (var item in order.OrderItems)
                        {
                            if (item.ProductVariant != null)
                            {
                                item.ProductVariant.StockQuantity -= item.Quantity;

                                if (item.ProductVariant.StockQuantity < 0)
                                    item.ProductVariant.StockQuantity = 0;

                                _dbcontext.ProductVariant.Update(item.ProductVariant);

                                if (item.ProductVariant.StockQuantity <= lowStockThreshold)
                                {
                                    string emailBody = GenerateLowStockEmailBody(item.ProductVariant);

                                    // Fire and forget email to admin
                                    _ = _emailService.SendEmailAsync(
                                        "kksaini0036@gmail.com",
                                        "Low Stock Alert",
                                        emailBody
                                    );
                                }
                            }
                        }

                    }
                    // --- Add Admin Notification ---
                    var notification = new Notification
                    {
                        RecipientRole = "Admin",
                        OrderNumber = order.OrderNumber,
                        ProductName = order.OrderItems.FirstOrDefault()?.ProductVariant?.Product?.Name,
                        Reason = $"New order confirmed: #{order.OrderNumber}",
                        CreatedAt = DateTime.UtcNow,
                        IsRead = false,
                    };
                    await _dbcontext.Notification.AddAsync(notification);

                    // --- Broadcast via SignalR ---
                    await _hubContext.Clients.Group("Admin").SendAsync("OrderReceiveNotification", new
                    {
                        id = notification.Id,
                        orderNumber = notification.OrderNumber,
                        message = notification.Reason,
                        createdAt = notification.CreatedAt,
                        isRead = notification.IsRead
                    });

                    await _dbcontext.SaveChangesAsync();
                }


                //Invoice Table

                var invoice = await GenerateInvoiceAsync(payment.Order, payment);
                _dbcontext.Invoice.Add(invoice);
                await _dbcontext.SaveChangesAsync();



                _ = Task.Run(async () =>
                {
                    try
                    {
                        var pdfBytes = GenerateInvoicePdf(payment.Order, payment, invoice);

                        // Prepare email content
                        var emailBody = GenerateOrderConfirmationEmail(payment.Order, payment.Order.User.Email);

                        // Send email with attachment
                        await _emailService.SendEmailWithAttachmentAsync(
                            payment.Order.User.Email,
                            $"Your Order #{payment.Order.OrderNumber} Confirmation",
                            emailBody,
                            pdfBytes,
                            $"Invoice_{payment.Order.OrderId}.pdf"
                        );
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Failed to send email: {ex.Message}");
                    }
                });

                return new ReturnMessageDto
                {
                    Succeeded = true,
                    Data = payment.Order.OrderNumber,

                    Message = "Payment and related records updated successfully"
                };

            }
            catch (Exception ex)
            {
                return new ReturnMessageDto
                {
                    Succeeded = false,
                    Message = $"Failed to verify payment: {ex.Message}"
                };
            }
        }

        private string GenerateLowStockEmailBody(ProductVariant variant)
        {
            string productName = variant.Product?.Name ?? "Unknown Product";
            int variantId = variant.ProductVariantId;
            int stockQty = variant.StockQuantity;
            string color = variant.Color?.ColorName ?? "N/A";
            string size = variant.ProductSize.ProductSizeId.ToString();

            return $@"
    <div style='font-family: Poppins, Arial, sans-serif; background: #fff8e1; padding: 20px; border-radius: 8px; max-width: 600px; margin: auto; box-shadow: 0 4px 12px rgba(0,0,0,0.1);'>
        <h2 style='color: #d84315;'>⚠️ Low Stock Alert</h2>
        <p style='font-size: 16px; color: #333;'>
            The stock for the following product variant has fallen below the threshold:
        </p>
        <ul style='font-size: 14px; color: #555;'>
            <li><strong>Product Name:</strong> {productName}</li>
            <li><strong>Variant ID:</strong> {variantId}</li>
            <li><strong>Color:</strong> {color}</li>
            <li><strong>Size:</strong> {size}</li>
            <li><strong>Current Stock Quantity:</strong> {stockQty}</li>
        </ul>
        <p style='font-size: 14px; color: #333;'>
            Please restock this item as soon as possible to avoid stockouts.
        </p>
        <hr style='border:none; border-top: 1px solid #eee;'/>
        <p style='font-size: 12px; color: #999;'>
            This is an automated message from your inventory management system.
        </p>
    </div>";
        }

        public async Task<ReturnMessageDto> IssueRefundAsync(RefundRequestDto refundRequest)
        {
            try
            {
                var payment = await _dbcontext.Payment
                    .Include(p => p.Order)
                        .ThenInclude(o => o.OrderItems)
                            .ThenInclude(oi => oi.ProductVariant)
                                .ThenInclude(pv => pv.Product)
                    .FirstOrDefaultAsync(p => p.OrderId == refundRequest.OrderId);

                if (payment == null)
                {
                    return new ReturnMessageDto { Succeeded = false, Message = "Order not found." };
                }

                if (payment.Status != PaymentStatus.Completed)
                {
                    return new ReturnMessageDto { Succeeded = false, Message = "Payment not completed or missing." };
                }

                if (payment.Status == PaymentStatus.Refunded)
                {
                    return new ReturnMessageDto { Succeeded = false, Message = "Payment already refunded." };
                }

                var order = payment.Order;
                decimal refundAmount = refundRequest.Amount ?? order.TotalAmount;
                if (refundAmount <= 0 || refundAmount > order.TotalAmount)
                {
                    return new ReturnMessageDto { Succeeded = false, Message = "Invalid refund amount." };
                }

                // ✅ Check refund eligibility based on return policy and order date
                var now = DateTime.UtcNow;
                var ineligibleItems = order.OrderItems.Where(oi =>
                {
                    var product = oi.ProductVariant.Product;
                    return !product.IsReturnable ||
                           (order.OrderDate.AddDays(product.ReturnDays) < now);
                }).ToList();

                if (ineligibleItems.Any())
                {
                    var productNames = string.Join(", ", ineligibleItems
                        .Select(oi => oi.ProductVariant.Product.Name)
                        .Distinct());

                    return new ReturnMessageDto
                    {
                        Succeeded = false,
                        Message = $"Refund rejected: The following items are no longer eligible for return - {productNames}"
                    };
                }

                // 🧾 Create refund with Razorpay
                var client = new RazorpayClient(
                    _configuration["Razorpay:Key"],
                    _configuration["Razorpay:SecretKey"]
                );

                // First fetch the payment from Razorpay using GatewayPaymentId
                var paymentRzp = client.Payment.Fetch(payment.GatewayPaymentId);

                // Now create the refund
                var refundParams = new Dictionary<string, object>
                {
                    { "amount", refundAmount * 100 }, // paise
                    { "speed", "normal" },
                    { "notes", new Dictionary<string, string>
                        {
                            { "reason", refundRequest.Reason },
                            { "orderId", order.OrderId.ToString() }
                        }
                    }
                };

                var razorpayRefund = paymentRzp.Refund(refundParams);


                // Update payment record
                payment.Status = PaymentStatus.Refunded;
                payment.RefundAmount = refundAmount;
                payment.RefundDate = DateTime.UtcNow;
                payment.RefundReason = refundRequest.Reason;
                payment.RefundReference = razorpayRefund["id"].ToString();

                // Update order status
                order.Status = OrderStatus.Refunded;

                var returnRequest = await _dbcontext.ReturnRequest
    .FirstOrDefaultAsync(r => r.OrderId == order.OrderId);

                if (returnRequest != null)
                {
                    returnRequest.Return_ReplaceStatus = ReturnStatus.Refunded; // or ReturnStatus.Replaced
                    _dbcontext.ReturnRequest.Update(returnRequest);
                }

                // Update shipping (if exists)
                var shipping = await _dbcontext.Shipping
                    .FirstOrDefaultAsync(s => s.OrderId == order.OrderId);

                if (shipping != null)
                {
                    shipping.ReturnStatus = ReturnStatus.Refunded;
                    _dbcontext.Shipping.Update(shipping);
                }


                // Restock items
                if (refundRequest.RestockItems)
                {
                    foreach (var item in order.OrderItems)
                    {
                        item.ProductVariant.StockQuantity += item.Quantity;
                        _dbcontext.ProductVariant.Update(item.ProductVariant);
                    }
                }

                await _dbcontext.SaveChangesAsync();

                return new ReturnMessageDto
                {
                    Succeeded = true,
                    Message = $"Refund of ₹{refundAmount} processed successfully.",
                    Data = new { RefundId = razorpayRefund["id"].ToString() }
                };
            }
            catch (Exception ex)
            {
                return new ReturnMessageDto
                {
                    Succeeded = false,
                    Message = $"Refund failed: {ex.Message}"
                };
            }
        }



        #region Invoice

        public async Task<Entity.Invoice> GenerateInvoiceAsync(Entity.Order order, Entity.Payment payment)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (payment == null)
                throw new ArgumentNullException(nameof(payment));
            var invoiceNumber = GenerateId("invoice");

            var invoice = new Entity.Invoice
            {
                OrderId = order.OrderId,
                PaymentId = payment.PaymentId,
                InvoiceNumber = invoiceNumber,
                IssueDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(7),
            };

            var pdfBytes = GenerateInvoicePdf(order, payment, invoice);
            invoice.FileContent = pdfBytes;

            // Optionally save to file system
            var invoiceDirectory = Path.Combine(_hostEnvironment.WebRootPath, "invoices");
            Directory.CreateDirectory(invoiceDirectory);

            var fileName = $"{invoiceNumber}.pdf";
            invoice.FilePath = Path.Combine("invoices", fileName);

            await File.WriteAllBytesAsync(Path.Combine(invoiceDirectory, fileName), pdfBytes);

            return invoice;
        }

        public string GenerateId(string type)
        {
            string prefix = type.ToLower() switch
            {
                "invoice" => "IN",
                "order" => "OD",
                _ => throw new ArgumentException("Invalid type. Use 'invoice' or 'order'")
            };

            string randomPart = GenerateRandomString(16);

            return $"{prefix}{randomPart}";
        }

        private string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            const string digits = "0123456789";

            var result = new List<char>();

            // Ensure at least one digit
            result.Add(digits[_random.Next(digits.Length)]);

            // Fill the rest with safe characters
            for (int i = 1; i < length; i++)
            {
                result.Add(chars[_random.Next(chars.Length)]);
            }

            // Shuffle
            return new string(result.OrderBy(_ => _random.Next()).ToArray());
        }

        public byte[] GenerateInvoicePdf(Entity.Order order, Entity.Payment payment, Entity.Invoice invoice)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    // Page styling with border
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontFamily("Helvetica").FontSize(10));

                    // Add a decorative border around the page
                    page.Background()
                        .Border(1)
                        .BorderColor(Colors.Grey.Lighten2)
                        .Padding(10);

                    // Header section
                    page.Header()
                        .Column(column =>
                        {
                            column.Item().Row(row =>
                            {
                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().Text("Little Bees").Bold().FontSize(18).FontColor(Colors.Blue.Darken3);
                                    col.Item().Text("Address: F-748, Phase 8B, Industrial Area").FontSize(9);
                                    col.Item().Text("Sector 74, SAS Nagar, Punjab 160071").FontSize(9);
                                    col.Item().Text("Phone: +91 98765 43210 | Email: info@techstylefashion.com").FontSize(9);
                                });

                                row.RelativeItem().AlignRight().Column(col =>
                                {
                                    col.Item().AlignRight().Text("INVOICE").Bold().FontSize(24).FontColor(Colors.Red.Medium);
                                    col.Item().AlignRight().Text(text =>
                                    {
                                        text.Span("Invoice Number: ").Bold().FontSize(10).FontColor(Colors.Grey.Darken2);
                                        text.Span(invoice.InvoiceNumber).Bold().FontSize(10).FontColor(Colors.Black);
                                    });
                                    col.Item().AlignRight().Text(text =>
                                    {
                                        text.Span("Order Number: ").Bold().FontSize(10).FontColor(Colors.Grey.Darken2);
                                        text.Span(order.OrderNumber).Bold().FontSize(10).FontColor(Colors.Black);
                                    });
                                    col.Item().AlignRight().Text(text =>
                                    {
                                        text.Span("Order Date: ").Bold().FontSize(10).FontColor(Colors.Grey.Darken2);
                                        text.Span($"{DateTime.Now:dd MMMM yyyy}").Bold().FontSize(10).FontColor(Colors.Black);
                                    });
                                });
                            });

                            column.Item().PaddingTop(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                        });

                    // Content section
                    page.Content()
                        .PaddingVertical(10)
                        .Column(column =>
                        {
                            // Client and shipping information
                            column.Item().Row(row =>
                            {
                                row.RelativeItem().PaddingRight(10).Background(Colors.Grey.Lighten5).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(col =>
                                {
                                    col.Item().Text("BILL TO").Bold().FontSize(11).FontColor(Colors.Blue.Darken2);
                                    col.Item().Text($"{order.User?.FirstName} {order.User?.LastName}").Bold();
                                    if (order.Address != null)
                                    {
                                        col.Item().Text(order.Address.AreaandStreet);
                                        col.Item().Text($"{order.Address.City}, {order.Address.State}");
                                        col.Item().Text($"PIN: {order.Address.PostCode}");
                                        col.Item().Text(order.Address.Country);
                                        col.Item().Text($"Phone: {order.Address.PhoneNumber ?? "NA"}");
                                    }
                                    col.Item().Text($"Email: {order.User?.Email ?? "NA"}");
                                });

                                row.RelativeItem().Background(Colors.Grey.Lighten5).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(col =>
                                {
                                    col.Item().Text("SHIP TO").Bold().FontSize(11).FontColor(Colors.Blue.Darken2);
                                    col.Item().Text($"{order.User?.FirstName} {order.User?.LastName}").Bold();
                                    if (order.Address != null)
                                    {
                                        col.Item().Text(order.Address.AreaandStreet);
                                        col.Item().Text($"{order.Address.City}, {order.Address.State}");
                                        col.Item().Text($"PIN: {order.Address.PostCode}");
                                        col.Item().Text(order.Address.Country);
                                        col.Item().Text($"Phone: {order.Address.PhoneNumber ?? "NA"}");
                                    }
                                });
                            });

                            // Items table
                            column.Item().PaddingTop(15).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(25); // #
                                    columns.ConstantColumn(60); // Image
                                    columns.RelativeColumn(3); // Product Details
                                    columns.ConstantColumn(50); // Qty
                                    columns.ConstantColumn(70); // Unit Price
                                    columns.ConstantColumn(70); // Discount
                                    columns.ConstantColumn(80); // Total
                                });

                                // Table header
                                table.Header(header =>
                                {
                                    header.Cell().Background(Colors.Blue.Darken3).Padding(5).Text("#").FontColor(Colors.White).SemiBold();
                                    header.Cell().Background(Colors.Blue.Darken3).Padding(5).Text("Image").FontColor(Colors.White);
                                    header.Cell().Background(Colors.Blue.Darken3).Padding(5).Text("Product Details").FontColor(Colors.White).SemiBold();
                                    header.Cell().Background(Colors.Blue.Darken3).Padding(5).AlignRight().Text("Qty").FontColor(Colors.White).SemiBold();
                                    header.Cell().Background(Colors.Blue.Darken3).Padding(5).AlignRight().Text("Unit Price").FontColor(Colors.White).SemiBold();
                                    header.Cell().Background(Colors.Blue.Darken3).Padding(5).AlignRight().Text("Discount").FontColor(Colors.White).SemiBold();
                                    header.Cell().Background(Colors.Blue.Darken3).Padding(5).AlignRight().Text("Total").FontColor(Colors.White).SemiBold();
                                });

                                // Table items
                                int index = 1;
                                foreach (var item in order.OrderItems)
                                {
                                    var productName = $"{item.ProductVariant.Product.Name}";
                                    if (item.ProductVariant.Color != null)
                                        productName += $" ({item.ProductVariant.Color.ColorName})";
                                    if (item.ProductVariant.ProductSize != null)
                                        productName += $" - {item.ProductVariant.ProductSize.SizeName}";

                                    var relativeImagePath = item.ProductVariant.Product.Images.FirstOrDefault()?.ImageUrl;
                                    byte[] imageBytes = Array.Empty<byte>();

                                    if (!string.IsNullOrEmpty(relativeImagePath))
                                    {
                                        try
                                        {
                                            string baseUrl = "http://108.181.215.219:830/";
                                            var fullImageUrl = $"{baseUrl}{relativeImagePath}";
                                            using (var httpClient = new HttpClient())
                                            {
                                                // Bypass SSL certificate validation for development
                                                var handler = new HttpClientHandler
                                                {
                                                    ServerCertificateCustomValidationCallback =
                                                        (sender, cert, chain, sslPolicyErrors) => true
                                                };

                                                using (var client = new HttpClient(handler))
                                                {
                                                    imageBytes = client.GetByteArrayAsync(fullImageUrl).GetAwaiter().GetResult();
                                                }
                                            }
                                        }
                                        catch
                                        {
                                            // Handle image error
                                            imageBytes = Array.Empty<byte>();
                                        }
                                    }

                                    var rowColor = index % 2 == 0 ? Colors.Grey.Lighten5 : Colors.White;

                                    table.Cell().Background(rowColor).PaddingVertical(5).Text(index.ToString());

                                    // Image cell
                                    if (imageBytes.Length > 0)
                                    {
                                        table.Cell()
                                            .Background(rowColor)
                                            .Width(60)
                                            .Height(60)
                                            .Padding(5)
                                            .AlignMiddle()
                                            .AlignCenter()
                                            .Image(imageBytes, ImageScaling.FitArea);

                                    }
                                    else
                                    {
                                        table.Cell()
                                           .Background(rowColor)
                                           .Width(60)
                                           .Height(60)
                                           .AlignMiddle()
                                           .Text("No Image").FontSize(8);
                                    }

                                    table.Cell().Background(rowColor).PaddingVertical(5).Column(col =>
                                    {
                                        col.Item().Text(productName).Bold().FontSize(11);
                                        col.Item().Text($"Brand: {item.ProductVariant.Product.BrandName}").FontSize(9);
                                        if (item.ProductVariant.Product.IsReturnable)
                                        {
                                            col.Item().Text($"Returnable: {item.ProductVariant.Product.ReturnDays} days").FontSize(9).FontColor(Colors.Green.Darken2);
                                        }
                                    });

                                    table.Cell().Background(rowColor).PaddingVertical(5).AlignRight().Text(item.Quantity.ToString());
                                    table.Cell().Background(rowColor).PaddingVertical(5).AlignRight().Text(item.UnitPrice.ToString("C", new CultureInfo("en-IN")));
                                    table.Cell().Background(rowColor).PaddingVertical(5).AlignRight().Text((item.Discount + item.OfferDiscount).ToString("C", new CultureInfo("en-IN"))).FontColor(Colors.Red.Darken2);
                                    table.Cell().Background(rowColor).PaddingVertical(5).AlignRight().Text(item.TotalPrice.ToString("C", new CultureInfo("en-IN"))).Bold();

                                    index++;
                                }
                            });

                            // Summary section
                            column.Item().PaddingTop(15).Row(row =>
                            {
                                row.RelativeItem(2);

                                row.RelativeItem()
                                    .Border(1)
                                    .BorderColor(Colors.Grey.Lighten2)
                                    .Padding(10)
                                    .Column(col =>
                                    {
                                        col.Item()
                                            .Text("SUMMARY")
                                            .Bold()
                                            .FontSize(12)
                                            .FontColor(Colors.Blue.Darken3);

                                        col.Item()
                                            .LineHorizontal(1)
                                            .LineColor(Colors.Grey.Lighten1);

                                        // Subtotal
                                        col.Item().Row(r =>
                                        {
                                            r.RelativeItem().Text("Subtotal:");
                                            r.ConstantItem(80)
                                             .AlignRight()
                                             .Text(order.OrderItems.Sum(i => i.UnitPrice * i.Quantity).ToString("C", new CultureInfo("en-IN")));
                                        });

                                        // Discounts
                                        col.Item().Row(r =>
                                        {
                                            r.RelativeItem().Text("Discounts:");
                                            r.ConstantItem(80)
                                             .AlignRight()
                                             .Text(order.OrderItems.Sum(i => i.Discount + i.OfferDiscount).ToString("C", new CultureInfo("en-IN")))
                                             .FontColor(Colors.Red.Darken2);
                                        });

                                        // Shipping
                                        col.Item().Row(r =>
                                        {
                                            r.RelativeItem().Text("Shipping:");
                                            r.ConstantItem(80)
                                             .AlignRight()
                                             .Text(order.OrderItems.Sum(i => i.ShippingFee).ToString("C", new CultureInfo("en-IN")));
                                        });

                                        // Tax (if applicable)
                                        if (order.OrderItems.Any(i => i.ProductVariant.TaxPercentage.HasValue))
                                        {
                                            var taxRate = order.OrderItems
                                                               .First(i => i.ProductVariant.TaxPercentage.HasValue)
                                                               .ProductVariant.TaxPercentage.Value;

                                            var taxAmount = order.TotalAmount * taxRate / 100;

                                            col.Item().Row(r =>
                                            {
                                                r.RelativeItem().Text($"Tax ({taxRate}%):");
                                                r.ConstantItem(80)
                                                 .AlignRight()
                                                 .Text(taxAmount.ToString("C", new CultureInfo("en-IN")));
                                            });
                                        }

                                        col.Item()
                                            .LineHorizontal(1)
                                            .LineColor(Colors.Grey.Lighten1);

                                        // Grand Total
                                        col.Item().Row(r =>
                                        {
                                            r.RelativeItem().Text("Grand Total:").Bold();
                                            r.ConstantItem(80)
                                             .AlignRight()
                                             .Text(order.TotalAmount.ToString("C", new CultureInfo("en-IN")))
                                             .Bold()
                                             .FontSize(12);
                                        });
                                        // Amount in Words
                                        col.Item().PaddingTop(5).Row(r =>
                                        {
                                            r.RelativeItem().Text("Amount in Words:").Bold().FontSize(9);
                                            r.RelativeItem().Text(NumberToWordsIndian(order.TotalAmount)).FontSize(9).FontColor(Colors.Blue.Darken2);
                                        });

                                    });
                            });


                            // Payment and terms section
                            column.Item().PaddingTop(15).Row(row =>
                            {
                                row.RelativeItem().PaddingRight(10).Column(col =>
                                {
                                    col.Item().Text("PAYMENT INFORMATION").Bold().FontSize(12).FontColor(Colors.Blue.Darken3);
                                    col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

                                    col.Item().Row(r =>
                                    {
                                        r.ConstantItem(100).Text("Payment Method:");
                                        r.RelativeItem().Text("Razorpay");
                                    });

                                    col.Item().Row(r =>
                                    {
                                        r.ConstantItem(100).Text("Payment ID:");
                                        r.RelativeItem().Text(payment.GatewayPaymentId).FontColor(Colors.Blue.Darken2);
                                    });

                                    col.Item().Row(r =>
                                    {
                                        r.ConstantItem(100).Text("Payment Date:");
                                        r.RelativeItem().Text(DateTime.Now.ToString("dd MMMM yyyy HH:mm"));
                                    });

                                    col.Item().Row(r =>
                                    {
                                        r.ConstantItem(100).Text("Status:");
                                        r.RelativeItem().Text("PAID").Bold().FontColor(Colors.Green.Darken2);
                                    });
                                });

                                // Signature section
                                column.Item().PaddingTop(30).AlignRight().Row(row =>
                                {
                                    row.ConstantItem(200).Column(col =>
                                    {
                                        col.Item().PaddingBottom(10).LineHorizontal(1).LineColor(Colors.Grey.Darken1);
                                        col.Item().AlignCenter().Text("Authorized Signature").FontSize(10).FontColor(Colors.Grey.Darken2);
                                        col.Item().AlignCenter().Text("Little Bees").FontSize(10).Bold();
                                    });
                                });
                            });

                        });

                    // Footer with page numbers
                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("Thank you for your business! | ").FontColor(Colors.Grey.Medium).FontSize(9);
                            text.Span("Page ").FontColor(Colors.Grey.Darken1).FontSize(9);
                            text.CurrentPageNumber().FontColor(Colors.Blue.Darken3).FontSize(9);
                            text.Span(" of ").FontColor(Colors.Grey.Darken1).FontSize(9);
                            text.TotalPages().FontColor(Colors.Blue.Darken3).FontSize(9);
                        });
                });
            });

            return document.GeneratePdf();
        }




        public string NumberToWordsIndian(decimal amount)
        {
            var n = (long)amount;
            if (n == 0) return "Zero Rupees Only";

            string[] ones = { "", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine" };
            string[] twos = { "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
            string[] tens = { "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

            string Words(long num)
            {
                if (num < 10) return ones[num];
                if (num < 20) return twos[num - 10];
                if (num < 100) return tens[num / 10] + (num % 10 > 0 ? " " + ones[num % 10] : "");
                if (num < 1000) return ones[num / 100] + " Hundred " + (num % 100 > 0 ? Words(num % 100) : "");
                if (num < 100000) return Words(num / 1000) + " Thousand " + (num % 1000 > 0 ? Words(num % 1000) : "");
                if (num < 10000000) return Words(num / 100000) + " Lakh " + (num % 100000 > 0 ? Words(num % 100000) : "");
                return Words(num / 10000000) + " Crore " + (num % 10000000 > 0 ? Words(num % 10000000) : "");
            }

            var words = Words(n).Trim() + " Rupees Only";
            return words;
        }

        private string GenerateOrderConfirmationEmail(Entity.Order order, string email)
        {
            return $@"
        <div style='font-family: Poppins, Arial, sans-serif; background: linear-gradient(to right, #e6f7ff, #ffffff); padding: 30px; border-radius: 12px; max-width: 650px; margin: auto; box-shadow: 0 4px 12px rgba(0,0,0,0.1);'>
    
            <div style='text-align: center; margin-bottom: 25px;'>
                <img src='cid:AppLogo' alt='Kids Shopping Logo' style='width: 120px; border-radius: 8px;' />
            </div>

            <h2 style='text-align: center; color: #333;'>Order Confirmation #{order.OrderNumber}</h2>

            <p style='font-size: 16px; color: #555; text-align: center; max-width: 90%; margin: auto;'>
               Thank you for your order! Your payment has been successfully received, and your order is being processed. Please find your invoice attached for your reference.
                    If you have any questions, feel free to contact our support team.
            </p>
           
           
            <p style='font-size: 14px; color: #555; text-align: center; margin-top: 20px;'>
                Your invoice is attached to this email. Please find the PDF attachment below.
            </p>

            <hr style='margin: 30px 0; border: none; border-top: 1px solid #eee;' />

            <p style='font-size: 14px; color: #555; text-align: center;'>
                Need help with your order? Contact us at
                <a href='mailto:support@kidsshopping.com' style='color: #FF6F61;'>support@kidsshopping.com</a>.
            </p>
        </div>";
        }

        public async Task<ReturnMessageDto> CancelPaymentAsync(int orderId)
        {
            var payment = await _dbcontext.Payment
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p => p.OrderId == orderId && p.Status == PaymentStatus.Pending);

            if (payment == null)
            {
                return new ReturnMessageDto
                {
                    Succeeded = false,
                    Message = "Pending payment not found"
                };
            }

            payment.Status = PaymentStatus.Failed; // or Cancelled
            if (payment.Order != null && payment.Order.Status == OrderStatus.Pending)
            {
                payment.Order.Status = OrderStatus.Cancelled;
            }

            await _dbcontext.SaveChangesAsync();

            return new ReturnMessageDto
            {
                Succeeded = true,
                Message = "Payment and order marked as cancelled"
            };
        }




        #endregion

    }
}