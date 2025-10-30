using Azure.Messaging.EventHubs.Consumer;
using System.Text;
using Microsoft.Extensions.Configuration;
using EventHub.Receiver;

// Build configuration
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
    .Build();

// Read Event Hub configuration
var eventHubConfig = configuration.GetSection("EventHub").Get<EventHubConfiguration>()
    ?? throw new InvalidOperationException("EventHub configuration section is missing");

if (string.IsNullOrEmpty(eventHubConfig.ConnectionString))
    throw new InvalidOperationException("EventHub ConnectionString is required");

if (string.IsNullOrEmpty(eventHubConfig.EventHubName))
 throw new InvalidOperationException("EventHub EventHubName is required");

var connectionString = eventHubConfig.ConnectionString;
var eventHubName = eventHubConfig.EventHubName;
var consumerGroup = string.IsNullOrEmpty(eventHubConfig.ConsumerGroup) 
    ? EventHubConsumerClient.DefaultConsumerGroupName 
    : eventHubConfig.ConsumerGroup;

// Create a consumer client
await using var consumer = new EventHubConsumerClient(consumerGroup, connectionString, eventHubName);

Console.WriteLine($"Starting Event Hub listener for '{eventHubName}'...");
Console.WriteLine("Press Ctrl+C to stop.");
Console.WriteLine();

try
{
    // Create a cancellation token source to handle Ctrl+C
    using var cancellationSource = new CancellationTokenSource();
    Console.CancelKeyPress += (sender, eventArgs) =>
    {
        eventArgs.Cancel = true;
      cancellationSource.Cancel();
    };

    // Read events from all partitions
    await foreach (PartitionEvent partitionEvent in consumer.ReadEventsAsync(cancellationSource.Token))
    {
        Console.WriteLine($"Event received from partition {partitionEvent.Partition.PartitionId}:");
        Console.WriteLine($"  Sequence Number: {partitionEvent.Data.SequenceNumber}");
        Console.WriteLine($"  Offset: {partitionEvent.Data.Offset}");
        Console.WriteLine($"  Enqueued Time: {partitionEvent.Data.EnqueuedTime}");

        // Convert event body to string
        string eventBody = Encoding.UTF8.GetString(partitionEvent.Data.EventBody.ToArray());
 Console.WriteLine($"  Body: {eventBody}");

        // Display properties if any
     if (partitionEvent.Data.Properties.Count > 0)
        {
   Console.WriteLine("  Properties:");
   foreach (var property in partitionEvent.Data.Properties)
          {
 Console.WriteLine($"    {property.Key}: {property.Value}");
         }
   }

        Console.WriteLine();
    }
}
catch (TaskCanceledException)
{
    Console.WriteLine("\nListener stopped.");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}