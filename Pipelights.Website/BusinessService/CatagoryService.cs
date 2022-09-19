
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
    public interface ICategoryService
    {
        ProductDetailsDto GetById(string id);
        bool AddAsync(LampEntity item);
        bool UpdateAsync(string id, LampEntity item);
        bool DeleteAsync(string id);
        IEnumerable<CategoryEntity> GetMultiple(string query);
        IEnumerable<CategoryEntity> GetMultiple(int max = 1000);
    }

    public class CategoryService : ICategoryService
    {
        private readonly ICategoryDbService _lampDbService;

        public CategoryService(ICategoryDbService categoryDbService)
        {
            _lampDbService = categoryDbService;
        }

        public ProductDetailsDto GetById(string id)
        {
            throw new System.NotImplementedException();

            //var lamp = _lampDbService.GetAsync(id).Result;

            //return new ProductDetailsDto(lamp, _webHostEnvironment, _blobService);
        }

        public bool AddAsync(LampEntity item)
        {
            throw new System.NotImplementedException();
            //try
            //{
            //    var task = Task.Run(async () => await _lampDbService.AddAsync(item));
            //    task.GetAwaiter().GetResult();
            //    return true;
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //    return false;
            //}


        }

        public bool UpdateAsync(string id, LampEntity item)
        {
            throw new System.NotImplementedException();
            //try
            //{
            //    var task = Task.Run(async () => await _lampDbService.UpdateAsync(item));
            //    task.GetAwaiter().GetResult();

            //    return true;
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //    return false;
            //}
        }

        public bool DeleteAsync(string id)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<CategoryEntity> GetMultiple(string query)
        {

            var dbResult = _lampDbService.GetMultipleAsync(query).Result;


            return dbResult;
        }

        public IEnumerable<CategoryEntity> GetMultiple(int max = 1000)
        {
            var dbResult = GetMultiple("SELECT * FROM c order by c._ts DESC");

            var result = dbResult.Take(max);

            return result;
        }
    }
}
