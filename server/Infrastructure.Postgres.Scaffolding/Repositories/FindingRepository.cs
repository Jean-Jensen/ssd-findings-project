using Application.Interfaces;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres.Scaffolding.Repositories;

public class FindingRepository(AppDbContext context) : IFindingRepository
{
    public async Task<Finding> GetFindingWithId(Guid id)
    {
        return await context.Findings.Where(f => f.Findingid == id.ToString()).FirstOrDefaultAsync();
    }

    public async Task<bool> AddFinding(Finding finding)
    {
        await context.AddAsync(finding);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateFinding(Finding finding)
    {
        context.Update(finding);
        await context.SaveChangesAsync();
        return true;
    }
    
}