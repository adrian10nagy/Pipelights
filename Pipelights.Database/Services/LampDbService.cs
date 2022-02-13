using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Pipelights.Database.Models;

namespace Pipelights.Database.Services
{
    public interface ILampDbService
    {
        Task<IEnumerable<LampEntity>> GetMultipleAsync(string query);
        Task<LampEntity> GetAsync(string id);
        Task AddAsync(LampEntity item);
        Task UpdateAsync(LampEntity item);
        Task DeleteAsync(string id);
    }

    public class LampDbService : ILampDbService
    {
        private Container _container;
        public LampDbService(
            CosmosClient cosmosDbClient,
            string databaseName,
            string containerName)
        {
            _container = cosmosDbClient.GetContainer(databaseName, containerName);
        }
        public async Task AddAsync(LampEntity item)
        {
            await _container.CreateItemAsync(item, new PartitionKey(item.id));
        }
        public async Task DeleteAsync(string id)
        {
            await _container.DeleteItemAsync<LampEntity>(id, new PartitionKey(id));
        }
        public async Task<LampEntity> GetAsync(string id)
        {
            try
            {
                var response = await _container.ReadItemAsync<LampEntity>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) //For handling item not found and other exceptions
            {
                return null;
            }
        }
        public async Task<IEnumerable<LampEntity>> GetMultipleAsync(string queryString)
        {
            var query = _container.GetItemQueryIterator<LampEntity>(new QueryDefinition(queryString));
            var results = new List<LampEntity>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }
            return results;
        }
        public async Task UpdateAsync(LampEntity item)
        {
            await _container.UpsertItemAsync(item);
        }
    }
}
