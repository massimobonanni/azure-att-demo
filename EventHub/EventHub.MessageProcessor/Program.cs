using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using System.Text;
using EventHub.MessageProcessor;

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

if (string.IsNullOrEmpty(eventHubConfig.BlobStorageConnectionString))
    throw new InvalidOperationException("BlobStorageConnectionString is required");

if (string.IsNullOrEmpty(eventHubConfig.BlobContainerName))
    throw new InvalidOperationException("BlobContainerName is required");

var eventHubConnectionString = eventHubConfig.ConnectionString;
var eventHubName = eventHubConfig.EventHubName;
var consumerGroup = string.IsNullOrEmpty(eventHubConfig.ConsumerGroup)
    ? EventHubConsumerClient.DefaultConsumerGroupName
    : eventHubConfig.ConsumerGroup;
var blobStorageConnectionString = eventHubConfig.BlobStorageConnectionString;
var blobContainerName = eventHubConfig.BlobContainerName;

// Create a blob container client for checkpointing
var storageClient = new BlobContainerClient(blobStorageConnectionString, blobContainerName);

// Create the event processor client
var processor = new EventProcessorClient(
    storageClient,
    consumerGroup,
    eventHubConnectionString,
    eventHubName);

Console.WriteLine($"Event Hub Message Processor for '{eventHubName}'");
Console.WriteLine($"Consumer Group: {consumerGroup}");
Console.WriteLine($"Checkpoint Storage: {blobContainerName}");
Console.WriteLine("Press Ctrl+C to stop.");
Console.WriteLine();

// Register handlers for processing events and handling errors
processor.ProcessEventAsync += ProcessEventHandler;
processor.ProcessErrorAsync += ProcessErrorHandler;

// Create a cancellation token source to handle Ctrl+C
using var cancellationSource = new CancellationTokenSource();
Console.CancelKeyPress += (sender, eventArgs) =>
{
    eventArgs.Cancel = true;
    cancellationSource.Cancel();
    Console.WriteLine("\nStopping processor...");
};

try
{
    // Start processing
    await processor.StartProcessingAsync(cancellationSource.Token);

    Console.WriteLine("Processor started. Waiting for events...");
    Console.WriteLine();

    // Wait until cancellation is requested
    await Task.Delay(Timeout.Infinite, cancellationSource.Token);
}
catch (TaskCanceledException)
{
    // Expected when cancellation is requested
}
finally
{
    try
    {
        await processor.StopProcessingAsync();
        Console.WriteLine("Processor stopped successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error stopping processor: {ex.Message}");
    }
}

// Event processing handler
async Task ProcessEventHandler(ProcessEventArgs eventArgs)
{
    try
    {
        // Only process if there's an event
        if (eventArgs.Data == null)
            return;

        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Event received from partition {eventArgs.Partition.PartitionId}:");
        Console.WriteLine($"  Sequence Number: {eventArgs.Data.SequenceNumber}");
        Console.WriteLine($"  Offset: {eventArgs.Data.OffsetString}");
        Console.WriteLine($"  Enqueued Time: {eventArgs.Data.EnqueuedTime}");

        // Convert event body to string
        string eventBody = Encoding.UTF8.GetString(eventArgs.Data.EventBody.ToArray());
        Console.WriteLine($"  Body: {eventBody}");

        // Display properties if any
        if (eventArgs.Data.Properties.Count > 0)
        {
            Console.WriteLine("  Properties:");
            foreach (var property in eventArgs.Data.Properties)
            {
                Console.WriteLine($"    {property.Key}: {property.Value}");
            }
        }

        Console.WriteLine();

        // Update checkpoint in the blob storage
        // This allows the processor to resume from this point if it restarts
        await eventArgs.UpdateCheckpointAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error processing event: {ex.Message}");
    }
}

// Error handling
Task ProcessErrorHandler(ProcessErrorEventArgs eventArgs)
{
    Console.WriteLine($"ERROR in partition {eventArgs.PartitionId ?? "N/A"}:");
    Console.WriteLine($"  Operation: {eventArgs.Operation}");
    Console.WriteLine($"  Exception: {eventArgs.Exception.Message}");
    Console.WriteLine();

    return Task.CompletedTask;
}
