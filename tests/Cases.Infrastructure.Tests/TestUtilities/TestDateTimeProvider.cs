using System;
using Cases.Domain.Exceptions;
using Cases.Application.Common.Interfaces;

namespace Cases.Infrastructure.Tests.TestUtilities;

public sealed class TestDateTimeProvider : IDateTimeProvider
{
    private DateTimeOffset _utcNow;

    public TestDateTimeProvider(DateTimeOffset utcNow)
    {
        _utcNow = utcNow;
    }

    public DateTimeOffset UtcNow => _utcNow;

    public void AdvanceTo(DateTimeOffset utcNow)
    {
        _utcNow = utcNow;
    }
}
