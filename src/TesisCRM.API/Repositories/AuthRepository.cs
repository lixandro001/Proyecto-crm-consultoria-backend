using Dapper;
using System.Data;
using TesisCRM.API.Data;

using TesisCRM.API.Models.Users;

namespace TesisCRM.API.Repositories;

public class AuthRepository
{
    private readonly SqlConnectionFactory _factory;
    public AuthRepository(SqlConnectionFactory factory) => _factory = factory;

    public async Task<UserAuthDto?> GetByUsernameAsync(string username)
    {
        using var cn = _factory.CreateConnection();
        return await cn.QueryFirstOrDefaultAsync<UserAuthDto>("usp_Auth_Login", new { Username = username }, commandType: CommandType.StoredProcedure);
    }
}
