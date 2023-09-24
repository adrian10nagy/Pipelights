using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Pipelights.Database.Models;

namespace Pipelights.Database.Services
{
    public interface IVoucherDbService
    {
        Task<IEnumerable<VoucherEntity>> GetMultipleAsync(string query);
        Task<VoucherEntity> GetAsync(string id);
        Task AddAsync(VoucherEntity item);
        Task UpdateAsync(VoucherEntity item);
        Task DeleteAsync(string id);
    }

    public class VoucherDbService : IVoucherDbService
    {
        private Container _container;

        public VoucherDbService(
            CosmosClient cosmosDbClient,
            string databaseName,
            string containerName)
        {
            _container = cosmosDbClient.GetContainer(databaseName, containerName);
        }

        public async Task AddAsync(VoucherEntity item)
        {
            await _container.CreateItemAsync(item, new PartitionKey(item.id));
        }

        public async Task DeleteAsync(string id)
        {
            await _container.DeleteItemAsync<VoucherEntity>(id, new PartitionKey(id));
        }

        public async Task<VoucherEntity> GetAsync(string? id)
        {
            try
            {
                var response = await _container.ReadItemAsync<VoucherEntity>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) //For handling item not found and other exceptions
            {
                return null;
            }
        }

        public async Task<IEnumerable<VoucherEntity>> GetMultipleAsync(string queryString)
        {
            var query = _container.GetItemQueryIterator<VoucherEntity>(new QueryDefinition(queryString));
            var results = new List<VoucherEntity>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }
            return results;
        }

        public async Task UpdateAsync(VoucherEntity item)
        {
            await _container.UpsertItemAsync(item);
        }
    }
}
