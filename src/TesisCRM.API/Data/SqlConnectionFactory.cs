using System.Data;
using System.Data.SqlClient;

namespace TesisCRM.API.Data;

public class SqlConnectionFactory
{
    private readonly IConfiguration _configuration;

    public SqlConnectionFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public SqlConnection CreateConnection()
    {
        var connectionString = _configuration.GetConnectionString("Default");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new Exception("No se encontró la cadena de conexión 'Default'.");
        }

        return new SqlConnection(connectionString);
    }
}
