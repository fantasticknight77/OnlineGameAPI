using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineGameAPI.Models;
using OnlineGameAPI.Models.Requests;
using OnlineGameAPI.Models.Responses;
using OnlineGameAPI.Services;
using Swashbuckle.AspNetCore.Filters;

namespace OnlineGameAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<RegisterResponse>> RegisterUser(RegisterRequest request)
        {
            var user = await _accountService.GetUserByUsername(request.Username);

            if(user != null)
            {
                return BadRequest("Username existed, please change another username.");
            }

            return Ok(await _accountService.Register(request));
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
        {
            var user = await _accountService.GetUserByUsername(request.Username);

            if (user == null)
            {
                return BadRequest("User not found.");
            }

            if (!_accountService.VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Wrong password.");
            }

            return Ok(_accountService.Login(user));
        }

        [HttpPost("updatepassword")]
        [Authorize]
        public async Task<ActionResult<UpdatePasswordResponse>> UpdatePassword(UpdatePasswordRequest request)
        {
            if (request.NewPassword == null)
            {
                return BadRequest("Empty Request Body");
            }

            var username = _accountService.GetUserNameInJWT();

            return Ok(await _accountService.UpdatePassword(username, request.NewPassword));
        }

        [HttpPost("topup")]
        [Authorize]
        public async Task<ActionResult<TopUpResponse>> TopUp(TopUpRequest request)
        {
            var username = _accountService.GetUserNameInJWT();

            var response = await _accountService.TopUp(username, request.Amount);

            if (response.BalanceBefore == response.BalanceAfter)
            {
                return BadRequest("Exceed top up limit");
            }

            return Ok(response);
        }

        [HttpPost("subscribeservice")]
        [Authorize]
        public async Task<ActionResult<SubscribeServiceResponse>> SubscribeService(SubscribeServiceRequest request)
        {
            if (request.SubscriptionType == null)
            {
                return BadRequest("Empty Request Body");
            }

            var username = _accountService.GetUserNameInJWT();

            return Ok(await _accountService.SubscribeService(username, request.SubscriptionType.SubscriptionTypeName));
        }

    }
}
