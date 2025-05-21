
using System.Text;
using UglyToad.PdfPig;
using DocumentFormat.OpenXml.Packaging;

namespace ResumeAnalyzerAPI.Utils
{
    public static class FileHelper
    {
        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

        public static async Task<string> ReadFileContentAsync(IFormFile file)
        {
            if (file.Length > MaxFileSize)
            {
                throw new ArgumentException($"File size exceeds the {MaxFileSize / (1024 * 1024)}MB limit.");
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var contentType = file.ContentType.ToLowerInvariant(); // Normalize for comparison

            bool isMimeTypeValid;
            string expectedMimeType = string.Empty;

            switch (extension)
            {
                case ".txt":
                    expectedMimeType = "text/plain";
                    isMimeTypeValid = contentType.Equals(expectedMimeType, StringComparison.OrdinalIgnoreCase);
                    break;
                case ".pdf":
                    expectedMimeType = "application/pdf";
                    isMimeTypeValid = contentType.Equals(expectedMimeType, StringComparison.OrdinalIgnoreCase);
                    break;
                case ".docx":
                    expectedMimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    isMimeTypeValid = contentType.Equals(expectedMimeType, StringComparison.OrdinalIgnoreCase);
                    break;
                default:
                    // This handles unsupported extensions directly.
                    throw new NotSupportedException($"File format with extension '{extension}' is not supported.");
            }

            if (!isMimeTypeValid)
            {
                // This block is reached if the extension was one of the supported ones, but the MIME type didn't match.
                // Consider logging this mismatch here if a logger is available/injected.
                // _logger.LogWarning("MIME type mismatch for file {FileName}. Expected {ExpectedMimeType} for extension {Extension}, but got {ContentType}.", file.FileName, expectedMimeType, extension, contentType);
                throw new NotSupportedException($"File content type '{contentType}' does not match its extension '{extension}'. Expected '{expectedMimeType}'. Please provide a valid file.");
            }

            // If both extension and MIME type are valid, proceed to parsing.
            // The outer switch already validated the extension, so the default case here is for safety or unexpected scenarios.
            switch (extension)
            {
                case ".txt":
                    using (var reader = new StreamReader(file.OpenReadStream()))
                    {
                        return await reader.ReadToEndAsync();
                    }
                // No try-catch needed for .txt as it's less prone to parsing errors compared to binary formats.
                // If specific exceptions were expected, they could be added.

                case ".pdf":
                    try
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
                    catch (Exception ex)
                    {
                        // This specific FileParsingException is caught by the controller.
                        throw new FileParsingException($"Error parsing PDF file (extension: {extension}, content-type: {contentType}). The file may be corrupted or password-protected.", ex);
                    }

                case ".docx":
                    try
                    {
                        using var memoryStream = new MemoryStream();
                        await file.CopyToAsync(memoryStream);
                        memoryStream.Position = 0;
                        using var wordDoc = WordprocessingDocument.Open(memoryStream, false);
                        var body = wordDoc.MainDocumentPart.Document.Body;
                        return body.InnerText;
                    }
                    catch (Exception ex)
                    {
                        // This specific FileParsingException is caught by the controller.
                        throw new FileParsingException($"Error parsing DOCX file (extension: {extension}, content-type: {contentType}). The file may be corrupted.", ex);
                    }

                default:
                    // This case should ideally not be reached if the first switch correctly handles all supported/unsupported extensions.
                    // It acts as a fallback.
                    throw new NotSupportedException($"File format with extension '{extension}' is not supported after MIME type check.");
            }
        }
    }
}
