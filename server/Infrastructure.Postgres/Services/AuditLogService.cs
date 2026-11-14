using Application.Interfaces;
using Core.Domain.Entities;
using Infrastructure.Postgres.Scaffolding;

namespace Infrastructure.PostGres.Services;

public class AuditLogService(AppDbContext db) : IAuditLogService
{
    private const string DetailsField = "details";

    public void Log(Guid findingId, Guid? actorId, string actorEmail, string action,
        string? fieldName = null, string? oldValue = null, string? newValue = null)
    {
        // details may be encrypted (FIND-11), so never store its old/new text in the log
        if (fieldName == DetailsField)
        {
            oldValue = null;
            newValue = null;
        }

        db.AuditLogs.Add(new AuditLog
        {
            FindingId = findingId,
            ActorId = actorId,
            ActorEmail = actorEmail,
            Action = action,
            FieldName = fieldName,
            OldValue = oldValue,
            NewValue = newValue
        });
    }
}