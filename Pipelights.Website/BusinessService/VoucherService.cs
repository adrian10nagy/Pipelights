
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
    public interface IVoucherService
    {
        VoucherEntity GetById(string id);
        bool AddAsync(VoucherEntity item);
        bool UpdateAsync(string id, VoucherEntity item);
        bool DeleteAsync(string id);
        IEnumerable<VoucherEntity> GetMultiple(string query);
        IEnumerable<VoucherEntity> GetMultiple(int max = 1000);
        decimal VouckerCheck(string s);

    }

    public class VoucherService : IVoucherService
    {
        private readonly IVoucherDbService _voucherDbService;

        public VoucherService(IVoucherDbService voucherService)
        {
            _voucherDbService = voucherService;
        }

        public VoucherEntity GetById(string? id)
        {
            return _voucherDbService.GetAsync(id).Result;
        }

        public bool AddAsync(VoucherEntity item)
        {

            try
            {
                var task = Task.Run(async () => await _voucherDbService.AddAsync(item));
                task.GetAwaiter().GetResult();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }


        }

        public bool UpdateAsync(string id, VoucherEntity item)
        {

            try
            {
                var task = Task.Run(async () => await _voucherDbService.UpdateAsync(item));
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

        public IEnumerable<VoucherEntity> GetMultiple(string query)
        {

            var dbResult = _voucherDbService.GetMultipleAsync(query).Result;


            return dbResult;
        }

        public IEnumerable<VoucherEntity> GetMultiple(int max = 1000)
        {
            var dbResult = GetMultiple("SELECT * FROM c order by c._ts DESC");

            var result = dbResult.Take(max);

            return result;
        }



        public decimal VouckerCheck(string s)
        {
            var voucher = GetById(s);

            decimal discount = 1;

            var dateNow = DateTime.Now;

            if (voucher != null)
            {
                if (voucher.ExpiringDate < dateNow || voucher.isUsed)
                {
                    voucher.isActive = false;
                }

                if (voucher.isActive)
                {
                    discount = voucher.Discount;
                }
            }

            return discount;
        }


    }
}
