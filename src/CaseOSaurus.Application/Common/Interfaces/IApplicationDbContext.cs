using CaseOSaurus.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CaseOSaurus.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<UserCase> Cases { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
