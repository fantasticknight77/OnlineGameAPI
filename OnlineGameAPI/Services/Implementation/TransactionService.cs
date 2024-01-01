// <copyright file="TransactionService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace OnlineGameAPI.Services.Implementation
{
    using Microsoft.EntityFrameworkCore;
    using OnlineGameAPI.Data;
    using OnlineGameAPI.Models;
    using OnlineGameAPI.Models.Requests;
    using OnlineGameAPI.Models.Responses;

    /// <summary>
    ///   <br />
    /// </summary>
    public class TransactionService : ITransactionService
    {
        private readonly OnlineGameContext dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionService"/> class.
        /// </summary>
        /// <param name="dbContext">Congiguration.</param>
        public TransactionService(OnlineGameContext dbContext)
        {
            this.dbContext = dbContext;
        }

        /// <summary>Purchases the specified username.</summary>
        /// <param name="username">The username.</param>
        /// <param name="amount">The amount.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public async Task<PurchaseResponse> Purchase(string username, decimal amount)
        {
            var user = await this.dbContext.Users
                .Include(x => x.SubscriptionType)
                .SingleOrDefaultAsync(u => u.Username == username);

            if (user!.Balance < amount)
            {
                return new PurchaseResponse(decimal.Zero, decimal.Zero, user.Balance, user.Balance);
            }

            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var transactionHistory = await this.dbContext.Transactions
                .Where(t => t.UserID == user!.UserID)
                .Where(t => t.TransactionDateTime >= today)
                .Where(t => t.TransactionDateTime <= tomorrow)
                .ToListAsync();

            var totalPurchasedToday = transactionHistory.Sum(x => x.DiscountedAmount);

            if (user.SubscriptionType!.PurchaseLimit < totalPurchasedToday)
            {
                return new PurchaseResponse(decimal.Zero, decimal.Zero, user.Balance, user.Balance);
            }

            var discountRate = user.SubscriptionType.Discount;

            var discountAmount = amount * discountRate;

            var discountedAmount = amount - discountAmount;

            var balanceBefore = user.Balance;

            var balanceAfter = user.Balance - discountedAmount;

            user.Balance = balanceAfter;

            var transaction = new Transaction
            {
                PurchaseAmount = amount,
                DiscountAmount = discountAmount,
                DiscountedAmount = discountedAmount,
                BalanceBefore = balanceBefore,
                BalanceAfter = balanceAfter,
                TransactionDateTime = DateTime.Now,
                UserID = user.UserID,
            };

            this.dbContext.Transactions.Add(transaction);

            await this.dbContext.SaveChangesAsync();

            return new PurchaseResponse(discountAmount, discountedAmount, balanceBefore, balanceAfter);
        }

        /// <summary>Gets the transaction history.</summary>
        /// <param name="username">The username.</param>
        /// <param name="request">The request.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public async Task<GetTransactionHistoryResponse> GetTransactionHistory(string username, GetTransactionHistoryRequest request)
        {
            var user = await this.dbContext.Users.SingleOrDefaultAsync(u => u.Username == username);

            var fromDate = request.FromDate;

            var toDate = request.ToDate;

            var transactionHistory = await this.dbContext.Transactions
                .Where(t => t.UserID == user!.UserID)
                .Where(t => t.TransactionDateTime >= fromDate)
                .Where(t => t.TransactionDateTime <= toDate)
                .ToListAsync();

            var transactionDtoList = transactionHistory
                .Select(x => new TransactionDto
                {
                    PurchaseAmount = x.PurchaseAmount,
                    DiscountAmount = x.DiscountAmount,
                    DiscountedAmount = x.DiscountedAmount,
                    BalanceBefore = x.BalanceBefore,
                    BalanceAfter = x.BalanceAfter,
                    TransactionDateTime = x.TransactionDateTime,
                })
                .ToList();

            var totalPurchaseAmount = transactionDtoList.Sum(x => x.PurchaseAmount);

            var totalDiscountAmount = transactionDtoList.Sum(x => x.DiscountAmount);

            var totalDiscountedAmount = transactionDtoList.Sum(x => x.DiscountedAmount);

            return new GetTransactionHistoryResponse(transactionDtoList, totalPurchaseAmount, totalDiscountAmount, totalDiscountedAmount);
        }
    }
}
