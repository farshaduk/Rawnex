using Rawnex.Domain.Enums;

namespace Rawnex.Application.Common.Interfaces;

public interface ICarrierApiService
{
    Task<CarrierTrackingResult> TrackShipmentAsync(string carrierName, string trackingNumber, CancellationToken ct = default);
    Task<IReadOnlyList<CarrierQuote>> GetFreightQuotesAsync(FreightQuoteRequest request, CancellationToken ct = default);
    Task<CarrierBookingResult> BookShipmentAsync(CarrierBookingRequest request, CancellationToken ct = default);
}

public record CarrierTrackingResult(
    string TrackingNumber,
    string Status,
    string? CurrentLocation,
    DateTime? EstimatedArrival,
    IReadOnlyList<TrackingEvent> Events);

public record TrackingEvent(
    DateTime Timestamp,
    string Location,
    string Status,
    string? Description);

public record CarrierQuote(
    string CarrierName,
    decimal Price,
    string Currency,
    int EstimatedTransitDays,
    DateTime ValidUntil);

public record FreightQuoteRequest(
    string OriginCity,
    string OriginCountry,
    string DestinationCity,
    string DestinationCountry,
    TransportMode TransportMode,
    decimal WeightKg,
    int Packages);

public record CarrierBookingRequest(
    string CarrierName,
    string OriginCity,
    string OriginCountry,
    string DestinationCity,
    string DestinationCountry,
    TransportMode TransportMode,
    decimal WeightKg,
    int Packages,
    DateTime PickupDate);

public record CarrierBookingResult(
    bool IsSuccess,
    string? BookingReference,
    string? TrackingNumber,
    string? ErrorMessage);
