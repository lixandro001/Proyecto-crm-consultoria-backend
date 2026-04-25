namespace TesisCRM.API.Models.Contratos; public class ContratoPlantillaDto
{
    public int ContratoId { get; set; }
    public int? PlantillaContratoId { get; set; }
    public string ClienteNombreCompleto { get; set; } = "";
    public string ClienteDni { get; set; } = "";
    public string TipoServicio { get; set; } = "";
    public decimal PrecioTotal { get; set; }
    public string PrecioEnLetras { get; set; } = "";
    public decimal Pago1 { get; set; }
    public decimal Pago2 { get; set; }
    public decimal Pago3 { get; set; }
    public DateTime? FechaEntrega { get; set; }
    public DateTime FechaFirma { get; set; }
    public string FirmaRepresentante { get; set; } = "";
    public string FirmaCliente { get; set; } = "";

}


 