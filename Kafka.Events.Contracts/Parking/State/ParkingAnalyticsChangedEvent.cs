namespace Kafka.Events.Contracts.Parking.State;

public record ParkingAnalyticsChangedEvent(List<ParkingStateInfo> ParkingStateInfos);