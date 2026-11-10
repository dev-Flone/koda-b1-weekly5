using EWallet.Core.Models;
using Microsoft.Data.SqlClient;

namespace EWallet.Core.Interfaces.Repositories;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int id);
    Task<int> CreateAsync(User user);
    Task<int> CreateAsync(User user, SqlConnection connection, SqlTransaction transaction);
    Task<bool> UpdateAsync(User user);
    Task<bool> EmailExistsAsync(string email);
    Task<bool> EmailExistsAsync(string email, int excludeUserId);
}