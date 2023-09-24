using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Pipelights.Database.Models;

namespace Pipelights.Database.Services
{
    public interface IOrderDbService
    {
        Task<IEnumerable<OrderEntity>> GetMultipleAsync(string query);
        Task<OrderEntity> GetAsync(string id);
        Task AddAsync(OrderEntity item);
        Task UpdateAsync(OrderEntity item);
        Task DeleteAsync(string id);
    }

    public class OrderDbService: IOrderDbService
    {
        private Container _container;
        public OrderDbService(
            CosmosClient cosmosDbClient,
            string databaseName,
            string containerName)
        {
            _container = cosmosDbClient.GetContainer(databaseName, containerName);
        }
        public async Task AddAsync(OrderEntity item)
        {
            await _container.CreateItemAsync(item, new PartitionKey(item.id));
        }
        public async Task DeleteAsync(string id)
        {
            await _container.DeleteItemAsync<OrderEntity>(id, new PartitionKey(id));
        }
        public async Task<OrderEntity> GetAsync(string id)
        {
            try
            {
                var response = await _container.ReadItemAsync<OrderEntity>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) //For handling item not found and other exceptions
            {
                return null;
            }
        }
        public async Task<IEnumerable<OrderEntity>> GetMultipleAsync(string queryString)
        {
            var query = _container.GetItemQueryIterator<OrderEntity>(new QueryDefinition(queryString));
            var results = new List<OrderEntity>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }
            return results;
        }
        public async Task UpdateAsync(OrderEntity item)
        {
            var updatedOrder = await _container.UpsertItemAsync(item);

        }
    }
}
