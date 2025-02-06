using Azure;
using Azure.Data.Tables;
using System;

namespace DemoApp.Models
{
    public class PostEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public int UserId { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public ETag ETag { get; set; } = default(ETag);
        public DateTimeOffset? Timestamp { get; set; } = DateTimeOffset.UtcNow;
    }
}
