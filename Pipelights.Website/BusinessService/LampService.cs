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
        IEnumerable<ProductDetailsDto> GetRobots(bool includeInactive, int max = 1000);
        IEnumerable<ProductDetailsDto> GetPipeLeather(bool includeInactive, int max = 1000);
        IEnumerable<ProductDetailsDto> GetSports(bool includeInactive, int max = 1000);
        IEnumerable<ProductDetailsDto> GetMusic(bool includeInactive, int max = 1000);
        IEnumerable<ProductDetailsDto> GetDrink(bool includeInactive, int max = 1000);
        IEnumerable<ProductDetailsDto> GetTravel(bool includeInactive, int max = 1000);
        IEnumerable<ProductDetailsDto> GetFood(bool includeInactive, int max = 1000);
        IEnumerable<ProductDetailsDto> GetHobby(bool includeInactive, int max = 1000);
        IEnumerable<ProductDetailsDto> GetJob(bool includeInactive, int max = 1000);
        IEnumerable<ProductDetailsDto> GetBusiness(bool includeInactive, int max = 1000);
        IEnumerable<ProductDetailsDto> GetOther(bool includeInactive, int max = 1000);
        IEnumerable<ProductDetailsDto> GetTwoBulbs(bool includeInactive, int max = 1000);



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
            && (x.CategoriesNew.Contains("oameni"))).Take(max);
            
            return result;
        }

        public IEnumerable<ProductDetailsDto> GetRobots(bool includeInactive, int max = 1000)
        {
            var result = GetMultiple("SELECT * FROM c Where Array_Contains(c.Categories, 'roboti')", includeInactive);
           
            return result;
        }


        public IEnumerable<ProductDetailsDto> GetPipeLeather(bool includeInactive, int max = 1000)
        {
            var result = GetMultiple("SELECT * FROM c Where Array_Contains(c.Categories, 'leather')", includeInactive);

            return result;
        }
        public IEnumerable<ProductDetailsDto> GetTwoBulbs(bool includeInactive, int max = 1000)
        {
            var result = GetMultiple("SELECT * FROM c Where Array_Contains(c.Categories, 'twoBulbs')", includeInactive);

            return result;
        }


        public IEnumerable<ProductDetailsDto> GetSports(bool includeInactive, int max = 1000)
        {
            var result = GetMultiple("SELECT * FROM c Where Array_Contains(c.Categories, 'sport')", includeInactive);

            return result;
        }

        public IEnumerable<ProductDetailsDto> GetMusic(bool includeInactive, int max = 1000)
        {
            var result = GetMultiple("SELECT * FROM c Where Array_Contains(c.Categories, 'music')", includeInactive);

            return result;
        }

        public IEnumerable<ProductDetailsDto> GetFood(bool includeInactive, int max = 1000)
        {
            var result = GetMultiple("SELECT * FROM c Where Array_Contains(c.Categories, 'food')", includeInactive);

            return result;
        }
        public IEnumerable<ProductDetailsDto> GetDrink(bool includeInactive, int max = 1000)
        {
            var result = GetMultiple("SELECT * FROM c Where Array_Contains(c.Categories, 'drink')", includeInactive);

            return result;
        }

        public IEnumerable<ProductDetailsDto> GetTravel(bool includeInactive, int max = 1000)
        {
            var result = GetMultiple("SELECT * FROM c Where Array_Contains(c.Categories, 'travel')", includeInactive);

            return result;
        }

        public IEnumerable<ProductDetailsDto> GetHobby(bool includeInactive, int max = 1000)
        {
            var result = GetMultiple("SELECT * FROM c Where Array_Contains(c.Categories, 'hobby')", includeInactive);

            return result;
        }

        public IEnumerable<ProductDetailsDto> GetJob(bool includeInactive, int max = 1000)
        {
            var result = GetMultiple("SELECT * FROM c Where Array_Contains(c.Categories, 'job')", includeInactive);

            return result;
        }

        public IEnumerable<ProductDetailsDto> GetBusiness(bool includeInactive, int max = 1000)
        {
            var result = GetMultiple("SELECT * FROM c Where Array_Contains(c.Categories, 'business')", includeInactive);

            return result;
        }

        public IEnumerable<ProductDetailsDto> GetOther(bool includeInactive, int max = 1000)
        {
            var result = GetMultiple("SELECT * FROM c Where Array_Contains(c.Categories, 'other')", includeInactive);

            return result;
        }
    }
}
