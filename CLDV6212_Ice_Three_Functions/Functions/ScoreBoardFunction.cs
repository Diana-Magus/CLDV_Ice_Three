using CLDV6212_Ice_Three_Functions.Models;
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

        public ScoreBoardFunction(ILogger<ScoreBoardFunction> logger)
        {
            _logger = logger;
        }

        [Function("ScoreBoardFunction")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            List<TeamModel> lstTeams = null;


            var teams = await _tableStorageService.GetAllTeamsAsync();
           
            var orderTeams = teams.OrderByDescending(x => x.TeamScore).ToList();

            lstTeams = orderTeams;

            return new OkObjectResult(lstTeams);

        }
    }
}
