using EWallet.Core.Models;

namespace EWallet.Core.Interfaces.Services;

public interface ITransactionService
{
    Task<bool> TransferAsync(int senderUserId, int receiverUserId, decimal amount, string? description);
    Task<List<Transaction>> GetByUserId(int userId);
}