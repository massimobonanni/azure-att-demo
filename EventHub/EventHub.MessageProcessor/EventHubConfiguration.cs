namespace EventHub.MessageProcessor;

public class EventHubConfiguration
{
    public string ConnectionString { get; set; } = string.Empty;
    public string EventHubName { get; set; } = string.Empty;
    public string ConsumerGroup { get; set; } = string.Empty;
    public string BlobStorageConnectionString { get; set; } = string.Empty;
    public string BlobContainerName { get; set; } = string.Empty;
}
