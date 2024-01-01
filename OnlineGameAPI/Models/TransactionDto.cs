namespace OnlineGameAPI.Models
{
    public class TransactionDto
    {
        public decimal PurchaseAmount { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal DiscountedAmount { get; set; }

        public decimal BalanceBefore { get; set; }

        public decimal BalanceAfter { get; set; }

        public DateTime TransactionDateTime { get; set; }
    }
}
