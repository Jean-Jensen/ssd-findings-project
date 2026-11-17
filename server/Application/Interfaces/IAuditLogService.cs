namespace Application.Interfaces;

public interface IAuditLogService
{
    /// Adds an audit row to the current DB session. It is saved when the caller
    /// calls SaveChangesAsync, in the same transaction as the finding change.
    void Log(Guid findingId, Guid? actorId, string actorEmail, string action,
        string? fieldName = null, string? oldValue = null, string? newValue = null);
}