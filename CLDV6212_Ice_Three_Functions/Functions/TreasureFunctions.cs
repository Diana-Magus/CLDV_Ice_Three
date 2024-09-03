using Azure;
using Azure.Data.Tables;
using CLDV6212_Ice_Three_Functions.Models;
using CLDV6212_Ice_Three_Functions.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CLDV6212_Ice_Three_Functions.Functions
{
    //(Mrzyglod, 2022)
    public class TreasureFunctions
    {
        private readonly ILogger<TreasureFunctions> _logger;
        private readonly TableStorageService _tableStorageService;

        public TreasureFunctions(ILogger<TreasureFunctions> logger, TableStorageService tableStorageService)
        {
            _logger = logger;
            _tableStorageService = tableStorageService;
        }

        // Function to Add a Treasure
        [Function("AddTreasure")]
        public async Task<IActionResult> AddTreasure([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)//(Microsoft, 2024)
        {
            _logger.LogInformation("Adding a treasure.");

            // Retrieve treasure details from the request
            var treasure = new TreasureModel
            {
                PartitionKey = "TreasurePartition",
                RowKey = Guid.NewGuid().ToString(),
                TreasureID = int.Parse(req.Form["TreasureID"]),
                TreasureName = req.Form["TreasureName"],
                TreasureDescription = req.Form["TreasureDescription"],
                TreasureImageUrl = req.Form["TreasureImageUrl"]
            };

            // Add the treasure to table storage
            await _tableStorageService.AddTreasureAsync(treasure);
            return new OkObjectResult("Treasure added successfully.");
        }//(Mrzyglod, 2022)

        // Function to Delete a Treasure
        [Function("DeleteTreasure")]//(Microsoft, 2024)
        public async Task<IActionResult> DeleteTreasure([HttpTrigger(AuthorizationLevel.Function, "delete")] HttpRequest req)
        {
            _logger.LogInformation("Deleting a treasure.");

            // Retrieve partitionKey and rowKey from query parameters
            string partitionKey = req.Query["partitionKey"];
            string rowKey = req.Query["rowKey"];

            // Delete the treasure from table storage
            await _tableStorageService.DeleteTreasureAsync(partitionKey, rowKey);
            return new OkObjectResult("Treasure deleted successfully.");
        }//(Mrzyglod, 2022)

        // Function to Get All Treasures
        [Function("GetAllTreasures")]//(Microsoft, 2024)
        public async Task<IActionResult> GetAllTreasures([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            _logger.LogInformation("Getting all treasures.");

            // Retrieve all treasures from table storage
            IEnumerable<TreasureModel> treasures = await _tableStorageService.GetTreasuresAsync();
            return new OkObjectResult(treasures);
        }
    }
}
//PM1: Mrzyglod, K. 2022. Azure for Developers: Implement rich Azure PaaS ecosystems using containers, serverless services, and storage solutions. ISBN: 9781803240091
//Microsoft. 2024. Azure Functions HTTP trigger. [Online]. Available at: https://learn.microsoft.com/en-us/azure/azure-functions/functions-bindings-http-webhook-trigger?tabs=python-v2%2Cisolated-process%2Cnodejs-v4%2Cfunctionsv2&pivots=programming-language-csharp [Accessed 30 August 2024].