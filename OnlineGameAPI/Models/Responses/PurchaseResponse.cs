namespace OnlineGameAPI.Models.Responses
{
    public class PurchaseResponse
    {
        public decimal DiscountAmount { get; set; }

        public decimal DiscountedAmount { get; set; }

        public decimal BalanceBefore { get; set; }

        public decimal BalanceAfter { get; set; }

        public PurchaseResponse(decimal discountAmount, decimal discountedAmount, decimal balanceBefore, decimal balanceAfter)
        {
            DiscountAmount = discountAmount;
            DiscountedAmount = discountedAmount;
            BalanceBefore = balanceBefore;
            BalanceAfter = balanceAfter;
        }
    }
}
