using Microsoft.AspNetCore.Mvc;
using OnlineGameAPI.Models;
using OnlineGameAPI.Models.Requests;
using OnlineGameAPI.Models.Responses;

namespace OnlineGameAPI.Services
{
    public interface IAccountService
    {
        Task<RegisterResponse> Register(RegisterRequest request);

        Task<User?> GetUserByUsername(string username);

        bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt);

        LoginResponse Login(User user);

        string GetUserNameInJWT();

        Task<UpdatePasswordResponse> UpdatePassword(string username, string newPassword);

        Task<TopUpResponse> TopUp(string username, decimal amount);

        Task<SubscribeServiceResponse> SubscribeService(string username, string subscriptionTypeName);

    }
}
