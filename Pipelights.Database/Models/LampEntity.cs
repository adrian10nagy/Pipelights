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

        [JsonPropertyName("price")]
        public string Price { get; set; }

        [JsonPropertyName("pricereduced")]
        public string PriceReduced { get; set; }
    }
}
