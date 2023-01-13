using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Pipelights.Database.Models
{
    public class LampEntity
    {
        public string id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("technicalData")]
        public string TechnicalData { get; set; }

        [JsonPropertyName("price")]
        public string Price { get; set; }

        [JsonPropertyName("pricereduced")]
        public string PriceReduced { get; set; }

        [JsonPropertyName("isInactive")]
        public bool IsInactive { get; set; }

        [JsonPropertyName("isInactive")]
        public bool IsOnStock { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; }
    }
}
