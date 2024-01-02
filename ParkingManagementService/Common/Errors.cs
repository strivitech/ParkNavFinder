using ErrorOr;

namespace ParkingManagementService.Common;

public static class Errors
{
    public static class Parking
    {
        public static Error NotFound(Guid id) => Error.NotFound("Parking with id {0} not found.", id.ToString());
        public static Error AddFailed(Guid id) => Error.Failure("Failed to add parking with id {0}.", id.ToString());
        
        public static Error UpdateFailed(Guid id) => Error.Failure("Failed to update parking with id {0}.", id.ToString());
        
        public static Error DeleteFailed(Guid id) => Error.Failure("Failed to delete parking with id {0}.", id.ToString());
        
        public static Error LocationCannotBeChanged() => Error.Failure("Parking location cannot be changed.");
    }
    
    public static class EventPublisher
    {
        public static Error PublishFailed(string eventName) => Error.Unexpected("Failed to publish event: {0}.", eventName);
    }
}