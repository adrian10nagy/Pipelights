using Microsoft.AspNetCore.Hosting;
using Pipelights.Database.Models;
using Pipelights.Database.Services;
using Pipelights.Website.BusinessService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pipelights.Website.Services.Interfaces;

namespace Pipelights.Website.BusinessService
{
    public interface ILampService
    {
        ProductDetailsDto GetById(string id);
        bool AddAsync(LampEntity item);
        bool UpdateAsync(string id, LampEntity item);
        bool DeleteAsync(string id);
        IEnumerable<ProductDetailsDto> GetMultiple(string query, bool includeInactive);
        IEnumerable<ProductDetailsDto> GetMultiple(bool includeInactive, int max = 1000);
        IEnumerable<ProductDetailsDto> GetSuggestions(bool includeInactive, int max = 1000);
    }

    public class LampService : ILampService
    {
        private readonly ILampDbService _lampDbService;
        private readonly IBlobService _blobService;

        public LampService(ILampDbService lampDbService, IBlobService blobService)
        {
            _lampDbService = lampDbService;
            _blobService = blobService;
        }

        public ProductDetailsDto GetById(string id)
        {
            var lamp = _lampDbService.GetAsync(id).Result;

            return new ProductDetailsDto(lamp, _blobService);
        }

        public bool AddAsync(LampEntity item)
        {
            try
            {
                var task = Task.Run(async () => await _lampDbService.AddAsync(item));
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
                var task = Task.Run(async () => await _lampDbService.UpdateAsync(item));
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

        public IEnumerable<ProductDetailsDto> GetMultiple(string query, bool includeInactive)
        {
            List<ProductDetailsDto> result = new List<ProductDetailsDto>();

            var dbResult = _lampDbService.GetMultipleAsync(query).Result;

            if (!includeInactive)
            {
                dbResult = dbResult.Where(x => !x.IsInactive);
            }

            foreach (var lamp in dbResult)
            {
                result.Add(new ProductDetailsDto(lamp, _blobService));
            }

            return result;
        }

        public IEnumerable<ProductDetailsDto> GetMultiple(bool includeInactive, int max = 1000)
        {

            var dbResult = GetMultiple("SELECT * FROM c order by c._ts DESC", includeInactive);

            var result = dbResult.Take(max);

            return result;
        }

        public IEnumerable<ProductDetailsDto> GetSuggestions(bool includeInactive, int max = 1000)
        {

            var dbResult = GetMultiple("SELECT TOP 20 * FROM c order by c._ts DESC", includeInactive);

            var result = dbResult.Where(x => x.CategoriesNew !=null 
            && (x.CategoriesNew.Contains("oameni") || x.Categories.Contains("lampi"))).Take(max);
            
            return result;
        }

    }
}
