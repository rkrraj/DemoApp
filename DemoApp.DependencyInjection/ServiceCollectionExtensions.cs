using Azure.Data.Tables;
using DemoApp.Service;
using DemoApp.Service.Facade;
using DemoApp.Service.Proxy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DemoApp.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddTransient<IApiClient, ApiClient>();
            services.AddTransient<IArticleFacade, ArticleFacade>();
            services.AddTransient<IArticleService, ArticleService>(sp =>
            {
                var articleFacade = sp.GetRequiredService<IArticleFacade>();
                var tableServiceClient = sp.GetRequiredService<TableServiceClient>();
                var logger = sp.GetRequiredService<ILogger<ArticleService>>();
                var tableClient = tableServiceClient.GetTableClient("posts");
                return new ArticleService(tableClient, articleFacade, logger);
            });

            return services;
        }

        public static IServiceCollection AddAzureTableStorage(this IServiceCollection services, string storageConnectionString)
        {
            services.AddSingleton(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<TableServiceClient>>();
                if (string.IsNullOrEmpty(storageConnectionString))
                {
                    logger.LogError("AzureWebJobsStorage connection string is missing.");
                    throw new InvalidOperationException("AzureWebJobsStorage connection string is missing.");
                }
                return new TableServiceClient(storageConnectionString);
            });

            return services;
        }
    }
}