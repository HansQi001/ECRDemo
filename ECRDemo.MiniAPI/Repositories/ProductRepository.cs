using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using ECRDemo.MiniAPI.Models;

namespace ECRDemo.MiniAPI.Repositories
{
    public interface IProductRepository : IRepository<Product> { }

    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(IDynamoDBContext context
            , IAmazonDynamoDB client) : base(context, client) { }

        public override Task UpdateAsync(Product model, CancellationToken cancellationToken)
        {
            model.LastModifiedAt = DateTime.UtcNow;
            return base.UpdateAsync(model, cancellationToken);
        }

        protected override UpdateItemRequest CreateUpdateRequest(Product product)
            => new UpdateItemRequest
            {
                TableName = "products",
                Key = new Dictionary<string, AttributeValue>
                {
                    { "id", new AttributeValue { S = product.Id } }
                },
                UpdateExpression = "SET #n = :name, lastModifiedAt = :modified",
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    { "#n", "name" }
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":name", new AttributeValue { S = product.Name } },
                    { ":modified", new AttributeValue { S = DateTime.UtcNow.ToString("o") } }
                },
                ConditionExpression = "attribute_exists(id)" // Ensures item must already exist
            };

    }
}
