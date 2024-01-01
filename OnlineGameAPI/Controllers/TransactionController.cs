using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineGameAPI.Models.Requests;
using OnlineGameAPI.Models.Responses;
using OnlineGameAPI.Services;

namespace OnlineGameAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly IAccountService _accountService;

        private readonly ITransactionService _transactionService;

        public TransactionController(IAccountService accountService, ITransactionService transactionService)
        {
            _accountService = accountService;
            _transactionService = transactionService;
        }

        [HttpPost("Purchase")]
        [Authorize]
        public async Task<ActionResult<PurchaseResponse>> Purchase(PurchaseRequest request)
        {
            var username = _accountService.GetUserNameInJWT();

            var response = await _transactionService.Purchase(username, request.Amount);

            if(response.DiscountedAmount == decimal.Zero)
            {
                return BadRequest("Purchase unsuccessfully");
            }

            return Ok(response);
        }

        [HttpPost("transactionhistory")]
        [Authorize]
        public async Task<ActionResult<GetTransactionHistoryResponse>> GetTransactionHistory(GetTransactionHistoryRequest request)
        {
            var username = _accountService.GetUserNameInJWT();

            return Ok(await _transactionService.GetTransactionHistory(username, request));
        }
    }
}
