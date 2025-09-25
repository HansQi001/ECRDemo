using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using ECRDemo.MiniAPI.Endpoints;
using ECRDemo.MiniAPI.Repositories;

namespace ECRDemo.MiniAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // for local dev
            if (builder.Environment.IsDevelopment())
            {
                // Load AWS options from appsettings.json or environment veriables
                builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
            }
            // Register DynamoDB client
            builder.Services.AddAWSService<IAmazonDynamoDB>();
            // Register the DynamoDB context
            builder.Services.AddSingleton<IDynamoDBContext, DynamoDBContext>();
            // Register product repository
            builder.Services.AddSingleton<IProductRepository, ProductRepository>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            app.UseSwagger();
            app.UseSwaggerUI();
            //}

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapGet("/products/{id}", ProductEndpoint.GetProductById)
            .WithName("GetProductById")
            .WithOpenApi();

            app.MapPost("/products", ProductEndpoint.CreateProduct)
            .WithName("CreateProduct")
            .WithOpenApi();

            app.MapDelete("/products/{id}", ProductEndpoint.DeleteProductById)
            .WithName("DeleteProductById")
            .WithOpenApi();

            app.MapPut("/products/{id}", ProductEndpoint.UpdateProductName)
            .WithName("UpdateProductName")
            .WithOpenApi();

            app.Run();
        }
    }
}
