using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Pipelights.Database.Models
{
    public class OrderEntity
    {
        public string id { get; set; }
        
        [JsonPropertyName("telephone")]
        public string Telephone { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("surname")]
        public string Surname { get; set; }

        [JsonPropertyName("judete")]
        public string Judete { get; set; }

        [JsonPropertyName("localitate")]
        public string Localitate { get; set; }

        [JsonPropertyName("payment")]
        public string Payment { get; set; }

        [JsonPropertyName("address")]
        public string Address { get; set; }

        [JsonPropertyName("subtotal")]
        public decimal Subtotal { get; set; }

        [JsonPropertyName("placedDate")]
        public DateTime PlacedDate { get; set; }


        [JsonPropertyName("products")]
        public List<CartProductsEntity> CartProducts { get; set; }

    }

    public class CartProductsEntity
    {
        [JsonPropertyName("lampid")]
        public string LampId { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
        
        [JsonPropertyName("price")]
        public string Price { get; set; }
    }
}
