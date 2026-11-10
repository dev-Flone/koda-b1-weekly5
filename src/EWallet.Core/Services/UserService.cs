using EWallet.Core.Interfaces.Data;
using EWallet.Core.Interfaces.Repositories;
using EWallet.Core.Interfaces.Services;
using EWallet.Core.Models;
using Microsoft.Data.SqlClient;

namespace EWallet.Core.Services;

public class UserService(IUserRepository userRepository, IWalletRepository walletRepository, IDbConnectionFactory connectionFactory) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IWalletRepository _walletRepository = walletRepository;
    private readonly IDbConnectionFactory _connectionFactory = connectionFactory;
    public async Task<int> CreateAsync(User user)
    {
        var emailExists = await _userRepository.EmailExistsAsync(user.Email);

        if (emailExists)
        {
            throw new InvalidOperationException("Email is already used by another user.");
        }

        await using var connection = _connectionFactory.CreateConnection();

        await connection.OpenAsync();

        await using SqlTransaction transaction = (SqlTransaction)await connection.BeginTransactionAsync();

        try
        {
            var userId = await _userRepository.CreateAsync(user, connection, transaction);
            var wallet = new Wallet
            {
                UserId = userId,
                WalletNumber = $"WLT{userId:D3}",
                Balance = 0
            };
            await _walletRepository.CreateAsync(wallet, connection, transaction);
            await transaction.CommitAsync();
            return userId;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _userRepository.GetAllAsync();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _userRepository.GetByIdAsync(id);
    }

    public async Task<bool> UpdateAsync(User user)
    {
        var existingUser = await _userRepository.GetByIdAsync(user.Id);

        if (existingUser is null)
        {
            return false;
        }

        var emailExists = await _userRepository.EmailExistsAsync(user.Email, user.Id);

        if (emailExists)
        {
            throw new InvalidOperationException(
                "Email is already used by another user."
            );
        }

        return await _userRepository.UpdateAsync(user);
    }
}