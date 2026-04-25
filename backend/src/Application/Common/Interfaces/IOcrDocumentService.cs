namespace Rawnex.Application.Common.Interfaces;

public interface IOcrDocumentService
{
    Task<OcrResult> ExtractTextAsync(Stream documentStream, string fileName, CancellationToken ct = default);
    Task<DocumentVerificationResult> VerifyDocumentAsync(Stream documentStream, string fileName, string expectedDocumentType, CancellationToken ct = default);
}

public record OcrResult(
    bool IsSuccess,
    string? ExtractedText,
    Dictionary<string, string> ExtractedFields,
    decimal ConfidenceScore);

public record DocumentVerificationResult(
    bool IsSuccess,
    bool IsAuthentic,
    string DocumentType,
    decimal ConfidenceScore,
    Dictionary<string, string> ExtractedData,
    IReadOnlyList<string> Warnings);
