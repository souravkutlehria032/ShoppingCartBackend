using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Utility.DTOs
{
    public class DailyReportDto
    {
        public string ReportDate { get; set; }
        public SummaryDto Summary { get; set; }
        public SalesBreakdownDto SalesBreakdown { get; set; }
        public OrdersStatusDto OrdersStatus { get; set; }
        public CustomerInsightsDto CustomerInsights { get; set; }
        public AlertsDto Alerts { get; set; }
        public string RangeStart { get; set; }
        public string RangeEnd { get; set; }
    }

    public class SummaryDto
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalVisitors { get; set; }
        public decimal ConversionRate { get; set; }
        public decimal AverageOrderValue { get; set; }
    }

    public class SalesBreakdownDto
    {
        public List<CategorySalesDto> TopCategories { get; set; }
        public List<ProductSalesDto> TopProducts { get; set; }
        public List<PaymentMethodDto> PaymentMethods { get; set; }
        public Dictionary<string, int> GenderBreakdown { get; set; }
        public Dictionary<string, int> OccasionBreakdown { get; set; }
        public Dictionary<string, int> SeasonBreakdown { get; set; }
    }

    public class MonthlyReportDto
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int TotalVisitors { get; set; }
        public decimal AverageOrderValue { get; set; }
        public List<DailyReportDto> DailyReports { get; set; }
    }

    public class WeeklyReportDto
    {
        public string WeekStart { get; set; }
        public string WeekEnd { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int TotalVisitors { get; set; }
        public decimal AverageOrderValue { get; set; }
        public List<DailyReportDto> DailyReports { get; set; }
    }

    public class CategorySalesDto
    {
        public string Name { get; set; }
        public decimal Revenue { get; set; }
        public int Orders { get; set; }
    }

    public class ProductSalesDto
    {
        public string Name { get; set; }
        public int QuantitySold { get; set; }
        public decimal Revenue { get; set; }
    }

    public class PaymentMethodDto
    {
        public string Method { get; set; }
        public decimal Revenue { get; set; }
    }

    public class OrdersStatusDto
    {
        public int Pending { get; set; }
        public int Shipped { get; set; }
        public int Delivered { get; set; }
        public int Cancelled { get; set; }
        public int Confirmed { get; set; }
        public int Refunded { get; set; }
    }

    public class CustomerInsightsDto
    {
        public List<CustomerDto> TopCustomers { get; set; }
        public List<LocationDto> Locations { get; set; }
    }

    public class CustomerDto
    {
        public string Name { get; set; }
        public int Orders { get; set; }
        public decimal Spent { get; set; }
    }

    public class LocationDto
    {
        public string City { get; set; }
        public int Orders { get; set; }
        public decimal Revenue { get; set; }
    }

    public class AlertsDto
    {
        public List<LowStockDto> LowStock { get; set; }
        public int RefundsIssued { get; set; }
    }

    public class LowStockDto
    {
        public string Product { get; set; }
        public int StockLeft { get; set; }
    }
}
