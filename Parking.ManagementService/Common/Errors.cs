using ErrorOr;

namespace Parking.ManagementService.Common;

public static class Errors
{
    public static class Parking
    {
        public static Error NotFound(Guid id) => Error.NotFound("Parking.NotFound", $"Parking with id {id.ToString()} not found.");
        public static Error AddFailed(Guid id) => Error.Failure("Parking.AddFailed", $"Failed to add parking with id {id.ToString()}.");
        
        public static Error UpdateFailed(Guid id) => Error.Failure("Parking.UpdateFailed", $"Failed to update parking with id {id.ToString()}.");
        
        public static Error DeleteFailed(Guid id) => Error.Failure("Parking.DeleteFailed", $"Failed to delete parking with id {id.ToString()}.");

        public static Error NotOwnedByCurrentUser(Guid id) => Error.Unauthorized("Parking.NotOwnedByCurrentUser", $"Parking with id {id.ToString()} is not owned by current user.");
    }
    
    public static class EventPublisher
    {
        public static Error PublishFailed(string eventName) => Error.Unexpected("EventPublisher.PublishFailed", $"Failed to publish event: {eventName}.");
    }
}