using CLDV6212_Ice_Three_Functions.Models;
using CLDV6212_Ice_Three_Functions.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CLDV6212_Ice_Three_Functions.Functions
{
    public class TeamTableFunctions
    {
        private readonly ILogger<TeamTableFunctions> _logger;

        private readonly TableStorageService _storageService;

        
        public TeamTableFunctions(ILogger<TeamTableFunctions> logger)
        {
            _logger = logger;
           
        }


        /// <summary>
        /// Get all teams function
        /// </summary>

        [Function("GetAllTeams")]
        public async Task<IActionResult> GetAllTeams(
                    [HttpTrigger(AuthorizationLevel.Function, "get", Route = "teams")] HttpRequest req,
                    ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request to get all teams.");

            var teams = await _storageService.GetTeamsAsync();
            return new OkObjectResult(teams);
        }



        /// <summary>
        /// Delete Teams Function (Potentially may not work)
        /// </summary>
        
        [Function("AddTeam")]
        public async Task<IActionResult> AddTeam(
              [HttpTrigger(AuthorizationLevel.Function, "post", Route = "teams")] HttpRequest req,
              ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request to add a team.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            TeamModel team;

            try
            {
                team = JsonSerializer.Deserialize<TeamModel>(requestBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (JsonException)
            {
                return new BadRequestObjectResult("Invalid team data. Unable to parse JSON.");
            }

            if (team == null)
            {
                return new BadRequestObjectResult("Invalid team data.");
            }

            var teams = await _storageService.GetTeamsAsync();
            int nextTeamId = teams.Count > 0 ? teams.Max(c => c.TeamID) + 1 : 1;
            team.TeamID = nextTeamId;

            team.PartitionKey = "TeamPartition";
            team.RowKey = Guid.NewGuid().ToString();
            await _storageService.AddTeamAsync(team);

            return new OkObjectResult(team);
        }

       /// <summary>
       /// Delete Teams Function
       /// </summary>
       

        [Function("DeleteTeam")]
        public async Task<IActionResult> DeleteTeam(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "teams/{partitionKey}/{rowKey}")] HttpRequest req,
            string partitionKey,
            string rowKey,
            ILogger log)
        {
            log.LogInformation($"C# HTTP trigger function processed a request to delete a team: {partitionKey}/{rowKey}");

            await _storageService.DeleteTeamAsync(partitionKey, rowKey);
            return new OkResult();
        }
    }
}

