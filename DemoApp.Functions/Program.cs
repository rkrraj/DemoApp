using Azure.Data.Tables;
using DemoApp.Service.Facade;
using DemoApp.Service.Proxy;
using DemoApp.Service;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services.AddHttpClient();
builder.Services.AddSingleton(sp =>
{
    var logger = sp.GetRequiredService<ILogger<TableServiceClient>>();
    string? storageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
    if (string.IsNullOrEmpty(storageConnectionString))
    {
        logger.LogError("AzureWebJobsStorage connection string is missing.");
        // This is a critical error and throw an exception to stop function startup. Azure function needs webjob storage setup
        throw new InvalidOperationException("AzureWebJobsStorage connection string is missing.");
    }
    return new TableServiceClient(storageConnectionString);
});

builder.Services.AddTransient<IArticleService, ArticleService>(sp =>
{
    var articleFacade = sp.GetRequiredService<IArticleFacade>();
    var tableServiceClient = sp.GetRequiredService<TableServiceClient>();
    var logger = sp.GetRequiredService<ILogger<ArticleService>>();

    var tableClient = tableServiceClient.GetTableClient("posts"); // Get the TableClient here

    return new ArticleService(tableClient, articleFacade, logger); // Inject the TableClient
});
builder.Services.AddTransient<IApiClient, ApiClient>();
builder.Services.AddTransient<IArticleFacade, ArticleFacade>();

builder.Build().Run();
