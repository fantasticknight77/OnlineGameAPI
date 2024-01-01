namespace OnlineGameAPI.Models.Responses
{
    public class GetTransactionHistoryResponse
    {
        public List<TransactionDto>? TransactionHistory { get; set; }

        public decimal TotalPurchaseAmount { get; set; }

        public decimal TotalDiscountAmount { get; set; }

        public decimal TotalDiscountedAmount { get; set; }

        public GetTransactionHistoryResponse(List<TransactionDto>? transactionHistory, decimal totalPurchaseAmount, decimal totalDiscountAmount, decimal totalDiscountedAmount)
        {
            TransactionHistory = transactionHistory;
            TotalPurchaseAmount = totalPurchaseAmount;
            TotalDiscountAmount = totalDiscountAmount;
            TotalDiscountedAmount = totalDiscountedAmount;
        }
    }
}
