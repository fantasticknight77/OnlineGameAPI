namespace OnlineGameAPI.Models.Requests
{
    public class GetTransactionHistoryRequest
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }
    }
}
