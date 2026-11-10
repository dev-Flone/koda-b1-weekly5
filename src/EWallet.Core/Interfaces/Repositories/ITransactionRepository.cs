using EWallet.Core.Models;
using Microsoft.Data.SqlClient;

namespace EWallet.Core.Interfaces.Repositories;

public interface ITransactionRepository
{
    Task<int> CreateAsync(Transaction transaction, SqlConnection connection, SqlTransaction dbTransaction);
    Task<int> CreateAsync(Transaction transaction);
    Task<List<Transaction>> GetByUserIdAsync(int userId);
}