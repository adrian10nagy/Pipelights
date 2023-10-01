using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Pipelights.Database.Models
{
    public class VoucherEntity
    {
        public string id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("percentage")]
        public string Percentage { get; set; }

        [JsonPropertyName("discount")]
        public decimal Discount { get; set; }

        [JsonPropertyName("creationDate")]
        public DateTime CreationDate { get; set; }

        [JsonPropertyName("expiringDate")]
        public DateTime ExpiringDate { get; set; }


        [JsonPropertyName("isActive")]
        public bool isActive { get; set; }

        [JsonPropertyName("isUsed")]
        public bool isUsed { get; set; }

        [JsonPropertyName("isSingleUse")]
        public bool isSingleUse { get; set; }
    }
}
