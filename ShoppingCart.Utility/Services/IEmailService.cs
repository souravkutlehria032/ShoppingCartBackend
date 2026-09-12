using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Utility.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(
            string toEmail,
            string subject,
            string body,
            string productImagePath = null,
            string contentId = "ProductImage");

        Task SendEmailWithAttachmentAsync(
            string toEmail,
            string subject,
            string body,
            byte[] attachment,
            string fileName);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(
            string toEmail,
            string subject,
            string body,
            string productImagePath = null,
            string contentId = "ProductImage")
        {
            try
            {
                var smtpSettings =
                    _configuration.GetSection("SmtpSettings");

                using (var smtpClient = new SmtpClient())
                {
                    smtpClient.Host = smtpSettings["Host"];
                    smtpClient.Port =
                        int.Parse(smtpSettings["Port"]);

                    smtpClient.EnableSsl =
                        bool.Parse(smtpSettings["EnableSsl"]);

                    smtpClient.UseDefaultCredentials = false;

                    smtpClient.Credentials =
                        new NetworkCredential(
                            smtpSettings["Username"],
                            smtpSettings["Password"]);

                    var message = new MailMessage
                    {
                        From = new MailAddress(
                            smtpSettings["Username"]),
                        Subject = subject,
                        IsBodyHtml = true
                    };

                    message.To.Add(toEmail);

                    var logoPath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "App-logo",
                        "logo.png");

                    var htmlView =
                        AlternateView.CreateAlternateViewFromString(
                            body,
                            null,
                            "text/html");

                    var logo =
                        new LinkedResource(
                            logoPath,
                            "image/png")
                        {
                            ContentId = "AppLogo",
                            TransferEncoding =
                                System.Net.Mime.TransferEncoding.Base64
                        };

                    htmlView.LinkedResources.Add(logo);

                    if (!string.IsNullOrEmpty(productImagePath) &&
                        File.Exists(productImagePath))
                    {
                        var imageResource =
                            new LinkedResource(
                                productImagePath,
                                "image/png")
                            {
                                ContentId = contentId,
                                TransferEncoding =
                                    System.Net.Mime.TransferEncoding.Base64
                            };

                        htmlView.LinkedResources.Add(imageResource);
                    }

                    message.AlternateViews.Add(htmlView);

                    await smtpClient.SendMailAsync(message);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "Failed to send email.", ex);
            }
        }

        public async Task SendEmailWithAttachmentAsync(
            string toEmail,
            string subject,
            string body,
            byte[] attachment,
            string fileName)
        {
            try
            {
                var smtpSettings =
                    _configuration.GetSection("SmtpSettings");

                using (var smtpClient = new SmtpClient())
                {
                    smtpClient.Host = smtpSettings["Host"];
                    smtpClient.Port =
                        int.Parse(smtpSettings["Port"]);

                    smtpClient.EnableSsl =
                        bool.Parse(smtpSettings["EnableSsl"]);

                    smtpClient.UseDefaultCredentials = false;

                    smtpClient.Credentials =
                        new NetworkCredential(
                            smtpSettings["Username"],
                            smtpSettings["Password"]);

                    var message = new MailMessage
                    {
                        From = new MailAddress(
                            smtpSettings["Username"]),
                        Subject = subject,
                        IsBodyHtml = true
                    };

                    message.To.Add(toEmail);

                    var logoPath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "App-logo",
                        "logo.png");

                    var htmlView =
                        AlternateView.CreateAlternateViewFromString(
                            body,
                            null,
                            "text/html");

                    var logo =
                        new LinkedResource(
                            logoPath,
                            "image/png")
                        {
                            ContentId = "AppLogo",
                            TransferEncoding =
                                System.Net.Mime.TransferEncoding.Base64
                        };

                    htmlView.LinkedResources.Add(logo);

                    message.AlternateViews.Add(htmlView);

                    using (var memoryStream =
                           new MemoryStream(attachment))
                    {
                        var attachmentItem =
                            new Attachment(
                                memoryStream,
                                fileName,
                                "application/pdf");

                        message.Attachments.Add(attachmentItem);

                        await smtpClient.SendMailAsync(message);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "Failed to send email with attachment.", ex);
            }
        }
    }
}
