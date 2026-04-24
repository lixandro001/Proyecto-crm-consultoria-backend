using Dapper;
using System.Data;
using TesisCRM.API.Data;

using TesisCRM.API.Models.Users;

namespace TesisCRM.API.Repositories;

public class UserRepository
{
    private readonly SqlConnectionFactory _factory;
    public UserRepository(SqlConnectionFactory factory) => _factory = factory;

    public async Task<IEnumerable<UserDto>> ListAsync()
    {
        using var cn = _factory.CreateConnection();
        return await cn.QueryAsync<UserDto>("usp_Users_List", commandType: CommandType.StoredProcedure);
    }

    public async Task<int> CreateAsync(string username, string fullName, string passwordHash, string roleCode)
    {
        using var cn = _factory.CreateConnection();
        return await cn.ExecuteScalarAsync<int>("usp_Users_Insert",
            new { Username = username, FullName = fullName, PasswordHash = passwordHash, RoleCode = roleCode },
            commandType: CommandType.StoredProcedure);
    }

    public async Task UpdateAsync(UserUpdateRequest request)
    {
        using var cn = _factory.CreateConnection();
        await cn.ExecuteAsync("usp_Users_Update", request, commandType: CommandType.StoredProcedure);
    }
}
