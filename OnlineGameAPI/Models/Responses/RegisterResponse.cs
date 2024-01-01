namespace OnlineGameAPI.Models.Responses
{
    public class RegisterResponse
    {
        public bool isSuccess { get; set; }

        public RegisterResponse(bool isSuccess)
        {
            this.isSuccess = isSuccess;
        }
    }
}
