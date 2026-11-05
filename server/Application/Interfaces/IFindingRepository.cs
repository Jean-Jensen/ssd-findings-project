using Core.Domain.Entities;

namespace Application.Interfaces;

public interface IFindingRepository
{
    public Task<Finding?> GetFindingWithId(Guid id);

    public Task<bool> AddFinding(Finding finding);

    public Task<bool> UpdateFinding(Finding finding);
}