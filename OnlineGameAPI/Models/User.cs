namespace OnlineGameAPI.Models
{
    public class User
    {
        public int UserID { get; set; }

        public string Username { get; set; } = string.Empty;

        public byte[] PasswordHash { get; set; } = new byte[32];

        public byte[] PasswordSalt { get; set; } = new byte[32];

        public string FullName { get; set; } = string.Empty;

        public decimal Balance { get; set; } = decimal.Zero;

        public int SubscriptionTypeID { get; set; }

        public SubscriptionType? SubscriptionType { get; set; }

    }
}
