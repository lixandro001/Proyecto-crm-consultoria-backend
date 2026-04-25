namespace TesisCRM.API.Models.Plantillas
{
    public class PlantillaContratoArchivoDto
    {
        public int Id { get; set; }
        public string NombrePlantilla { get; set; } = string.Empty;
        public string NombreArchivoWord { get; set; } = string.Empty;
        public byte[] ArchivoWord { get; set; } = Array.Empty<byte>();
    }
}
