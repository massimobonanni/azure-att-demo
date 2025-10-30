namespace EventHub.Receiver;

public class EventHubConfiguration
{
    public string ConnectionString { get; set; } = string.Empty;
    public string EventHubName { get; set; } = string.Empty;
    public string ConsumerGroup { get; set; } = string.Empty;
}
