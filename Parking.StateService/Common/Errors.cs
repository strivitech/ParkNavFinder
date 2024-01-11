using ErrorOr;

namespace Parking.StateService.Common;

public static class Errors
{
    public static class EventPublisher
    {
        public static Error PublishFailed(string eventName) => Error.Unexpected("EventPublisher.PublishFailed", $"Failed to publish event: {eventName}.");
    }
}