using EWallet.Core.Models;
using Microsoft.Data.SqlClient;

namespace EWallet.Core.Interfaces.Repositories;

public interface IWalletRepository
{
    Task<Wallet?> GetByIdAsync(int id);
    Task<Wallet?> GetByUserIdAsync(int userId);
    Task<int> CreateAsync(Wallet wallet, SqlConnection connection, SqlTransaction transaction);
    Task<bool> UpdateBalanceAsync(int walletId, decimal balance);
    Task<bool> UpdateBalanceAsync(int walletId, decimal balance, SqlConnection connection, SqlTransaction transaction);
    Task<bool> AddBalanceAsync(int walletId, decimal amount, SqlConnection connection, SqlTransaction transaction);
    Task<bool> DebitBalanceAsync(int walletId, decimal amount, SqlConnection connection, SqlTransaction transaction);
}