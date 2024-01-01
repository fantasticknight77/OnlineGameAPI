namespace OnlineGameAPI.Models
{
    public class Transaction
    {
        public int TransactionID { get; set; }

        public decimal PurchaseAmount { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal DiscountedAmount { get; set; }

        public decimal BalanceBefore { get; set; }

        public decimal BalanceAfter { get; set;}

        public DateTime TransactionDateTime { get; set; }

        public int UserID { get; set; }

        public User? User { get; set; }
    }
}
