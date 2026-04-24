namespace TesisCRM.API.Models.Plantillas
{
    public class PlantillaContratoUploadRequest
    {
        public IFormFile ArchivoWord { get; set; } = default!;
        public string NombrePlantilla { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
    }
}
