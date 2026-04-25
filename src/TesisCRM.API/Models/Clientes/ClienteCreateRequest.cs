namespace TesisCRM.API.Models.Clientes; 
public class ClienteCreateRequest 
{ 
    public string Nombres { get; set; } = "";
    public string Apellidos { get; set; } = "";
    public string DocumentoTipo { get; set; } = "DNI";
    public string DocumentoNumero { get; set; } = "";
    public string? Telefono { get; set; } 
    public string? Email { get; set; } 
    public string? Direccion { get; set; } 
    public string EstadoCliente { get; set; } = "ACTIVO";
    public string? Universidad { get; set; }
    public string? Carrera { get; set; }
    public int Ciclo { get; set; }
    // tambien poner los 3 campos nuevos 

}