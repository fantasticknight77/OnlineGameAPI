namespace OnlineGameAPI.Models.Responses
{
    public class LoginResponse
    {
        public string JWT { get; set; }

        public LoginResponse(string jWT)
        {
            JWT = jWT;
        }
    }
}
