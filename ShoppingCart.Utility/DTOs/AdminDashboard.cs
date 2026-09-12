using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Utility.DTOs
{
    public class AdminDashboard
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int TotalCustomers { get; set; }
        public double ConversionRate { get; set; }
        public int TotalVisitors { get; set; }
        public List<ChartDataDto> ProfitData { get; set; }
        public List<AudienceDto> AudienceOverview { get; set; }
        public List<FeedbackDto> FeedbackData { get; set; }
        public List<OrderStatusDto> RecentOrders { get; set; }
        public List<CustomerFeedbackDto> CustomerFeedbacks { get; set; }
    }

    public class ChartDataDto
    {
        public string Name { get; set; }
        public decimal Value { get; set; }
    }

    public class CustomerFeedbackDto
    {
        public string UserName { get; set; }
        public string ProductName { get; set; }
        public decimal Rating { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }
    }

    public class AudienceDto
    {
        public string Name { get; set; }
        public decimal Revenue { get; set; }
        public int Orders { get; set; }
        public int Refunds { get; set; }
    }

    public class FeedbackDto
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }

    public class OrderStatusDto
    {
        public string OrderId { get; set; }
        public string Date { get; set; }
        public string Amount { get; set; }
        public string Status { get; set; }
    }
}
