using System.Text.Json.Serialization;
using System;

namespace Pipelights.Website.BusinessService.Models
{
    public class VoucherDetailsDto
    {
        public string id { get; set; }

        public string Name { get; set; }

        public string Percentage { get; set; }

        public decimal Discount { get; set; }
       
        public DateTime CreationDate { get; set; }

        public DateTime ExpiringDate { get; set; }

        public bool isActive { get; set; }
        public bool isSingleUse { get; set; }
        public bool isUsed { get; set; }

    }
}
