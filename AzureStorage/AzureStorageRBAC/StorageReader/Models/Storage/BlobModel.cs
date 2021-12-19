using System;

namespace StorageReader.Models.Storage
{
    public class BlobModel
    {
        public string Feature { get; set; }
        public string ContainerName { get; set; }
        public string BlobPath { get; set; }
        public string Content { get; set; }

        public Exception Exception { get; set; }

        public bool HasException { get => Exception != null; }
    }
}
