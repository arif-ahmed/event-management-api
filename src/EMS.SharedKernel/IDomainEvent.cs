namespace EMS.SharedKernel;

public interface IDomainEvent
{
    Guid EventId { get; }
    DateTime OccurredAt { get; }
    Guid CorrelationId { get; }
}
