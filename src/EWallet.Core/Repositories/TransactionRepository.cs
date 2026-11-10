using EWallet.Core.Enums;
using EWallet.Core.Interfaces.Data;
using EWallet.Core.Interfaces.Repositories;
using EWallet.Core.Models;
using Microsoft.Data.SqlClient;

namespace EWallet.Core.Repositories;

public class TransactionRepository(IDbConnectionFactory connectionFactory) : ITransactionRepository
{
    private readonly IDbConnectionFactory _connectionFactory = connectionFactory;
    public async Task<int> CreateAsync(Transaction transaction)
    {
        const string query = """
            INSERT INTO Transactions(SenderWalletId, ReceiverWalletId, Type, Amount, Description)
            VALUES (@SenderWalletId, @ReceiverWalletId, @Type, @Amount, @Description);

            SELECT CAST(SCOPE_IDENTITY() AS INT);
        """;

        await using var connection = _connectionFactory.CreateConnection();

        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@SenderWalletId", transaction.SenderWalletId.HasValue ? transaction.SenderWalletId : DBNull.Value);
        command.Parameters.AddWithValue("@ReceiverWalletId", transaction.ReceiverWalletId);
        command.Parameters.AddWithValue("@Type", (int)transaction.Type);
        command.Parameters.AddWithValue("@Amount", transaction.Amount);
        command.Parameters.AddWithValue("@Description", (object?)transaction.Description ?? DBNull.Value);

        var result = await command.ExecuteScalarAsync();

        return Convert.ToInt32(result);
    }
    public async Task<int> CreateAsync(Transaction transaction, SqlConnection connection, SqlTransaction dbTransaction)
    {
        const string query = """
            INSERT INTO Transactions(SenderWalletId, ReceiverWalletId, Type, Amount, Description)
            VALUES (@SenderWalletId, @ReceiverWalletId, @Type, @Amount, @Description);

            SELECT CAST(SCOPE_IDENTITY() AS INT);
        """;

        await using var command = new SqlCommand(query, connection, dbTransaction);

        command.Parameters.AddWithValue("@SenderWalletId", transaction.SenderWalletId.HasValue ? transaction.SenderWalletId : DBNull.Value);
        command.Parameters.AddWithValue("@ReceiverWalletId", transaction.ReceiverWalletId);
        command.Parameters.AddWithValue("@Type", (int)transaction.Type);
        command.Parameters.AddWithValue("@Amount", transaction.Amount);
        command.Parameters.AddWithValue("@Description", (object?)transaction.Description ?? DBNull.Value);

        var result = await command.ExecuteScalarAsync();

        return Convert.ToInt32(result);
    }

    public async Task<List<Transaction>> GetByUserIdAsync(int userId)
    {
        var transactions = new List<Transaction>();

        const string query = """
            SELECT t.Id, t.SenderWalletId, t.ReceiverWalletId, t.Type, t.Amount, t.Description, t.CreatedAt
            FROM Transactions t
            JOIN Wallets w 
                ON t.ReceiverWalletId = w.Id
            WHERE w.UserId = @UserId

            UNION

            SELECT t.Id, t.SenderWalletId, t.ReceiverWalletId, t.Type, t.Amount, t.Description, t.CreatedAt
            FROM Transactions t
            JOIN Wallets w
                ON t.SenderWalletId = w.Id
            WHERE w.UserId = @UserId

            ORDER BY CreatedAt DESC;
        """;

        await using var connection = _connectionFactory.CreateConnection();

        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@UserId", userId);

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            transactions.Add(new Transaction
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                SenderWalletId = reader.IsDBNull(reader.GetOrdinal("SenderWalletId")) ? null : reader.GetInt32(reader.GetOrdinal("SenderWalletId")),
                ReceiverWalletId = reader.GetInt32(reader.GetOrdinal("ReceiverWalletId")),
                Type = (TransactionType)reader.GetInt32(reader.GetOrdinal("Type")),
                Amount = reader.GetDecimal(reader.GetOrdinal("Amount")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
            });
        }

        return transactions;
    }
}