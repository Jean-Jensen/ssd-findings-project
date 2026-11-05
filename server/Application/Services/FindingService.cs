using Application.Interfaces;
using Core.Domain.Entities;

namespace Application.Services;

public class FindingService(IFindingRepository findRepo)
{
    public async Task<Finding> GetFindingWithId(Guid id)
    {
        return await findRepo.GetFindingWithId(id);
    }

    public async Task<bool> AddFinding(Finding finding)
    {
        return await findRepo.AddFinding(finding);
    }

    public async Task<bool> UpdateFinding(Finding finding)
    {
        return await findRepo.UpdateFinding(finding);
    }
}