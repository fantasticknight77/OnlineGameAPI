namespace OnlineGameAPI.Models.Responses
{
    public class SubscribeServiceResponse
    {
        public string Message { get; set; } = string.Empty;

        public SubscribeServiceResponse(string message)
        {
            Message = message;
        }
    }
}
