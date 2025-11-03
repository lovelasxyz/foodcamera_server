using System;
using Cases.Domain.Exceptions;
using Cases.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cases.Infrastructure.Tests.TestUtilities;

public static class DbContextFactory
{
    public static CasesDbContext CreateInMemory()
    {
        var options = new DbContextOptionsBuilder<CasesDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new CasesDbContext(options);
    }
}
