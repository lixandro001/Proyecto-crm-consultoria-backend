using Dapper;
using System.Data;
using TesisCRM.API.Data;

using TesisCRM.API.Models.Clientes;

namespace TesisCRM.API.Repositories;

public class ClienteRepository
{
    private readonly SqlConnectionFactory _factory;
    public ClienteRepository(SqlConnectionFactory factory) => _factory = factory;

    public async Task<IEnumerable<ClienteDto>> ListAsync()
    {
        using var cn = _factory.CreateConnection();
        return await cn.QueryAsync<ClienteDto>("usp_Clientes_List", commandType: CommandType.StoredProcedure);
    }

    public async Task<ClienteDto?> GetByIdAsync(int id)
    {
        using var cn = _factory.CreateConnection();
        return await cn.QueryFirstOrDefaultAsync<ClienteDto>("usp_Clientes_GetById", new { Id = id }, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> CreateAsync(ClienteCreateRequest request)
    {
        using var cn = _factory.CreateConnection();
        return await cn.ExecuteScalarAsync<int>("usp_Clientes_Insert", request, commandType: CommandType.StoredProcedure);
    }

    public async Task UpdateAsync(ClienteUpdateRequest request)
    {
        using var cn = _factory.CreateConnection();
        await cn.ExecuteAsync("usp_Clientes_Update", request, commandType: CommandType.StoredProcedure);
    }
}
