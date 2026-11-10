using Microsoft.Data.SqlClient;

namespace EWallet.Core.Interfaces.Data;

public interface IDbConnectionFactory
{
    SqlConnection CreateConnection();
}