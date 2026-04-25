using Dapper;
using System.Data;
using TesisCRM.API.Data;

using TesisCRM.API.Models.Plantillas;

namespace TesisCRM.API.Repositories;

public class PlantillaContratoRepository
{
    private readonly SqlConnectionFactory _factory;
    public PlantillaContratoRepository(SqlConnectionFactory factory) => _factory = factory;

    public async Task<IEnumerable<PlantillaContratoDto>> ListAsync()
    {
        using var cn = _factory.CreateConnection();
        return await cn.QueryAsync<PlantillaContratoDto>("usp_PlantillasContrato_List", commandType: CommandType.StoredProcedure);
    }

    //public async Task<int> CreateAsync(string nombrePlantilla, string rutaArchivoWord, string descripcion)
    //{
    //    using var cn = _factory.CreateConnection();
    //    return await cn.ExecuteScalarAsync<int>("usp_PlantillasContrato_Insert",
    //        new { NombrePlantilla = nombrePlantilla, RutaArchivoWord = rutaArchivoWord, Descripcion = descripcion },
    //        commandType: CommandType.StoredProcedure);
    //}


    public async Task<int> CreateAsync(string nombrePlantilla, string nombreArchivoWord, byte[] archivoWord, string descripcion)
    {
        using var cn = _factory.CreateConnection();

        return await cn.ExecuteScalarAsync<int>(
            "usp_PlantillasContrato_Insert",
            new
            {
                NombrePlantilla = nombrePlantilla,
                NombreArchivoWord = nombreArchivoWord,
                ArchivoWord = archivoWord,
                Descripcion = descripcion
            },
            commandType: CommandType.StoredProcedure
        );
    }


    public async Task<PlantillaContratoArchivoDto?> GetArchivoByIdAsync(int id)
    {
        using var cn = _factory.CreateConnection();

        return await cn.QueryFirstOrDefaultAsync<PlantillaContratoArchivoDto>(
            "usp_PlantillasContrato_GetArchivoById",
            new { Id = id },
            commandType: CommandType.StoredProcedure
        );
    }

   
}
