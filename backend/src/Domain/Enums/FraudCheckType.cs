namespace Rawnex.Domain.Enums;

public enum FraudCheckType
{
    Registration = 0,
    Transaction = 1,
    BehavioralPattern = 2,
    DocumentVerification = 3,
    SanctionScreening = 4,
    DeviceFingerprint = 5
}
