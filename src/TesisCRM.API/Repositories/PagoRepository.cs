using Dapper;
using System.Data;
using TesisCRM.API.Data;

using TesisCRM.API.Models.Pagos;

namespace TesisCRM.API.Repositories;

public class PagoRepository
{
    private readonly SqlConnectionFactory _factory;
    public PagoRepository(SqlConnectionFactory factory) => _factory = factory;

    public async Task<IEnumerable<PagoDto>> ListAsync()
    {
        using var cn = _factory.CreateConnection();
        return await cn.QueryAsync<PagoDto>("usp_Pagos_List", commandType: CommandType.StoredProcedure);
    }

    public async Task<int> CreateAsync(PagoCreateRequest request)
    {
        using var cn = _factory.CreateConnection();
        return await cn.ExecuteScalarAsync<int>("usp_Pagos_Insert", request, commandType: CommandType.StoredProcedure);
    }
}
