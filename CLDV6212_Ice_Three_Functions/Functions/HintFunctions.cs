using CLDV6212_Ice_Three_Functions.Models;
using CLDV6212_Ice_Three_Functions.Services;
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
        private readonly BlobService _blobStorageService;
        private readonly ILogger<HintFunctions> _logger;

        public HintFunctions(TableStorageService tableStorageService, BlobService blobStorageService, ILogger<HintFunctions> log)
        {
            _tableStorageService = tableStorageService;
            _blobStorageService = blobStorageService;
            _logger = log;
        }

        [Function("GetHints")]
        public async Task<IActionResult> GetHintsAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "hints/{HintID}")] HttpRequest req, string HintID)
        {
            _logger.LogInformation("Processing GetHints request.");

            int iHintID = Int32.Parse(HintID);

            var hint = await _tableStorageService.GetHintByIdAsync("PartitionKey", iHintID);
            if (hint == null)
            {
                return new NotFoundObjectResult("Hint not found");
            }
            Stream blobContent = null;
            if (!string.IsNullOrEmpty(hint.HintImageUrl))
            {
                string img = "replace with file name";
                var imageUrl = await _blobStorageService.UploadsHintAsync(blobContent, img);
                hint.HintImageUrl = imageUrl;

                //blobContent = await _blobStorageService.GetBlobAsync(hint.HintImageUrl);
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
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "hints")] [FromBody] string model)
        {
            _logger.LogInformation("Processing AddHint request.");


            // var formData = await req.ReadFormAsync();


            // var imageFile = formData.Files.GetFile("HintImage");


            //  var hintJson = formData["HintData"];
            var hint = model;

            if (hint == null)
            {
                return new BadRequestObjectResult("Invalid hint data.");
            }


            string hintImageUrl = null;

            //   if (imageFile != null)
            //  {
            //      hintImageUrl = await _blobStorageService.UploadsHintAsync(imageFile.OpenReadStream(), imageFile.FileName);

            //     hint.HintImageUrl = hintImageUrl;
            //  }

            //  try
            //   {
            //       await _tableStorageService.AddHintAsync(hint);
            //}
            //   catch (Exception ex)
            //     {
            //      _logger.LogError(ex, "Error adding hint.");
            //    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            // }

            return new OkObjectResult("Hint added successfully.");
        }
        /*
                [Function("DeleteHint")]
                public async Task<IActionResult> DeleteHintAsync(
                [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "hints/{hintID}")] HttpRequest req, string hintID, ILogger log)
                {
                    log.LogInformation("Processing DeleteHint request.");

                    string partitionKey = "defaultPartition"; 
                    string rowKey = hintID;

                    try
                    {
                        await _tableStorageService.;
                        await _tableStorageService.DeleteHintAsync(partitionKey, rowKey);
                    }
                    catch (Exception ex)
                    {
                        log.LogError(ex, "Error deleting hint.");
                        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                    }

                    return new OkObjectResult("Hint deleted successfully.");
                }
        */

        [Function("GiveTeamHint")]
        public async Task<IActionResult> GiveTeamHintAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "hints/give/{hintID}/{teamID}")] HttpRequest req, string hintID, string teamID)
        {
            _logger.LogInformation("Processing GiveTeamHint request.");

            int iHintID = Int32.Parse(hintID);
            int iTeamID = Int32.Parse(teamID);

            var hint = await _tableStorageService.GetHintByIdAsync("defaultPartition", iHintID);

            if (hint == null)
            {
                return new NotFoundObjectResult("Hint not found.");
            }

            hint.TeamID = iTeamID;

            try
            {

                await _tableStorageService.UpdateHintTeamIDAsync(hint.PartitionKey, hint.RowKey, iHintID, iTeamID);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error giving hint to team.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return new OkObjectResult("Hint assigned to team successfully.");
        }


        [Function("MarkHintAsFound")]
        public async Task<IActionResult> MarkHintAsFoundAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "hints/markfound/{hintID}")] HttpRequest req, string hintID, ILogger log)
        {
            log.LogInformation("Processing MarkHintAsFound request.");

            int iHintID = Int32.Parse(hintID);

            var hint = await _tableStorageService.GetHintByIdAsync("defaultPartition", iHintID);

            if (hint == null)
            {
                return new NotFoundObjectResult("Hint not found.");
            }

            hint.IsFound = true;

            try
            {
                await _tableStorageService.UpdateHintFoundAsync(hint.PartitionKey, hint.RowKey, iHintID);

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

