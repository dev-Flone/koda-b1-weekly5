using EWallet.Core.Enums;
using EWallet.Core.Interfaces.Data;
using EWallet.Core.Interfaces.Repositories;
using EWallet.Core.Interfaces.Services;
using EWallet.Core.Models;
using Microsoft.Data.SqlClient;
using EWallet.Core.Exceptions;

namespace EWallet.Core.Services;

public class TransactionService(ITransactionRepository transactionRepository, IWalletRepository walletRepository, IDbConnectionFactory connectionFactory) : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository = transactionRepository;
    private readonly IWalletRepository _walletRepository = walletRepository;
    private readonly IDbConnectionFactory _connectionFactory = connectionFactory;
    public async Task<List<Transaction>> GetByUserId(int userId)
    {
        return await _transactionRepository.GetByUserIdAsync(userId);
    }

    public async Task<bool> TransferAsync(int senderUserId, int receiverUserId, decimal amount, string? description)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Transfer amount must be greater than 0.");
        }

        if (senderUserId == receiverUserId)
        {
            throw new ArgumentException("Cannot transfer to your own Wallet.");
        }

        var senderWallet = await _walletRepository.GetByUserIdAsync(senderUserId);
        var receiverWallet = await _walletRepository.GetByUserIdAsync(receiverUserId);

        if (senderWallet is null || receiverWallet is null)
        {
            return false;
        }

        if (senderWallet.Balance < amount)
        {
            throw new InsufficientBalanceException("Insufficient wallet balance");
        }

        await using var connection = _connectionFactory.CreateConnection();

        await connection.OpenAsync();

        await using SqlTransaction transaction = (SqlTransaction)await connection.BeginTransactionAsync();

        try
        {
            var isSenderUpdated = await _walletRepository.DebitBalanceAsync(senderWallet.Id, amount, connection, transaction);

            if (!isSenderUpdated)
            {
                throw new InvalidOperationException("Insufficient wallet balance");
            }

            var isReceiverUpdated = await _walletRepository.AddBalanceAsync(receiverWallet.Id, amount, connection, transaction);

            if (!isReceiverUpdated)
            {
                throw new InvalidOperationException("Failed to update receiver balance.");
            }

            var newTransaction = new Transaction
            {
                SenderWalletId = senderWallet.Id,
                ReceiverWalletId = receiverWallet.Id,
                Type = TransactionType.Transfer,
                Amount = amount,
                Description = description,
                CreatedAt = DateTime.UtcNow
            };

            await _transactionRepository.CreateAsync(newTransaction, connection, transaction);

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