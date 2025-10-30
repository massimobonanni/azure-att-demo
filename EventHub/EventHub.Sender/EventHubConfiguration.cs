namespace EventHub.Sender;

public class EventHubConfiguration
{
    public string ConnectionString { get; set; } = string.Empty;
    public string EventHubName { get; set; } = string.Empty;
}
