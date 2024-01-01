namespace OnlineGameAPI.Models.Responses
{
    public class TopUpResponse
    {
        public decimal BalanceBefore { get; set; }

        public decimal BalanceAfter { get; set; }

        public TopUpResponse(decimal balanceBefore, decimal balanceAfter)
        {
            BalanceBefore = balanceBefore;
            BalanceAfter = balanceAfter;
        }
    }
}
