using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Utility.DTOs
{
    public class ReturnMessageDto
    {
        public bool Succeeded { get; set; }
        public string? Message { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public string? Role { get; set; }

        public string? deviceId { get; set; }
        public object Data { get; set; }
    }
}
