using Dapper;
using System.Data;
using TesisCRM.API.Data;

using TesisCRM.API.Models.ClienteServicios;

namespace TesisCRM.API.Repositories;

public class ClienteServicioRepository
{
    private readonly SqlConnectionFactory _factory;
    public ClienteServicioRepository(SqlConnectionFactory factory) => _factory = factory;

    public async Task<IEnumerable<ClienteServicioDto>> ListAsync()
    {
        using var cn = _factory.CreateConnection();
        return await cn.QueryAsync<ClienteServicioDto>("usp_ClienteServicios_List", commandType: CommandType.StoredProcedure);
    }

    public async Task<int> CreateAsync(ClienteServicioCreateRequest request)
    {
        using var cn = _factory.CreateConnection();
        return await cn.ExecuteScalarAsync<int>("usp_ClienteServicios_Insert", request, commandType: CommandType.StoredProcedure);
    }
}
