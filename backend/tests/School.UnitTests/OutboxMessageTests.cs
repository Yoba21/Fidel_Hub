using FluentAssertions;
using School.Shared.Outbox;
using School.Shared.Tenancy;
using Xunit;

namespace School.UnitTests;

public class OutboxMessageTests
{
    [Fact]
    public void OutboxMessage_Implements_ITenantScoped()
    {
        var message = new OutboxMessage
        {
            TenantId = Guid.NewGuid(),
            EventType = "StudentEnrolledEvent",
            Payload = "{\"studentId\":\"123\"}"
        };

        message.Should().BeAssignableTo<ITenantScoped>();
        message.TenantId.Should().NotBeEmpty();
        message.OccurredAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        message.ProcessedAt.Should().BeNull();
        message.RetryCount.Should().Be(0);
        message.Error.Should().BeNull();
    }
}
