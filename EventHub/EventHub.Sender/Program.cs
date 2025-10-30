using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.Configuration;
using System.Text;
using EventHub.Sender;

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

// Ask user for sender configuration
Console.WriteLine($"Event Hub Sender for '{eventHubName}'");
Console.WriteLine();

int numberOfMessages;
while (true)
{
    Console.Write("Enter the number of messages to send: ");
    if (int.TryParse(Console.ReadLine(), out numberOfMessages) && numberOfMessages > 0)
        break;
    Console.WriteLine("Please enter a valid positive number.");
}

int delayBetweenMessages;
while (true)
{
    Console.Write("Enter the delay between messages (in milliseconds): ");
    if (int.TryParse(Console.ReadLine(), out delayBetweenMessages) && delayBetweenMessages >= 0)
        break;
    Console.WriteLine("Please enter a valid non-negative number.");
}

Console.WriteLine();

// Create a producer client
await using var producer = new EventHubProducerClient(connectionString, eventHubName);

Console.WriteLine($"Sending {numberOfMessages} messages with {delayBetweenMessages}ms delay between them...");
Console.WriteLine();

try
{
    for (int i = 1; i <= numberOfMessages; i++)
    {
        // Create event data
        var messageBody = new
        {
            MessageId = i,
            Timestamp = DateTime.UtcNow,
            Message = $"Test message #{i}",
            Sender = "EventHub.Sender"
        };

        var messageJson = System.Text.Json.JsonSerializer.Serialize(messageBody);
        var eventData = new EventData(Encoding.UTF8.GetBytes(messageJson));

        // Add custom properties
        eventData.Properties.Add("CustomProperty01", new Guid());
        eventData.Properties.Add("CustomProperty02", Random.Shared.Next());

        // Send the event to the event hub
        await producer.SendAsync(new[] { eventData });

        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Message {i}/{numberOfMessages} sent successfully");
        Console.WriteLine($"  Content: {messageJson}");

        // Wait before sending the next message (except for the last one)
        if (i < numberOfMessages)
        {
            await Task.Delay(delayBetweenMessages);
        }
    }

    Console.WriteLine();
    Console.WriteLine($"All {numberOfMessages} messages sent successfully!");
}
catch (Exception ex)
{
    Console.WriteLine($"Error sending messages: {ex.Message}");
}
