// <copyright file="AccountService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace OnlineGameAPI.Services.Implementation
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Security.Cryptography;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.IdentityModel.Tokens;
    using OnlineGameAPI.Data;
    using OnlineGameAPI.Models;
    using OnlineGameAPI.Models.Requests;
    using OnlineGameAPI.Models.Responses;

    /// <summary>
    ///   <br />
    /// </summary>
    public class AccountService : IAccountService
    {
        private readonly OnlineGameContext dbContext;

        private readonly IConfiguration configuration;

        private readonly IHttpContextAccessor httpContextAccessor;

        /// <summary>Initializes a new instance of the <see cref="AccountService" /> class.</summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        public AccountService(OnlineGameContext dbContext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            this.dbContext = dbContext;
            this.configuration = configuration;
            this.httpContextAccessor = httpContextAccessor;
        }

        /// <summary>Registers the specified request.</summary>
        /// <param name="request">The request.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public async Task<RegisterResponse> Register(RegisterRequest request)
        {
            var user = new User();

            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            user.Username = request.Username;
            user.FullName = request.FullName;
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.Balance = decimal.Zero;
            user.SubscriptionTypeID = 1;

            this.dbContext.Users.Add(user);
            int isSuccess = await this.dbContext.SaveChangesAsync();

            return isSuccess > 0 ? new RegisterResponse(true) : new RegisterResponse(false);
        }

        /// <summary>Verifies the password.</summary>
        /// <param name="password">The password.</param>
        /// <param name="passwordHash">The password hash.</param>
        /// <param name="passwordSalt">The password salt.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512(passwordSalt);
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }

        /// <summary>Logins the specified user.</summary>
        /// <param name="user">The user.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public LoginResponse Login(User user)
        {
            List<Claim> claims = new ()
            {
                new Claim(ClaimTypes.Name, user.Username),
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(this.configuration.GetSection("AppSettings:Token").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return new LoginResponse(jwt);
        }

        /// <summary>Gets the user by username.</summary>
        /// <param name="username">The username.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public async Task<User?> GetUserByUsername(string username)
        {
            var user = await this.dbContext.Users.SingleOrDefaultAsync(u => u.Username == username);

            return user;
        }

        /// <summary>Gets the user name in JWT.</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        public string GetUserNameInJWT()
        {
            var result = string.Empty;
            if (this.httpContextAccessor.HttpContext != null)
            {
                result = this.httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
            }

            return result;
        }

        /// <summary>Updates the password.</summary>
        /// <param name="username">The username.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public async Task<UpdatePasswordResponse> UpdatePassword(string username, string newPassword)
        {
            var user = await this.dbContext.Users.SingleOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                return new UpdatePasswordResponse("User not found.");
            }

            CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            int isSuccess = await this.dbContext.SaveChangesAsync();

            return isSuccess > 0 ? new UpdatePasswordResponse("Password updated.") : new UpdatePasswordResponse("Password update failed.");
        }

        /// <summary>Tops up.</summary>
        /// <param name="username">The username.</param>
        /// <param name="amount">The amount.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public async Task<TopUpResponse> TopUp(string username, decimal amount)
        {
            var user = await this.dbContext.Users
                .Include(x => x.SubscriptionType)
                .SingleOrDefaultAsync(u => u.Username == username);

            var limit = user!.SubscriptionType!.TopUpLimit;

            var balanceBefore = user.Balance;

            var balanceAfter = balanceBefore + amount;

            if (balanceAfter > limit)
            {
                return new TopUpResponse(balanceBefore, balanceBefore);
            }

            user.Balance = balanceAfter;

            await this.dbContext.SaveChangesAsync();

            TopUpResponse topUp = new (balanceBefore, balanceAfter);

            return topUp;
        }

        /// <summary>Subscribes the service.</summary>
        /// <param name="username">The username.</param>
        /// <param name="subscriptionTypeName">Name of the subscription type.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public async Task<SubscribeServiceResponse> SubscribeService(string username, string subscriptionTypeName)
        {
            if (subscriptionTypeName.ToLower() == "free")
            {
                return new SubscribeServiceResponse("Cannot subsribe to free subscription type.");
            }

            var subsriptionToSubsribe = await this.dbContext.Subscriptions.SingleOrDefaultAsync(s => s.SubscriptionTypeName.ToLower() == subscriptionTypeName.ToLower());

            if (subsriptionToSubsribe == null)
            {
                return new SubscribeServiceResponse("Subscription type not found.");
            }

            var user = await this.dbContext.Users.SingleOrDefaultAsync(u => u.Username == username);

            var currentSubscriptionID = user!.SubscriptionTypeID;

            if (currentSubscriptionID == subsriptionToSubsribe.SubscriptionTypeID)
            {
                return new SubscribeServiceResponse("Already subsribe to the service.");
            }

            var balance = user.Balance;

            if (balance < subsriptionToSubsribe.Cost)
            {
                return new SubscribeServiceResponse("Insufficient fund.");
            }

            balance -= subsriptionToSubsribe.Cost;

            user.SubscriptionTypeID = subsriptionToSubsribe.SubscriptionTypeID;
            user.Balance = balance;

            int isSuccess = await this.dbContext.SaveChangesAsync();

            return isSuccess > 0 ? new SubscribeServiceResponse("Subscribed to service.") : new SubscribeServiceResponse("Unable to subscribe service");
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }
}
