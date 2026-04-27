
using TesisCRM.API.Repositories;

namespace TesisCRM.API.Services;

public class ContratoPdfService
{
    private readonly IWebHostEnvironment _env;
    private readonly ContratoRepository _repository;
    public ContratoPdfService(IWebHostEnvironment env, ContratoRepository repository) { _env = env; _repository = repository; }

    //public async Task<string> GenerarPdfAsync(int contratoId)
    //{
    //    var contrato = await _repository.GetByIdAsync(contratoId)
    //        ?? throw new InvalidOperationException("Contrato no encontrado.");

    //    var folder = Path.Combine(_env.ContentRootPath, "Storage", "ContratosPdf");
    //    Directory.CreateDirectory(folder);

    //    var path = Path.Combine(folder, $"Contrato_{contrato.Id}_{DateTime.Now:yyyyMMddHHmmss}.pdf");

    //    Document.Create(c =>
    //    {
    //        c.Page(page =>
    //        {
    //            page.Margin(30);
    //            page.Size(PageSizes.A4);
    //            page.DefaultTextStyle(x => x.FontSize(11));

    //            page.Header().AlignCenter().Text("CONTRATO DE SERVICIO").SemiBold().FontSize(18);

    //            page.Content().Column(col =>
    //            {
    //                col.Spacing(8);
    //                col.Item().Text($"Cliente: {contrato.ClienteNombre}");
    //                col.Item().Text($"Servicio: {contrato.ServicioNombre}");
    //                col.Item().Text($"Fecha contrato: {contrato.FechaContrato:dd/MM/yyyy}");
    //                col.Item().Text($"Fecha entrega: {(contrato.FechaEntrega.HasValue ? contrato.FechaEntrega.Value.ToString("dd/MM/yyyy") : "Pendiente")}");
    //                col.Item().Text($"Precio total: S/ {contrato.PrecioTotal:0.00}");
    //                col.Item().Text($"Monto pagado: S/ {contrato.MontoPagado:0.00}");
    //                col.Item().Text($"Saldo pendiente: S/ {contrato.SaldoPendiente:0.00}");
    //                col.Item().PaddingTop(10).Text("Cláusulas").Bold();
    //                col.Item().Text("1. El proveedor brindará el servicio conforme a lo pactado.");
    //                col.Item().Text("2. El cliente entregará la información requerida y cumplirá con los pagos.");
    //                col.Item().Text("3. La vigencia y entrega se rigen por las fechas registradas.");
    //                col.Item().Text("4. Cualquier cambio deberá ser coordinado entre ambas partes.");
    //            });

    //            page.Footer().AlignCenter().Text($"Generado por TesisCRM - {DateTime.Now:dd/MM/yyyy HH:mm}");
    //        });
    //    }).GeneratePdf(path);

    //    await _repository.UpdateRutaPdfAsync(contratoId, path);
    //    return path;
    //}
}
