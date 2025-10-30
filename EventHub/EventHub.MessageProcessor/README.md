# Event Hub Message Processor

This application processes events from an Azure Event Hub using the `EventProcessorClient` with blob storage checkpointing for reliable event processing.

## Features

- **Event Processing**: Processes events from all partitions of an Event Hub
- **Checkpointing**: Uses Azure Blob Storage to track processing progress
- **Fault Tolerance**: Can resume from the last checkpoint if the application restarts
- **Load Balancing**: Automatically balances partition processing across multiple instances
- **Error Handling**: Comprehensive error handling and logging

## Configuration

The application uses a two-tier configuration system:

1. **appsettings.json** - Template configuration (committed to Git)
   - Contains placeholder values
   - Shows the required configuration structure

2. **appsettings.local.json** - Local configuration (excluded from Git)
   - Contains your actual connection strings and settings
   - This file should NOT be committed to Git

### Configuration Structure

```json
{
  "EventHub": {
    "ConnectionString": "<YOUR_EVENT_HUB_CONNECTION_STRING>",
    "EventHubName": "<YOUR_EVENT_HUB_NAME>",
    "ConsumerGroup": "$Default",
    "BlobStorageConnectionString": "<YOUR_BLOB_STORAGE_CONNECTION_STRING>",
    "BlobContainerName": "eventhub-checkpoints"
  }
}
```

### Configuration Parameters

- **ConnectionString**: The connection string to your Event Hub namespace (with Listen permissions)
- **EventHubName**: The name of your Event Hub
- **ConsumerGroup**: The consumer group to use (default: `$Default`)
- **BlobStorageConnectionString**: Connection string to Azure Blob Storage for checkpointing
- **BlobContainerName**: Name of the blob container for storing checkpoints (will be created if it doesn't exist)

## How It Works

### Event Processor Client

The `EventProcessorClient` is a high-level client that:

1. **Automatically connects** to all partitions in the Event Hub
2. **Load balances** processing across multiple instances of the processor
3. **Stores checkpoints** in Azure Blob Storage to track progress
4. **Resumes processing** from the last checkpoint after a restart
5. **Handles failures** gracefully with automatic retry logic

### Checkpointing

Checkpoints are stored in Azure Blob Storage and contain:
- Partition ID
- Sequence number of the last processed event
- Offset information

After processing each event, the application updates the checkpoint. If the application restarts, it will resume from the last checkpoint rather than reprocessing all events.

## Prerequisites

1. **Azure Event Hub**: With appropriate permissions (Listen)
2. **Azure Storage Account**: For storing checkpoints
   - The blob container will be created automatically if it doesn't exist
   - The container name is configurable in the settings

## Setup

1. Create an Azure Storage Account for checkpoints (if you don't have one)
2. Get your Event Hub connection string with **Listen** permissions
3. Get your Blob Storage connection string
4. Update `appsettings.local.json` with your actual values
5. Run the application

## Running the Application

```bash
dotnet run
```

The application will:
1. Connect to the Event Hub
2. Connect to Blob Storage for checkpointing
3. Start processing events from all partitions
4. Display each event as it's received
5. Update checkpoints after processing each event
6. Continue running until you press Ctrl+C

## Event Display Format

For each event received, the processor displays:
- Timestamp
- Partition ID
- Sequence Number
- Offset
- Enqueued Time
- Event Body
- Custom Properties (if any)

## Scaling

You can run multiple instances of this processor:
- Each instance will automatically claim partitions
- Partitions are distributed across all running instances
- If an instance fails, its partitions are reassigned to other instances
- All instances share the same checkpoint storage

## Error Handling

The processor includes error handlers for:
- Event processing errors (logged but don't stop the processor)
- Partition-level errors (logged with partition details)
- General processor errors (logged with operation details)

## Security

?? **IMPORTANT**: Make sure `appsettings.local.json` is in your `.gitignore` file to prevent committing secrets to version control.

Add this line to your `.gitignore`:
```
**/appsettings.local.json
```

## Differences from Simple Consumer

| Feature | EventHubConsumerClient | EventProcessorClient |
|---------|----------------------|---------------------|
| Checkpointing | Manual | Automatic with Blob Storage |
| Load Balancing | No | Yes, across multiple instances |
| Partition Assignment | Manual | Automatic |
| Fault Tolerance | Basic | Advanced with checkpoint recovery |
| Best For | Simple scenarios, testing | Production workloads |

## Troubleshooting

### Processor not receiving events
- Check Event Hub connection string has Listen permissions
- Verify the Event Hub name is correct
- Ensure events are being sent to the hub

### Checkpoint errors
- Verify Blob Storage connection string is correct
- Ensure the storage account is accessible
- Check if the blob container can be created/accessed

### Multiple instances not load balancing
- Ensure all instances use the same consumer group
- Check that all instances use the same blob container for checkpoints
- Verify network connectivity between instances and Azure services
