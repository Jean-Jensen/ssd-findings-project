using Application.Interfaces;
using Application.Models.Dtos;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.PostGres.Services;

public class AuditLogReader(AppDbContext db) : IAuditLogReader
{
    public async Task<bool> FindingExistsAsync(Guid findingId)
    {
        return await db.Findings
            .AnyAsync(f => f.FindingId == findingId);
    }

    public async Task<AuditLogPageDto> GetPageAsync(
        Guid? findingId,
        int offset,
        int limit)
    {
        var query = db.AuditLogs.AsNoTracking();

        if (findingId.HasValue)
        {
            query = query.Where(a => a.FindingId == findingId.Value);
        }

        var total = await query.CountAsync();

        var ordered = findingId.HasValue
            ? query.OrderBy(a => a.OccurredAt).ThenBy(a => a.AuditId)
            : query.OrderByDescending(a => a.OccurredAt)
                .ThenByDescending(a => a.AuditId);

        var items = await ordered
            .Skip(offset)
            .Take(limit)
            .Select(a => new AuditLogEntryDto
            {
                AuditId = a.AuditId,
                OccurredAt = a.OccurredAt,
                FindingId = a.FindingId,
                ActorId = a.ActorId,
                ActorEmail = a.ActorEmail,
                Action = a.Action,
                FieldName = a.FieldName,
                OldValue = a.OldValue,
                NewValue = a.NewValue
            })
            .ToListAsync();

        return new AuditLogPageDto
        {
            Items = items,
            Total = total
        };
    }
}