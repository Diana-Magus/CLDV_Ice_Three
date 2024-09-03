using CLDV6212_Ice_Three_Functions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CLDV6212_Ice_Three_Functions.Functions
{
    public class HintFunctions
    {
        private readonly TableStorageService _tableStorageService;
        private readonly BlobStorageService _blobStorageService;

        public HintFunctions(TableStorageService tableStorageService, BlobStorageService blobStorageService)
        {
            _tableStorageService = tableStorageService;
            _blobStorageService = blobStorageService;
        }

        [Function("GetHints")]
        public async Task<IActionResult> GetHintsAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "hints/{HintID}")] HttpRequest req,string HintID,ILogger log)
        {
            log.LogInformation("Processing GetHints request.");

            var hint = await _tableStorageService.GetHintAsync("PartitionKey", HintID);
            if (hint == null)
            {
                return new NotFoundObjectResult("Hint not found");
            }
            Stream blobContent = null;
            if (!string.IsNullOrEmpty(hint.HintImageUrl))
            {
                blobContent = await _blobStorageService.GetBlobAsync(hint.HintImageUrl);
            }
            var response = new
            {
                hint.HintName,
                hint.HintText,
                hint.HintAnswer,
                HintBlobContent = blobContent != null ? ConvertToBase64(blobContent) : null
            };
            return new OkObjectResult(hint);
        }

        //referencing image converter(https://learn.microsoft.com/en-us/dotnet/api/system.convert.tobase64string?view=net-8.0)

        private string ConvertToBase64(Stream stream)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                byte[] byteArray = memoryStream.ToArray();
                return Convert.ToBase64String(byteArray);
            }
        }

        [Function("AddHint")]
        public async Task<IActionResult> AddHintAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "hints")] HttpRequest req, ILogger log)
        {
            log.LogInformation("Processing AddHint request.");

            var formData = await req.ReadFormAsync();

            var hintJson = formData["HintModel"];
            var hint = JsonConvert.DeserializeObject<HintModel>(hintJson);

            var imageFile = formData.Files.GetFile("HintImage");
            string hintImageUrl = null;

            if (imageFile != null)
            {
                hintImageUrl = await _blobStorageService.UploadBlobAsync(imageFile.OpenReadStream(), imageFile.FileName);
                hint.HintImageUrl = hintImageUrl;
            }

            if (hint == null)
            {
                return new BadRequestObjectResult("Invalid hint data.");
            }

            await _tableStorageService.AddHintAsync(hint);
            return new OkObjectResult("Hint added successfully.");
        }

        [Function("DeleteHint")]
        public async Task<IActionResult> DeleteHintAsync(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "hints/{hintID}")] HttpRequest req, string hintID, ILogger log)
        {
            log.LogInformation("Processing DeleteHint request.");

            string partitionKey = "defaultPartition"; 
            string rowKey = hintID;

            try
            {
                await _tableStorageService.DeleteHintAsync(partitionKey, rowKey);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error deleting hint.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return new OkObjectResult("Hint deleted successfully.");
        }
    }
}

