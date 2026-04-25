using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Rawnex.Application.Common.Interfaces;

namespace Rawnex.Infrastructure.Services;

public class BillOfLadingService : IBillOfLadingService
{
    private readonly IApplicationDbContext _db;
    private readonly IFileStorageService _fileStorage;
    private readonly ILogger<BillOfLadingService> _logger;

    public BillOfLadingService(
        IApplicationDbContext db,
        IFileStorageService fileStorage,
        ILogger<BillOfLadingService> logger)
    {
        _db = db;
        _fileStorage = fileStorage;
        _logger = logger;
    }

    public async Task<BillOfLadingResult> GenerateAsync(Guid shipmentId, CancellationToken ct = default)
    {
        var shipment = await _db.Shipments
            .Include(s => s.SellerCompany)
            .Include(s => s.BuyerCompany)
            .Include(s => s.Batches)
            .FirstOrDefaultAsync(s => s.Id == shipmentId, ct);

        if (shipment is null)
            return new BillOfLadingResult(false, null, null, "Shipment not found");

        try
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var bolNumber = shipment.BillOfLadingNumber
                ?? $"BOL-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpperInvariant()}";

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Column(col =>
                    {
                        col.Item().Text("BILL OF LADING").Bold().FontSize(20).AlignCenter();
                        col.Item().PaddingTop(5).Text($"B/L No: {bolNumber}").FontSize(12).AlignCenter();
                        col.Item().PaddingTop(2).Text($"Date: {DateTime.UtcNow:yyyy-MM-dd}").AlignCenter();
                        col.Item().PaddingBottom(10).LineHorizontal(1);
                    });

                    page.Content().Column(col =>
                    {
                        // Shipper & Consignee
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Border(1).Padding(8).Column(c =>
                            {
                                c.Item().Text("SHIPPER / EXPORTER").Bold();
                                c.Item().Text(shipment.SellerCompany?.LegalName ?? "N/A");
                                c.Item().Text($"{shipment.OriginCity}, {shipment.OriginCountry}");
                            });

                            row.RelativeItem().Border(1).Padding(8).Column(c =>
                            {
                                c.Item().Text("CONSIGNEE").Bold();
                                c.Item().Text(shipment.BuyerCompany?.LegalName ?? "N/A");
                                c.Item().Text($"{shipment.DestinationCity}, {shipment.DestinationCountry}");
                            });
                        });

                        col.Item().PaddingTop(10);

                        // Shipping details
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Border(1).Padding(8).Column(c =>
                            {
                                c.Item().Text("VESSEL / TRANSPORT").Bold();
                                c.Item().Text($"Carrier: {shipment.CarrierName ?? "N/A"}");
                                c.Item().Text($"Mode: {shipment.TransportMode}");
                                c.Item().Text($"Container: {shipment.ContainerNumber ?? "N/A"}");
                            });

                            row.RelativeItem().Border(1).Padding(8).Column(c =>
                            {
                                c.Item().Text("PORT OF LOADING / DISCHARGE").Bold();
                                c.Item().Text($"From: {shipment.OriginCity ?? "N/A"}, {shipment.OriginCountry ?? "N/A"}");
                                c.Item().Text($"To: {shipment.DestinationCity ?? "N/A"}, {shipment.DestinationCountry ?? "N/A"}");
                                c.Item().Text($"Incoterm: {shipment.Incoterm}");
                            });
                        });

                        col.Item().PaddingTop(10);

                        // Cargo details
                        col.Item().Text("PARTICULARS OF GOODS").Bold().FontSize(11);
                        col.Item().PaddingTop(5).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Border(1).Padding(4).Text("Description").Bold();
                                header.Cell().Border(1).Padding(4).Text("Batch No").Bold();
                                header.Cell().Border(1).Padding(4).Text("Quantity").Bold();
                                header.Cell().Border(1).Padding(4).Text("Origin").Bold();
                            });

                            if (shipment.Batches.Count > 0)
                            {
                                foreach (var batch in shipment.Batches)
                                {
                                    table.Cell().Border(1).Padding(4).Text(batch.QualityGrade ?? "Goods");
                                    table.Cell().Border(1).Padding(4).Text(batch.BatchNumber);
                                    table.Cell().Border(1).Padding(4).Text($"{batch.Quantity} {batch.UnitOfMeasure}");
                                    table.Cell().Border(1).Padding(4).Text(batch.Origin ?? "N/A");
                                }
                            }
                            else
                            {
                                table.Cell().Border(1).Padding(4).Text("General Cargo");
                                table.Cell().Border(1).Padding(4).Text("-");
                                table.Cell().Border(1).Padding(4).Text($"{shipment.GrossWeightKg ?? 0} kg");
                                table.Cell().Border(1).Padding(4).Text(shipment.OriginCountry ?? "N/A");
                            }
                        });

                        col.Item().PaddingTop(10);

                        // Weight & packages
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Border(1).Padding(8).Column(c =>
                            {
                                c.Item().Text("GROSS WEIGHT").Bold();
                                c.Item().Text($"{shipment.GrossWeightKg ?? 0} kg");
                            });

                            row.RelativeItem().Border(1).Padding(8).Column(c =>
                            {
                                c.Item().Text("NUMBER OF PACKAGES").Bold();
                                c.Item().Text($"{shipment.NumberOfPackages ?? 0}");
                            });
                        });
                    });

                    page.Footer().Column(col =>
                    {
                        col.Item().PaddingTop(20).LineHorizontal(1);
                        col.Item().PaddingTop(5).Text($"Shipment No: {shipment.ShipmentNumber}").FontSize(8);
                        col.Item().Text($"Generated by Rawnex Platform on {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC").FontSize(8);
                    });
                });
            });

            using var stream = new MemoryStream();
            document.GeneratePdf(stream);
            stream.Position = 0;

            var fileUrl = await _fileStorage.UploadAsync(stream, $"{bolNumber}.pdf", "bills-of-lading", ct);

            // Update shipment with B/L info
            shipment.BillOfLadingNumber = bolNumber;
            shipment.BillOfLadingUrl = fileUrl;
            await _db.SaveChangesAsync(ct);

            _logger.LogInformation("Bill of Lading {BolNumber} generated for shipment {ShipmentId}", bolNumber, shipmentId);
            return new BillOfLadingResult(true, bolNumber, fileUrl, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate Bill of Lading for shipment {ShipmentId}", shipmentId);
            return new BillOfLadingResult(false, null, null, ex.Message);
        }
    }
}
