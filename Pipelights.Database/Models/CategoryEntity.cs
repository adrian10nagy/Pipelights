using System.Text.Json.Serialization;

namespace Pipelights.Database.Models
{
    public class CategoryEntity
    {
        public string id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
