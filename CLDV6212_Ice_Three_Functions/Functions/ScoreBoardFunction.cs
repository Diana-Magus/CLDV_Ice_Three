using CLDV6212_Ice_Three_Functions.Models;
using CLDV6212_Ice_Three_Functions.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace CLDV6212_Ice_Three_Functions.Functions
{
    public class ScoreBoardFunction
    {
        private readonly ILogger<ScoreBoardFunction> _logger;

        private readonly TableStorageService _tableStorageService;

        public ScoreBoardFunction(ILogger<ScoreBoardFunction> logger)
        {
            _logger = logger;
            _tableStorageService = new TableStorageService();

        }

        [Function("ScoreBoardFunction")]
        public async Task<IActionResult> UpdateScoreBoardAsync([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            List<TeamModel> lstTeams = null;

            try
            {
                var teams = await _tableStorageService.GetTeamsAsync();
                var orderTeams = teams.OrderByDescending(x => x.TeamScore).ToList();

                lstTeams = orderTeams;
            }
            catch (Exception ex)
            {
                lstTeams = null;
            }
            
           
            

            return new OkObjectResult(lstTeams);

        }
    }
}
