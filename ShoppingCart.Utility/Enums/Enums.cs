using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Utility.Enums
{
    public class Enums
    {
        public enum Gender
        {
            Boy,
            Girl,
            ForBoth
        }

        public enum AddressType
        {
            Customer = 1,
            Vendor = 2,
        }

        public enum VendorAddressType
        {
            Shop = 1,
            Warehouse = 2
        }

        public enum Season
        {
            Summer,
            Winter,
            Spring,
            Autumn,
            AllSeason
        }

        public enum Occasion
        {
            Casual = 1,
            Party = 2,
            Formal = 3,
            Festive = 4,
            Sportswear = 5,
            Sleepwear = 6,
            Beachwear = 7,
            Ethnic = 8,
            Holiday = 9,
            AllOccasions = 10
        }

        public enum OrderStatus
        {
            Pending,
            Shipped,
            Delivered,
            Cancelled,
            Confirmed,
            ReturnRequested,
            ReturnApproved,
            PickupScheduled,
            PickedUp,
            Received,
            Refunded,
            Replaced,
            Rejected,
        }

        public enum PaymentStatus
        {
            Pending,
            Completed,
            Failed,
            Refunded,
        }

        public enum ShippingStatus
        {
            Shipped
        }

        public enum ReturnType
        {
            Return = 1,
            Replace = 2
        }

        public enum ShippingType
        {
            Forward = 1,
            Reverse = 2
        }

        public enum ReturnStatus
        {
            Requested = 1,
            Approved = 2,
            PickupScheduled = 3,
            PickedUp = 4,
            Received = 5,
            Refunded = 6,
            Replaced = 7,
            Rejected = 8
        }
    }
}
