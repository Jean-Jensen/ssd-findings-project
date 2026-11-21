using Application.Models.Dtos;

namespace Application.Interfaces;

public interface IAuditLogReader
{
    Task<AuditLogPageDto> GetPageAsync(Guid? findingId, int offset, int limit);

    Task<bool> FindingExistsAsync(Guid findingId);
}