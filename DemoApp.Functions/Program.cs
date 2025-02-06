using Azure.Data.Tables;
using DemoApp.DependencyInjection;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services.AddHttpClient();
string storageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
builder.Services.AddAzureTableStorage(storageConnectionString);
builder.Services.AddApplicationServices();

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var tableServiceClient = scope.ServiceProvider.GetRequiredService<TableServiceClient>();
    var tableClient = tableServiceClient.GetTableClient("posts");
    await tableClient.CreateIfNotExistsAsync();
}

host.Run();
