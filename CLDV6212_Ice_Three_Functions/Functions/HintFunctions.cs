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

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var hint = JsonConvert.DeserializeObject<HintModel>(requestBody);

            if (hint == null)
            {
                return new BadRequestObjectResult("Invalid hint data.");
            }

            var formData = await req.ReadFormAsync();
            var imageFile = formData.Files.GetFile("HintImage");
            string hintImageUrl = null;

            if (imageFile != null)
            {
                hintImageUrl = await _blobStorageService.UploadBlobAsync(imageFile.OpenReadStream(), imageFile.FileName);
                hint.HintImageUrl = hintImageUrl;
            }

            try
            {
                await _tableStorageService.AddHintAsync(hint);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error adding hint.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

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
        [Function("GiveTeamHint")]
        public async Task<IActionResult> GiveTeamHintAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "hints/give/{hintID}/{teamID}")] HttpRequest req, string hintID, string teamID, ILogger log)
        {
            log.LogInformation("Processing GiveTeamHint request.");

            var hint = await _tableStorageService.GetHintAsync("defaultPartition", hintID);
            if (hint == null)
            {
                return new NotFoundObjectResult("Hint not found.");
            }

            hint.TeamID = teamID;

            try
            {
                await _tableStorageService.UpdateHintAsync(hint);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error giving hint to team.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return new OkObjectResult("Hint assigned to team successfully.");
        }
        [Function("MarkHintAsFound")]
        public async Task<IActionResult> MarkHintAsFoundAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "hints/markfound/{hintID}")] HttpRequest req, string hintID, ILogger log)
        {
            log.LogInformation("Processing MarkHintAsFound request.");

            var hint = await _tableStorageService.GetHintAsync("defaultPartition", hintID);
            if (hint == null)
            {
                return new NotFoundObjectResult("Hint not found.");
            }

            hint.IsFound = true;

            try
            {
                await _tableStorageService.UpdateHintAsync(hint);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error marking hint as found.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return new OkObjectResult("Hint marked as found.");
        }
    }
}

