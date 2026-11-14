using Application.Interfaces;
using Application.Models.Constants;
using Core.Domain.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TempAuditTest;

[ApiController]
[Route("api/temp-audit-test")]
public class TempAuditTestController(IAuditLogService audit, AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Run()
    {
        var finding = new Finding
        {
            FindingId = Guid.NewGuid(),
            Title = "Audit test",
            Details = "secret details",
            AffectedAsset = "test",
            DiscoveryDate = DateOnly.FromDateTime(DateTime.UtcNow),
            ReporterEmail = "test@example.com",
            Status = "New"
        };
        db.Findings.Add(finding);

        var actorId = Guid.NewGuid();   // fake user, just for the test
        var email = "tester@example.com";

        audit.Log(finding.FindingId, actorId, email, AuditActionConstants.Created);
        audit.Log(finding.FindingId, actorId, email, AuditActionConstants.Updated, "status", "New", "Triaged");
        audit.Log(finding.FindingId, actorId, email, AuditActionConstants.Updated, "details", "old secret", "new secret");
        audit.Log(finding.FindingId, actorId, email, AuditActionConstants.Assigned, null, null, Guid.NewGuid().ToString());

        await db.SaveChangesAsync();   // saves the finding and the 4 log lines together

        var rows = await db.AuditLogs
            .Where(a => a.FindingId == finding.FindingId)
            .OrderBy(a => a.AuditId)
            .Select(a => new { a.AuditId, a.OccurredAt, a.Action, a.FieldName, a.OldValue, a.NewValue, a.ActorEmail })
            .ToListAsync();

        return Ok(rows);
    }
}