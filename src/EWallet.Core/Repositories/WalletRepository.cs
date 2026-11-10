using EWallet.Core.Interfaces.Data;
using EWallet.Core.Interfaces.Repositories;
using EWallet.Core.Models;
using Microsoft.Data.SqlClient;

namespace EWallet.Core.Repositories;

public class WalletRepository(IDbConnectionFactory connectionFactory) : IWalletRepository
{
    private readonly IDbConnectionFactory _connectionFactory = connectionFactory;

    public async Task<bool> AddBalanceAsync(int walletId, decimal amount, SqlConnection connection, SqlTransaction transaction)
    {
        const string query = """
            UPDATE Wallets
            SET Balance = Balance + @Amount
            WHERE Id = @WalletId;
        """;

        await using var command = new SqlCommand(query, connection, transaction);

        command.Parameters.AddWithValue("@WalletId", walletId);
        command.Parameters.AddWithValue("@Amount", amount);

        var affectedRows = await command.ExecuteNonQueryAsync();

        return affectedRows > 0;
    }

    public async Task<int> CreateAsync(Wallet wallet)
    {
        const string query = """
            INSERT INTO Wallets(UserId, WalletNumber, Balance)
            VALUES (@UserId, @WalletNumber, @Balance);

            SELECT CAST(SCOPE_IDENTITY() AS INT);
        """;

        await using var connection = _connectionFactory.CreateConnection();

        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@UserId", wallet.UserId);
        command.Parameters.AddWithValue("@WalletNumber", wallet.WalletNumber);
        command.Parameters.AddWithValue("@Balance", wallet.Balance);

        var result = await command.ExecuteScalarAsync();

        return Convert.ToInt32(result);
    }

    public async Task<int> CreateAsync(Wallet wallet, SqlConnection connection, SqlTransaction transaction)
    {
        const string query = """
            INSERT INTO Wallets(UserId, WalletNumber, Balance)
            VALUES (@UserId, @WalletNumber, @Balance);

            SELECT CAST(SCOPE_IDENTITY() AS INT);
        """;

        await using var command = new SqlCommand(query, connection, transaction);

        command.Parameters.AddWithValue("@UserId", wallet.UserId);
        command.Parameters.AddWithValue("@WalletNumber", wallet.WalletNumber);
        command.Parameters.AddWithValue("@Balance", wallet.Balance);

        var result = await command.ExecuteScalarAsync();

        return Convert.ToInt32(result);
    }

    public async Task<bool> DebitBalanceAsync(int walletId, decimal amount, SqlConnection connection, SqlTransaction transaction)
    {
        const string query = """
            UPDATE Wallets
            SET Balance = Balance - @Amount
            WHERE Id = @WalletId AND Balance >= @Amount;
        """;

        await using var command = new SqlCommand(query, connection, transaction);

        command.Parameters.AddWithValue("@WalletId", walletId);
        command.Parameters.AddWithValue("@Amount", amount);

        var affectedRows = await command.ExecuteNonQueryAsync();

        return affectedRows > 0;
    }

    public async Task<Wallet?> GetByIdAsync(int id)
    {
        const string query = """
            SELECT Id, UserId, WalletNumber, Balance
            FROM Wallets
            WHERE Id = @Id;
        """;

        await using var connection = _connectionFactory.CreateConnection();

        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@Id", id);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new Wallet
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
            WalletNumber = reader.GetString(reader.GetOrdinal("WalletNumber")),
            Balance = reader.GetDecimal(reader.GetOrdinal("Balance")),
        };
    }

    public async Task<Wallet?> GetByUserIdAsync(int userId)
    {
        const string query = """
            SELECT Id, UserId, WalletNumber, Balance
            FROM Wallets
            WHERE UserId = @userId;
        """;

        await using var connection = _connectionFactory.CreateConnection();

        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@UserId", userId);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new Wallet
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
            WalletNumber = reader.GetString(reader.GetOrdinal("WalletNumber")),
            Balance = reader.GetDecimal(reader.GetOrdinal("Balance")),
        };
    }

    public async Task<bool> UpdateBalanceAsync(int walletId, decimal balance)
    {
        const string query = """
            UPDATE Wallets
            SET Balance = @Balance
            WHERE Id = @WalletId;
        """;

        await using var connection = _connectionFactory.CreateConnection();

        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@WalletId", walletId);
        command.Parameters.AddWithValue("@Balance", balance);

        var affectedRows = await command.ExecuteNonQueryAsync();

        return affectedRows > 0;
    }
    public async Task<bool> UpdateBalanceAsync(int walletId, decimal balance, SqlConnection connection, SqlTransaction transaction)
    {
        const string query = """
            UPDATE Wallets
            SET Balance = @Balance
            WHERE Id = @WalletId;
        """;

        await using var command = new SqlCommand(query, connection, transaction);

        command.Parameters.AddWithValue("@WalletId", walletId);
        command.Parameters.AddWithValue("@Balance", balance);

        var affectedRows = await command.ExecuteNonQueryAsync();

        return affectedRows > 0;
    }
}