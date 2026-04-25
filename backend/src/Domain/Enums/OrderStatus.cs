namespace Rawnex.Domain.Enums;

public enum OrderStatus
{
    Draft = 0,
    PendingApproval = 1,
    Approved = 2,
    Confirmed = 3,
    InProduction = 4,
    ReadyToShip = 5,
    Shipped = 6,
    Delivered = 7,
    Completed = 8,
    Cancelled = 9,
    Disputed = 10
}
