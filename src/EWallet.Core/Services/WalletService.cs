using EWallet.Core.Interfaces.Data;
using EWallet.Core.Interfaces.Repositories;
using EWallet.Core.Interfaces.Services;
using EWallet.Core.Models;
using EWallet.Core.Enums;
using Microsoft.Data.SqlClient;

namespace EWallet.Core.Services;

public class WalletService(IWalletRepository walletRepository, ITransactionRepository transactionRepository, IDbConnectionFactory connectionFactory) : IWalletService
{
    private readonly IWalletRepository _walletRepository = walletRepository;
    private readonly ITransactionRepository _transactionRepository = transactionRepository;
    private readonly IDbConnectionFactory _connectionFactory = connectionFactory;
    public async Task<Wallet?> GetByUserIdAsync(int userId)
    {
        return await _walletRepository.GetByUserIdAsync(userId);
    }

    public async Task<bool> TopUpAsync(int userId, decimal amount, string? description)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Top up amount must be greater than zero.");
        }

        var wallet = await _walletRepository.GetByUserIdAsync(userId);

        if (wallet is null)
        {
            return false;
        }

        await using var connection = _connectionFactory.CreateConnection();

        await connection.OpenAsync();

        await using SqlTransaction transaction =
            (SqlTransaction)await connection.BeginTransactionAsync();

        try
        {
            var newBalance = wallet.Balance + amount;

            var isBalanceUpdated = await _walletRepository.AddBalanceAsync(
                wallet.Id,
                amount,
                connection,
                transaction
            );

            if (!isBalanceUpdated)
            {
                throw new InvalidOperationException("Failed to update wallet balance.");
            }

            var newTransaction = new Transaction
            {
                SenderWalletId = null,
                ReceiverWalletId = wallet.Id,
                Type = TransactionType.TopUp,
                Amount = amount,
                Description = description,
                CreatedAt = DateTime.UtcNow
            };

            await _transactionRepository.CreateAsync(
                newTransaction,
                connection,
                transaction
            );

            await transaction.CommitAsync();

            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}