using Application.Interfaces;
using Infrastructure.Postgres.Scaffolding;

namespace Infrastructure.Postgres.Repositories;

public class UserFindingRepository(AppDbContext context) : IUserFindingRepository
{
    /*public async Task<userfindin> GetFindingWithId(Guid id)
    {
        var finding = await context.Findings.Where(f => f.FindingId == id).FirstOrDefaultAsync();
        return finding;
    }*/
}