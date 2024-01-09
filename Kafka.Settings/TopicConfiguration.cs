namespace Kafka.Settings;

public static class TopicConfig
{
    public static class UserLocations
    {   
        public const string TopicName = "user-locations";      
        public const int NumberOfPartitions = 1;
        public const short ReplicationFactor = 1;
    }
    
    public static class ParkingManagementEvents
    {       
        public const string TopicName = "parking-management-events";
        public const int NumberOfPartitions = 1;
        public const short ReplicationFactor = 1;
    }
    
    public static class ParkingStateEvents
    {       
        public const string TopicName = "parking-state-events"; 
        public const int NumberOfPartitions = 1;
        public const short ReplicationFactor = 1;
    }
}