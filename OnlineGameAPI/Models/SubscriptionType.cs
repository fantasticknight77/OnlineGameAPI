using System.ComponentModel.DataAnnotations;

namespace OnlineGameAPI.Models
{
    public class SubscriptionType
    {
        public int SubscriptionTypeID { get; set; }

        public string SubscriptionTypeName { get; set; } = string.Empty;

        public decimal Cost { get; set; }

        public decimal Discount { get; set; }

        public decimal TopUpLimit { get; set; }

        public decimal PurchaseLimit { get; set; }
    }
}
