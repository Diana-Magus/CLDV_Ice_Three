using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CLDV6212_Ice_Three_Functions.Services;
using Microsoft.Extensions.Configuration;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context, services) =>
    {
        // Configure Application Insights
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        // Add TableStorageService as a singleton
        services.AddSingleton<TableStorageService>(sp =>
        {
            var storageConnectionString = context.Configuration.GetConnectionString("AzureWebJobsStorage");
            return new TableStorageService("TreasuresTable", storageConnectionString);
        });
    })
    .Build();

host.Run();