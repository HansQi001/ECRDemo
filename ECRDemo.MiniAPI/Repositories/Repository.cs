using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using ECRDemo.MiniAPI.Models;
using System.Xml.Linq;

namespace ECRDemo.MiniAPI.Repositories
{
    public interface IRepository<T>
    {
        Task SaveAsync(T model, CancellationToken cancellationToken);

        Task<T?> GetAsync(string id, CancellationToken cancellationToken);

        Task DeleteAsync(string id, CancellationToken cancellationToken);

        Task UpdateAsync(T model, CancellationToken cancellationToken);

        Task PartialUpdateAsync(T model, CancellationToken cancellationToken);
    }

    public abstract class Repository<T> : IRepository<T>
        where T : class
    {
        protected readonly IDynamoDBContext _context;
        protected readonly IAmazonDynamoDB _client;

        public Repository(IDynamoDBContext context
            , IAmazonDynamoDB client)
        {
            _context = context;
            _client = client;
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken) => await _context.DeleteAsync<T>(id, cancellationToken);

        public async Task<T?> GetAsync(string id, CancellationToken cancellationToken) => await _context.LoadAsync<T>(id, cancellationToken);

        public async Task SaveAsync(T model, CancellationToken cancellationToken) => await _context.SaveAsync(model, cancellationToken);

        public virtual async Task UpdateAsync(T model, CancellationToken cancellationToken) => await SaveAsync(model, cancellationToken);

        public async Task PartialUpdateAsync(T model, CancellationToken cancellationToken)
        {
            var updateRequest = CreateUpdateRequest(model);

            await _client.UpdateItemAsync(updateRequest, cancellationToken);
        }

        protected abstract UpdateItemRequest CreateUpdateRequest(T model);
    }
}
