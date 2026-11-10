using EWallet.Core.Interfaces.Data;
using Microsoft.Data.SqlClient;

namespace EWallet.Core.Data;

public class DbConnectionFactory(string connectionString) : IDbConnectionFactory
{
    private readonly string _connectionString = connectionString;
    public SqlConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}