using Dapper;
using System.Data;
using TesisCRM.API.Data;

using TesisCRM.API.Models.Servicios;

namespace TesisCRM.API.Repositories;

public class ServicioRepository
{
    private readonly SqlConnectionFactory _factory;
    public ServicioRepository(SqlConnectionFactory factory) => _factory = factory;

    public async Task<IEnumerable<ServicioDto>> ListAsync()
    {
        using var cn = _factory.CreateConnection();
        return await cn.QueryAsync<ServicioDto>("usp_Servicios_List", commandType: CommandType.StoredProcedure);
    }

    public async Task<int> CreateAsync(ServicioCreateRequest request)
    {
        using var cn = _factory.CreateConnection();
        return await cn.ExecuteScalarAsync<int>("usp_Servicios_Insert", request, commandType: CommandType.StoredProcedure);
    }
}
