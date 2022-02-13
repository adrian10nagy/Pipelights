using System;
using System.Collections.Generic;
using Pipelights.Database.BusinessService.Models;
using Pipelights.Database.Models;
using Pipelights.Database.Services;
using System.Threading.Tasks;

namespace Pipelights.Database.BusinessService
{
    public interface ILampService
    {
        ProductDetailsDto GetById(string id);
        bool AddAsync(LampEntity item);
        bool UpdateAsync(string id, LampEntity item);
        bool DeleteAsync(string id);
        IEnumerable<ProductDetailsDto> GetMultiple(string query);
    }

    public class LampService : ILampService
    {
        private readonly ICosmosDbService _cosmosDbService;

        public LampService(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        public ProductDetailsDto GetById(string id)
        {
            var lamp = _cosmosDbService.GetAsync(id).Result;

            return new ProductDetailsDto(lamp);
        }

        public bool AddAsync(LampEntity item)
        {
            try
            {
                var task = Task.Run(async () => await _cosmosDbService.AddAsync(item));
                task.GetAwaiter().GetResult();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }


        }

        public bool UpdateAsync(string id, LampEntity item)
        {
            try
            {
                var task = Task.Run(async () => await _cosmosDbService.UpdateAsync(id, item));
                task.GetAwaiter().GetResult();

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public bool DeleteAsync(string id)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<ProductDetailsDto> GetMultiple(string query)
        {
            var result =  _cosmosDbService.GetMultipleAsync("SELECT * FROM c").Result;


        }
    }
}
