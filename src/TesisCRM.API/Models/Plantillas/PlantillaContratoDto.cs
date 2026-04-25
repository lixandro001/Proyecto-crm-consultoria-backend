namespace TesisCRM.API.Models.Plantillas; public class PlantillaContratoDto
{ 
    public int Id { get; set; } 
    public string NombrePlantilla { get; set; } = "";
    public string RutaArchivoWord { get; set; } = "";
    public string? Descripcion { get; set; } 
    public bool Activa { get; set; }
    public DateTime FechaRegistro { get; set; }
    public string? NombreArchivoWord { get; set; }
}