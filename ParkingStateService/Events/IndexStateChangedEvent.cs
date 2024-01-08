using System.Collections;
using ParkingStateService.DTOs;

namespace ParkingStateService.Events;

public record IndexStateChangedEvent(
    string EventId,     
    string Index,
    IEnumerable<ActiveParkingStateDto> ParkingState, 
    DateTime IssuedUtc
);  