using EWallet.Core.Interfaces.Data;
using EWallet.Core.Interfaces.Repositories;
using EWallet.Core.Models;
using Microsoft.Data.SqlClient;

namespace EWallet.Core.Repositories;

public class UserRepository(IDbConnectionFactory connectionFactory) : IUserRepository
{
    private readonly IDbConnectionFactory _connectionFactory = connectionFactory;
    public async Task<int> CreateAsync(User user)
    {
        const string query = """
            INSERT INTO Users (Name, Email, Phone)
            VALUES (@Name, @Email, @Phone);

            SELECT CAST(SCOPE_IDENTITY() AS INT);
        """;

        await using var connection = _connectionFactory.CreateConnection();

        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@Name", user.Name);
        command.Parameters.AddWithValue("@Email", user.Email);
        command.Parameters.AddWithValue("@Phone", user.Phone);

        var result = await command.ExecuteScalarAsync();

        return Convert.ToInt32(result);
    }

    public async Task<int> CreateAsync(User user, SqlConnection connection, SqlTransaction transaction)
    {
        const string query = """
            INSERT INTO Users (Name, Email, Phone)
            VALUES (@Name, @Email, @Phone);

            SELECT CAST(SCOPE_IDENTITY() AS INT);
        """;

        await using var command = new SqlCommand(query, connection, transaction);

        command.Parameters.AddWithValue("@Name", user.Name);
        command.Parameters.AddWithValue("@Email", user.Email);
        command.Parameters.AddWithValue("@Phone", user.Phone);

        var result = await command.ExecuteScalarAsync();

        return Convert.ToInt32(result);
    }

    public async Task<bool> EmailExistsAsync(string email, int excludeUserId)
    {
        const string query = """
            SELECT COUNT(1)
            FROM Users
            WHERE Email = @Email AND Id <> @ExcludeUserId;
        """;

        await using var connection = _connectionFactory.CreateConnection();

        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@Email", email);
        command.Parameters.AddWithValue("@ExcludeUserId", excludeUserId);

        var result = await command.ExecuteScalarAsync();

        return Convert.ToInt32(result) > 0;
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        const string query = """
            SELECT COUNT(1)
            FROM Users
            WHERE Email = @Email;
        """;

        await using var connection = _connectionFactory.CreateConnection();

        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@Email", email);

        var result = await command.ExecuteScalarAsync();

        return Convert.ToInt32(result) > 0;
    }

    public async Task<List<User>> GetAllAsync()
    {
        var users = new List<User>();

        const string query = """
            SELECT Id, Name, Email, Phone
            FROM Users
            ORDER BY Id;
        """;

        await using var connection = _connectionFactory.CreateConnection();

        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            users.Add(new User
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                Phone = reader.GetString(reader.GetOrdinal("Phone"))
            }
            );
        }
        return users;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        const string query = """
            SELECT Id, Name, Email, Phone
            FROM Users
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

        return new User
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            Name = reader.GetString(reader.GetOrdinal("Name")),
            Email = reader.GetString(reader.GetOrdinal("Email")),
            Phone = reader.GetString(reader.GetOrdinal("Phone"))
        };
    }

    public async Task<bool> UpdateAsync(User user)
    {
        const string query = """
            UPDATE Users
            SET 
                Name = @Name,
                Email = @Email,
                Phone = @Phone
            WHERE Id = @Id;
        """;

        await using var connection = _connectionFactory.CreateConnection();

        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@Id", user.Id);
        command.Parameters.AddWithValue("@Name", user.Name);
        command.Parameters.AddWithValue("@Email", user.Email);
        command.Parameters.AddWithValue("@Phone", user.Phone);

        var affectedRows = await command.ExecuteNonQueryAsync();

        return affectedRows > 0;
    }
}