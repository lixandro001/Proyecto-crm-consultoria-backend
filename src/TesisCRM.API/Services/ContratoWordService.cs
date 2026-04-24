using DocumentFormat.OpenXml.Packaging;
using TesisCRM.API.Models.Contratos;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
 

namespace TesisCRM.API.Services;

public class ContratoWordService
{
    public byte[] GenerarWordDesdePlantilla(string rutaPlantilla, ContratoPlantillaDto data, out string outputPath)
    {
        if (!File.Exists(rutaPlantilla))
            throw new FileNotFoundException("No se encontró la plantilla Word.", rutaPlantilla);

        outputPath = Path.Combine(Path.GetTempPath(), $"Contrato_{data.ContratoId}_{Guid.NewGuid():N}.docx");
        File.Copy(rutaPlantilla, outputPath, true);

        using (var wordDoc = WordprocessingDocument.Open(outputPath, true))
        {
            var body = wordDoc.MainDocumentPart?.Document.Body;

            if (body == null)
                throw new InvalidOperationException("El documento Word no tiene contenido válido.");

            ReplacePlaceholders(wordDoc, new Dictionary<string, string>
            {
                { "{CLIENTE_NOMBRE_COMPLETO}", data.ClienteNombreCompleto ?? "" },
                { "{CLIENTE_DNI}", data.ClienteDni ?? "" },
                { "{TIPO_SERVICIO}", data.TipoServicio ?? "" },
                { "{PRECIO_TOTAL}", data.PrecioTotal.ToString("0.00") },
                { "{PRECIO_EN_LETRAS}", data.PrecioEnLetras ?? "" },
                { "{PAGO_1}", data.Pago1.ToString("0.00") },
                { "{PAGO_2}", data.Pago2.ToString("0.00") },
                { "{PAGO_3}", data.Pago3.ToString("0.00") },
                { "{FECHA_ENTREGA}", data.FechaEntrega?.ToString("dd/MM/yyyy") ?? "" },
                { "{FECHA_FIRMA}", data.FechaFirma.ToString("dd/MM/yyyy") },
                { "{FIRMA_REPRESENTANTE}", data.FirmaRepresentante ?? "" },
                { "{FIRMA_CLIENTE}", data.FirmaCliente ?? "" }
            });

            wordDoc.MainDocumentPart!.Document.Save();
        }

        return File.ReadAllBytes(outputPath);
    }

    private void ReplacePlaceholders(WordprocessingDocument doc, Dictionary<string, string> replacements)
    {
        var mainPart = doc.MainDocumentPart;
        if (mainPart?.Document?.Body == null)
            return;

        // Reemplazo en documento principal
        ReplaceInTextElements(mainPart.Document.Body.Descendants<Text>(), replacements);

        // Reemplazo en headers
        foreach (var headerPart in mainPart.HeaderParts)
        {
            ReplaceInTextElements(headerPart.RootElement?.Descendants<Text>() ?? Enumerable.Empty<Text>(), replacements);
        }

        // Reemplazo en footers
        foreach (var footerPart in mainPart.FooterParts)
        {
            ReplaceInTextElements(footerPart.RootElement?.Descendants<Text>() ?? Enumerable.Empty<Text>(), replacements);
        }
    }

    private void ReplaceInTextElements(IEnumerable<Text> textElements, Dictionary<string, string> replacements)
    {
        foreach (var text in textElements)
        {
            if (string.IsNullOrEmpty(text.Text))
                continue;

            foreach (var replacement in replacements)
            {
                if (text.Text.Contains(replacement.Key))
                {
                    text.Text = text.Text.Replace(replacement.Key, replacement.Value);
                }
            }
        }
    }
}
