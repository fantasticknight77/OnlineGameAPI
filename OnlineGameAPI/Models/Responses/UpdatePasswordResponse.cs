namespace OnlineGameAPI.Models.Responses
{
    public class UpdatePasswordResponse
    {
        public string Message { get; set; } = string.Empty;

        public UpdatePasswordResponse(string message)
        {
            Message = message;
        }
    }
}
