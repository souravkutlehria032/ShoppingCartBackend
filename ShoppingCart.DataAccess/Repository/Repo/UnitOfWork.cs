using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ShoppingCart.DataAccess.BackgroundService;
using ShoppingCart.DataAccess.Data;
using ShoppingCart.DataAccess.Entity;
using ShoppingCart.DataAccess.Repository.IRepo;
using ShoppingCart.Utility.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DataAccess.Repository.Repo
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtService _jwtService;
        private readonly ApplicationDbContext _dbcontext;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IEmailService _emailService;

        private readonly ConcurrentDictionary<string,
            (ApplicationUser user, string password, string role, string otp, DateTime CreatedAt)>
            _temporaryUserStore;

        private readonly LinkGenerator _linkGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IConfiguration _configuration;

        private readonly ILogger<OrderRepository> _Ologger;
        private readonly ILogger<AuthManagementRepository> _alogger;
        private readonly ILogger<ProductManagementRepository> _plogger;

        public UnitOfWork(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            ILogger<ProductManagementRepository> plogger,
            ILogger<AuthManagementRepository> alogger,
            IHubContext<NotificationHub> hubContext,
            ILogger<OrderRepository> Ologger,
            LinkGenerator linkGenerator,
            IHttpContextAccessor httpContextAccessor,
            SignInManager<ApplicationUser> signInManager,
            JwtService jwtService,
            ApplicationDbContext dbcontext,
            IWebHostEnvironment hostEnvironment,
            IEmailService emailService,
            ConcurrentDictionary<string,
                (ApplicationUser user, string password, string role, string otp, DateTime CreatedAt)>
                temporaryUserStore)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _dbcontext = dbcontext;
            _hubContext = hubContext;
            _hostEnvironment = hostEnvironment;
            _temporaryUserStore = temporaryUserStore;
            _emailService = emailService;
            _linkGenerator = linkGenerator;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;

            _Ologger = Ologger;

            // Fixed original bug:
            _alogger = alogger;

            _plogger = plogger;

            Auth = new AuthManagementRepository(
                _userManager,
                _dbcontext,
                _alogger,
                _linkGenerator,
                _httpContextAccessor,
                _signInManager,
                _jwtService,
                _temporaryUserStore,
                _emailService,
                _hostEnvironment);

            Product = new ProductManagementRepository(
                _dbcontext,
                _userManager,
                _emailService,
                _hostEnvironment,
                _plogger);

            Vendor = new VendorRepository(
                _dbcontext,
                _userManager,
                _emailService,
                _hostEnvironment);

            User = new UserRespository(
                _hostEnvironment,
                _userManager,
                _hubContext,
                _emailService,
                _configuration,
                _dbcontext);

            Order = new OrderRepository(
                _dbcontext,
                _Ologger,
                _hubContext);

            Payment = new PaymentRepository(
                _configuration,
                _dbcontext,
                _emailService,
                _hostEnvironment,
                _hubContext);

            ReturnRequest = new ReturnRequestRepository(
                _dbcontext,
                Order,
                _hubContext,
                Product,
                _emailService);
        }

        public IAuthManagementRepository Auth { get; private set; }

        public IProductManagementRepository Product { get; private set; }

        public IUserRepository User { get; private set; }

        public IVendorRepository Vendor { get; private set; }

        public IOrderRepository Order { get; private set; }

        public IPaymentRepository Payment { get; private set; }

        public IReturnRequestRepository ReturnRequest { get; private set; }

        public void Save()
        {
            _dbcontext.SaveChanges();
        }
    }
}
