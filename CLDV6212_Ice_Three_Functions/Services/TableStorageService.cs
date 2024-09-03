using Azure.Data.Tables;
using Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDV6212_Ice_Three_Functions.Models;

namespace CLDV6212_Ice_Three_Functions.Services
{
    internal class TableStorageService
    {


        private readonly string connectionString = "defaultEndpointsProtocol=https;AccountName=clvd6212ice3;AccountKey=973k4FLw7sEDEEQX/DMQe4lRp46dJRUwIjyquB1DYPDAJzKDDPJh85ms+epGWvu8RTFQpap1mM44+AStLe8V5Q==;EndpointSuffix=core.windows.net";

        private readonly TableClient _teamTableClient;
        private readonly TableClient _treasureTableClient;
        private readonly TableClient _hintTableClient;

        public TableStorageService(string connectionString)
        {
            _teamTableClient = new TableClient(connectionString, "Team");
            _treasureTableClient = new TableClient(connectionString, "Treasure");
            _hintTableClient = new TableClient(connectionString, "Hint");
        }


        /// <summary>
        /// TeamMethods
        /// </summary>


        public async Task<List<TeamModel>> GetTeamsAsync()
        {
            var teams = new List<TeamModel>();
            await foreach (var team in _teamTableClient.QueryAsync<TeamModel>())
            {
                teams.Add(team);
            }
            return teams;
        }

        public async Task AddTeamAsync(TeamModel team)
        {
            if (string.IsNullOrEmpty(team.PartitionKey) || string.IsNullOrEmpty(team.RowKey))
            {
                throw new ArgumentException("PartitionKey and RowKey must be set");
            }
            try
            {
                await _teamTableClient.AddEntityAsync(team);
            }
            catch (RequestFailedException ex)
            {
                throw new InvalidOperationException("Error adding team to table storage", ex);
            }
        }

        public async Task DeleteTeamAsync(string partitionKey, string rowKey)
        {
            await _teamTableClient.DeleteEntityAsync(partitionKey, rowKey);
        }

        public async Task<TeamModel?> GetTeamAsync(string partitionKey, string rowKey)
        {
            try
            {
                var response = await _teamTableClient.GetEntityAsync<TeamModel>(partitionKey, rowKey);
                return response.Value;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return null;
            }
        }

        public async Task<TeamModel?> GetTeamByIdAsync(string partitionKey, int teamId)
        {
            var query = _teamTableClient.QueryAsync<TeamModel>(filter: $"PartitionKey eq '{partitionKey}' and TeamID eq {teamId}");
            await foreach (var team in query)
            {
                return team;
            }
            return null;
        }

        /// <summary>
        /// Treasure Methods
        /// </summary>
      

        public async Task<List<TreasureModel>> GetTreasuresAsync()
        {
            var treasures = new List<TreasureModel>();
            await foreach (var treasure in _treasureTableClient.QueryAsync<TreasureModel>())
            {
                treasures.Add(treasure);
            }
            return treasures;
        }

        public async Task AddTreasureAsync(TreasureModel treasure)
        {
            if (string.IsNullOrEmpty(treasure.PartitionKey) || string.IsNullOrEmpty(treasure.RowKey))
            {
                throw new ArgumentException("PartitionKey and RowKey must be set");
            }
            try
            {
                await _treasureTableClient.AddEntityAsync(treasure);
            }
            catch (RequestFailedException ex)
            {
                throw new InvalidOperationException("Error adding treasure to table storage", ex);
            }
        }

        public async Task DeleteTreasureAsync(string partitionKey, string rowKey)
        {
            await _treasureTableClient.DeleteEntityAsync(partitionKey, rowKey);
        }

        public async Task<TreasureModel?> GetTreasureAsync(string partitionKey, string rowKey)
        {
            try
            {
                var response = await _treasureTableClient.GetEntityAsync<TreasureModel>(partitionKey, rowKey);
                return response.Value;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return null;
            }
        }

        public async Task<TreasureModel?> GetTreasureByIdAsync(string partitionKey, int treasureId)
        {
            var query = _treasureTableClient.QueryAsync<TreasureModel>(filter: $"PartitionKey eq '{partitionKey}' and TreasureID eq {treasureId}");
            await foreach (var treasure in query)
            {
                return treasure;
            }
            return null;
        }

        /// <summary>
        /// Hint Methods 
        /// </summary>

        public async Task AddHintAsync(HintModel hint)
        {
            if (string.IsNullOrEmpty(hint.PartitionKey) || string.IsNullOrEmpty(hint.RowKey))
            {
                throw new ArgumentException("Partition key and row key must be set.");
            }

            try
            {
                await _hintTableClient.AddEntityAsync(hint);
            }
            catch (RequestFailedException ex)
            {
                throw new InvalidOperationException("Error adding hint to table storage", ex);
            }
        }

        public async Task<List<HintModel>> GetAllHintsAsync()
        {
            var hints = new List<HintModel>();
            await foreach (var hint in _hintTableClient.QueryAsync<HintModel>())
            {
                hints.Add(hint);
            }
            return hints;
        }

        public async Task<TeamModel?> GetHintByIdAsync(string partitionKey, int hintID)
        {
            var query = _teamTableClient.QueryAsync<HintModel>(filter: $"PartitionKey eq '{partitionKey}' and HintID eq {hintID}");
            await foreach (var hint in query)
            {
                return hint;
            }
            return null;
        }

    }
}
