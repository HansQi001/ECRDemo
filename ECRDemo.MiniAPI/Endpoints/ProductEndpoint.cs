using Amazon.DynamoDBv2.Model;
using ECRDemo.MiniAPI.Models;
using ECRDemo.MiniAPI.Repositories;

namespace ECRDemo.MiniAPI.Endpoints
{
    public static class ProductEndpoint
    {
        public static async Task<IResult> GetProductById(string id, IProductRepository repo, CancellationToken cancellationToken)
        {
            id = id.Trim();

            var product = await repo.GetAsync(id, cancellationToken);

            if (product == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(product);
        }

        public static async Task<IResult> CreateProduct(ProductDTO dto, IProductRepository repo, CancellationToken cancellationToken)
        {
            var product = new Product
            {
                Name = dto.Name
            };

            await repo.SaveAsync(product, cancellationToken);

            return Results.Created($"/products/{product.Id}", product);
        }

        public static async Task<IResult> DeleteProductById(string id, IProductRepository repo, CancellationToken cancellationToken)
        {
            id = id.Trim();

            await repo.DeleteAsync(id, cancellationToken);

            return Results.NoContent();
        }

        public static async Task<IResult> UpdateProductName(string id, ProductDTO dto, IProductRepository repo, CancellationToken cancellationToken)
        {
            id = id.Trim();

            var product = new Product
            {
                Id = id,
                Name = dto.Name
            };

            try
            {
                await repo.PartialUpdateAsync(product, cancellationToken);
            }
            catch (ConditionalCheckFailedException)
            {
                return Results.BadRequest();
            }

            return Results.Ok();
        }
    }
}
