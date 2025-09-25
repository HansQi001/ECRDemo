using Amazon.DynamoDBv2.DataModel;

namespace ECRDemo.MiniAPI.Models
{
    [DynamoDBTable("products")]
    public class Product
    {
        [DynamoDBHashKey("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [DynamoDBProperty("name")]
        public string Name { get; set; } = string.Empty;
        [DynamoDBProperty("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [DynamoDBProperty("lastModifiedAt")]
        public DateTime? LastModifiedAt { get; set; }
    }
}
