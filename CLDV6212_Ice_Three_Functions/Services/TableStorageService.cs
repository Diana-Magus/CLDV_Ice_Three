using Azure.Data.Tables;
using CLDV6212_Ice_Three_Functions.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CLDV6212_Ice_Three_Functions.Services
{
    public class TableStorageService
    {
        private readonly TableClient _tableClient;

        public TableStorageService(string tableName, string connectionString)
        {
            var serviceClient = new TableServiceClient(connectionString);
            _tableClient = serviceClient.GetTableClient(tableName);
            _tableClient.CreateIfNotExists();
        }

        public async Task AddEntityAsync<T>(T entity) where T : class, ITableEntity
        {
            await _tableClient.AddEntityAsync(entity);
        }

        public async Task DeleteEntityAsync(string partitionKey, string rowKey)
        {
            await _tableClient.DeleteEntityAsync(partitionKey, rowKey);
        }

        public async Task<IEnumerable<T>> GetAllEntitiesAsync<T>() where T : class, ITableEntity, new()
        {
            var entities = new List<T>();
            await foreach (var entity in _tableClient.QueryAsync<T>())
            {
                entities.Add(entity);
            }
            return entities;
        }
    }
}