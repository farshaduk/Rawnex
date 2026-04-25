namespace Rawnex.Domain.Enums;

public enum AuditAction
{
    LoginSuccess = 0,
    LoginFailed = 1,
    Logout = 2,
    TokenRefreshed = 3,
    TokenRefreshFailed = 4,
    TokenReuseDetected = 5,
    AllSessionsRevoked = 6,
    PasswordChanged = 7,
    PasswordResetRequested = 8,
    PasswordResetCompleted = 9,
    RoleAssigned = 10,
    RoleRemoved = 11,
    UserCreated = 12,
    UserLocked = 13,
    UserUnlocked = 14,
    PermissionDenied = 15,
    SensitiveDataAccessed = 16,
    PermissionGrantedToRole = 17,
    PermissionRevokedFromRole = 18,
    PermissionGrantedToUser = 19,
    PermissionRevokedFromUser = 20
}
