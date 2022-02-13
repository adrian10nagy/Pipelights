using System.Collections.Generic;
using Pipelights.Database.Models;

namespace Pipelights.Database.BusinessService.Models
{
    public class ProductDetailsDto
    {
        public ProductDetailsDto()
        {
            
        }

        public ProductDetailsDto(LampEntity lamp)
        {
            Id = lamp.id;
            Name = lamp.Name;
            Description = lamp.Description;
            Price = lamp.Price;
            PriceReduced = lamp.PriceReduced;
            ImageFolder = Id + "/";
        }

        public List<string> Images { get; set; }
        public string Id { get; set; }
        public string ImageFolder { get; set; }
        public string MainImage { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public string PriceReduced { get; set; }
    }
}
