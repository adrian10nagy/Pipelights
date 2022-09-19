using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Pipelights.Database.Models;

namespace Pipelights.Database.Services
{
    public interface ICategoryDbService
    {
        Task<IEnumerable<CategoryEntity>> GetMultipleAsync(string query);
        Task<CategoryEntity> GetAsync(string id);
        Task AddAsync(CategoryEntity item);
        Task UpdateAsync(CategoryEntity item);
        Task DeleteAsync(string id);
    }

    public class CategoryDbService : ICategoryDbService
    {
        private Container _container;
        public CategoryDbService(
            CosmosClient cosmosDbClient,
            string databaseName,
            string containerName)
        {
            _container = cosmosDbClient.GetContainer(databaseName, containerName);
        }

        public async Task AddAsync(CategoryEntity item)
        {
            await _container.CreateItemAsync(item, new PartitionKey(item.id));
        }

        public async Task DeleteAsync(string id)
        {
            await _container.DeleteItemAsync<CategoryEntity>(id, new PartitionKey(id));
        }

        public async Task<CategoryEntity> GetAsync(string id)
        {
            try
            {
                var response = await _container.ReadItemAsync<CategoryEntity>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) //For handling item not found and other exceptions
            {
                return null;
            }
        }

        public async Task<IEnumerable<CategoryEntity>> GetMultipleAsync(string queryString)
        {
            var query = _container.GetItemQueryIterator<CategoryEntity>(new QueryDefinition(queryString));
            var results = new List<CategoryEntity>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }
            return results;
        }

        public async Task UpdateAsync(CategoryEntity item)
        {
            await _container.UpsertItemAsync(item);
        }
    }
}
