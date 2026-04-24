using Dapper;
using System.Data;
using TesisCRM.API.Data;

using TesisCRM.API.Models.Dashboard;

namespace TesisCRM.API.Repositories;

public class DashboardRepository
{
    private readonly SqlConnectionFactory _factory;
    public DashboardRepository(SqlConnectionFactory factory) => _factory = factory;

    public async Task<DashboardResumenDto> GetResumenAsync()
    {
        using var cn = _factory.CreateConnection();
        return await cn.QueryFirstAsync<DashboardResumenDto>("usp_Dashboard_Resumen", commandType: CommandType.StoredProcedure);
    }
}
