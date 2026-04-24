using Dapper;
using System.Data;
using TesisCRM.API.Data;

using TesisCRM.API.Models.Agenda;

namespace TesisCRM.API.Repositories;

public class AgendaRepository
{
    private readonly SqlConnectionFactory _factory;
    public AgendaRepository(SqlConnectionFactory factory) => _factory = factory;

    public async Task<IEnumerable<AgendaEventoDto>> ListAsync()
    {
        using var cn = _factory.CreateConnection();
        return await cn.QueryAsync<AgendaEventoDto>("usp_Agenda_List", commandType: CommandType.StoredProcedure);
    }

    public async Task<int> CreateAsync(AgendaCreateRequest request)
    {
        using var cn = _factory.CreateConnection();
        return await cn.ExecuteScalarAsync<int>("usp_Agenda_Insert", request, commandType: CommandType.StoredProcedure);
    }
}
