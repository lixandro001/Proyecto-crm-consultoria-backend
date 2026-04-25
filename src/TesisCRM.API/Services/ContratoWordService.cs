using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using TesisCRM.API.Models.Contratos;

namespace TesisCRM.API.Services;

public class ContratoWordService
{
    public byte[] GenerarWordDesdePlantillaBytes(byte[] plantillaBytes, ContratoPlantillaDto data)
    {
        using var inputStream = new MemoryStream();
        inputStream.Write(plantillaBytes, 0, plantillaBytes.Length);

        using var workingStream = new MemoryStream(inputStream.ToArray());

        using (var wordDoc = WordprocessingDocument.Open(workingStream, true))
        {
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

            wordDoc.MainDocumentPart?.Document.Save();
        }

        return workingStream.ToArray();
    }

    private void ReplacePlaceholders(WordprocessingDocument doc, Dictionary<string, string> replacements)
    {
        var mainPart = doc.MainDocumentPart;
        if (mainPart?.Document?.Body == null)
            return;

        ReplaceInTextElements(mainPart.Document.Body.Descendants<Text>(), replacements);

        foreach (var headerPart in mainPart.HeaderParts)
        {
            ReplaceInTextElements(headerPart.RootElement?.Descendants<Text>() ?? Enumerable.Empty<Text>(), replacements);
        }

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