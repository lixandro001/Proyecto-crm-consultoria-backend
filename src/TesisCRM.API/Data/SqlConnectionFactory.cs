using System.Data;
using Microsoft.Data.SqlClient;

namespace TesisCRM.API.Data;

public class SqlConnectionFactory
{
    private readonly IConfiguration _configuration;
    public SqlConnectionFactory(IConfiguration configuration) => _configuration = configuration;

    public IDbConnection CreateConnection()
    {
        var cs = _configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("No existe ConnectionStrings:Default.");
        return new SqlConnection(cs);
    }
}
