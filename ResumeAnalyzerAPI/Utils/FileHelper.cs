
using System.Text;
using UglyToad.PdfPig;
using DocumentFormat.OpenXml.Packaging;

namespace ResumeAnalyzerAPI.Utils
{
    public static class FileHelper
    {
        public static async Task<string> ReadFileContentAsync(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            switch (extension)
            {
                case ".txt":
                    using (var reader = new StreamReader(file.OpenReadStream()))
                    {
                        return await reader.ReadToEndAsync();
                    }

                case ".pdf":
                    {
                        using var stream = file.OpenReadStream();
                        var sb = new StringBuilder();
                        using var pdf = PdfDocument.Open(stream);
                        foreach (var page in pdf.GetPages())
                        {
                            sb.Append(page.Text);
                        }
                        return sb.ToString();
                    }

                case ".docx":
                    {
                        using var memoryStream = new MemoryStream();
                        await file.CopyToAsync(memoryStream);
                        memoryStream.Position = 0;
                        using var wordDoc = WordprocessingDocument.Open(memoryStream, false);
                        var body = wordDoc.MainDocumentPart.Document.Body;
                        return body.InnerText;
                    }

                default:
                    throw new NotSupportedException("File format is not supported.");
            }
        }
    }
}
