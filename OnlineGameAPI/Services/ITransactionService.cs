using OnlineGameAPI.Models.Requests;
using OnlineGameAPI.Models.Responses;

namespace OnlineGameAPI.Services
{
    public interface ITransactionService
    {
        Task<PurchaseResponse> Purchase(string username, decimal amount);

        Task<GetTransactionHistoryResponse> GetTransactionHistory(string username, GetTransactionHistoryRequest request);
    }
}
