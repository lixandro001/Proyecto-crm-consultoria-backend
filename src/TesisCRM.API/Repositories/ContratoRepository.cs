using Dapper;
using System.Data;
using TesisCRM.API.Data;

using TesisCRM.API.Models.Contratos;

namespace TesisCRM.API.Repositories;

public class ContratoRepository
{
    private readonly SqlConnectionFactory _factory;
    public ContratoRepository(SqlConnectionFactory factory) => _factory = factory;

    public async Task<IEnumerable<ContratoDto>> ListAsync()
    {
        using var cn = _factory.CreateConnection();
        return await cn.QueryAsync<ContratoDto>("usp_Contratos_List", commandType: CommandType.StoredProcedure);
    }

    public async Task<ContratoDto?> GetByIdAsync(int id)
    {
        using var cn = _factory.CreateConnection();
        return await cn.QueryFirstOrDefaultAsync<ContratoDto>("usp_Contratos_GetById", new { Id = id }, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> CreateAsync(ContratoCreateRequest request)
    {
        using var cn = _factory.CreateConnection();
        return await cn.ExecuteScalarAsync<int>("usp_Contratos_Insert", request, commandType: CommandType.StoredProcedure);
    }

    public async Task UpdateRutaWordAsync(int contratoId, string rutaWordGenerado)
    {
        using var cn = _factory.CreateConnection();
        await cn.ExecuteAsync("usp_Contratos_UpdateRutaWord", new { ContratoId = contratoId, RutaWordGenerado = rutaWordGenerado }, commandType: CommandType.StoredProcedure);
    }

    public async Task UpdateRutaPdfAsync(int contratoId, string rutaPdf)
    {
        using var cn = _factory.CreateConnection();
        await cn.ExecuteAsync("usp_Contratos_UpdateRutaPdf", new { ContratoId = contratoId, RutaPdf = rutaPdf }, commandType: CommandType.StoredProcedure);
    }

    public async Task<ContratoPlantillaDto?> ObtenerDataPlantillaAsync(int contratoId)
    {
        using var cn = _factory.CreateConnection();
        var data = await cn.QueryFirstOrDefaultAsync<ContratoPlantillaDto>("usp_Contrato_ObtenerDataPlantilla", new { ContratoId = contratoId }, commandType: CommandType.StoredProcedure);
        if (data is not null) data.PrecioEnLetras = $"{data.PrecioTotal:0.00} SOLES";
        return data;
    }
}
