using Food.Application.Abstractions;

namespace Food.Application.Tests.TestDoubles;

public sealed class FakeClock : IClock
{
    public FakeClock(DateTimeOffset utcNow) => UtcNow = utcNow;

    public DateTimeOffset UtcNow { get; }
}
